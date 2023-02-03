using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADO.Repos.Application;
using ADO.Repos.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ADO.Repos.ExternalServices
{
    public class AdoExternalService : IAdoExternalService
    {
        private readonly AdoConfig _adoOptions;
        private readonly GitHttpClient _gitClient;
        private readonly ILogger<AdoExternalService> _logger;

        public AdoExternalService(IOptions<AdoConfig> adoOptions, ILogger<AdoExternalService> logger)
        {
            _adoOptions = adoOptions?.Value ?? throw new ArgumentNullException(nameof(adoOptions)); ;
            var creds = new VssBasicCredential(string.Empty, _adoOptions.Pat);
            var adoConnection = new VssConnection(new Uri(_adoOptions.BaseUrl), creds);
            _gitClient = adoConnection.GetClient<GitHttpClient>();
            this._logger = logger;
        }

        public Task<Repository> GetRepository(RepoConfig repoConfig)
        {
            var repository = new Repository(
                repoConfig.Id,
                repoConfig.Name,
                new Uri($"{_adoOptions.BaseRepositoryUrl}/{repoConfig.Name}")
            );

            return Task.FromResult(repository);
        }

        public async Task<IEnumerable<Branch>> StaleBranches(Repository repository)
        {
            var branches = await GetRepoBranches(repository);

            return branches
                .Where(b => b.Commit.Committer.Date < DateTime.UtcNow.AddDays(-_adoOptions.StaleBranchThreshholdInDays))
                .Select(b => b.AsBranch(repository));
        }

        public async Task<IEnumerable<Branch>> GetBranchesAheadOf(string targetBranchName, Repository repository)
        {
            var branchesAhead = new List<Branch>();

            var branches = await GetRepoBranches(repository);

            var target = branches.Find(targetBranchName);
            if (target == null)
                return branchesAhead;

            var projectName = _adoOptions.ProjectName;
            var branchesToCompare = GetBranchesToCompare(target, branches);

            foreach (var comparingBranch in branchesToCompare)
            {
                if (comparingBranch.Commit == null)
                    continue;

                var firstBasesCommit = await GetMergeBasesAsync(repository, projectName, target.Commit.CommitId, comparingBranch.Commit.CommitId);

                if (firstBasesCommit?.Committer.Date < comparingBranch.Commit.Committer.Date)
                    branchesAhead.Add(comparingBranch.AsBranch(repository));
            }

            return branchesAhead;
        }

        public async Task<bool> SourceIsAheadOfTarget(Repository repository, Branch sourceBranch, Branch targetBranch)
        {
            var diff = await _gitClient.GetCommitDiffsAsync(
                _adoOptions.ProjectName,
                repository.Id.ToString(),
                baseVersionDescriptor: new GitBaseVersionDescriptor { VersionType = GitVersionType.Commit, Version = sourceBranch.CommitId },
                targetVersionDescriptor: new GitTargetVersionDescriptor { VersionType = GitVersionType.Commit, Version = targetBranch.CommitId });

            return diff.ChangeCounts.Count > 0;
        }

        public async Task<bool> BranchIsLocked(Repository repository, Branch branch)
        {
            var refs = await _gitClient.GetRefsAsync(_adoOptions.ProjectName, repository.Id, filterContains: branch.Name);
            var branchRef = refs.SingleOrDefault(r => r.Name.EndsWith(branch.Name));

            return branchRef.IsLocked;
        }

        public async Task LockBranch(Guid repositoryId, Branch branch)
        {
            try
            {
                var model = new GitRefUpdate { IsLocked = true };
                await _gitClient.UpdateRefAsync(model, repositoryId: repositoryId, filter: branch.Name.ToBranchHeadName());
            }
            catch (VssServiceException ex)
            {
                _logger.LogWarning($"Failed to lock branch because: {ex.Message}");
            }
        }

        public async Task UnlockBranch(Guid repositoryId, Branch branch)
        {
            try
            {
                var model = new GitRefUpdate { IsLocked = false };
                await _gitClient.UpdateRefAsync(model, repositoryId: repositoryId, filter: branch.Name.ToBranchHeadName());
            }
            catch (VssServiceException ex)
            {
                _logger.LogWarning($"Failed to unlock branch because: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Branch>> GetBranches(Repository repository)
            => (await GetRepoBranches(repository))
                .Select(b => b.AsBranch(repository));

        public async Task<Branch> GetBranch(Repository repository, string branchName)
        {
            try
            {
                var adoBranch = await _gitClient.GetBranchAsync(_adoOptions.ProjectName, repository.Id, branchName);
                return adoBranch.AsBranch(repository);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get branch: {branchName}");
                return null;
            }
        }

        public async Task<IEnumerable<Repository>> GetAllRepositories()
        {
            var repositories = await _gitClient.GetRepositoriesAsync(_adoOptions.ProjectName);
            return repositories.Select(r => r.AsRepository());
        }

        public async Task CreatePullRequest(string sourceBranch, string targetBranch, Guid repositoryId)
        {
            var pullRequest = new GitPullRequest
            {
                SourceRefName = sourceBranch.ToBranchRefName(),
                TargetRefName = targetBranch.ToBranchRefName(),
                Title = $"Merge {sourceBranch} into {targetBranch}"
            };

            await _gitClient.CreatePullRequestAsync(pullRequest, repositoryId);
        }

        public async Task<IEnumerable<Branch>> TargetBranches(Repository repository)
        {
            var branches = await GetRepoBranches(repository);
            return GetTargetBranches(branches).Select(b => b.AsBranch(repository));
        }

        private async Task<IEnumerable<GitBranchStats>> GetRepoBranches(Repository repository)
        {
            try
            {
                return await _gitClient.GetBranchesAsync(repository.Id);
            }
            catch (VssServiceResponseException ex)
            {
                _logger.LogWarning($"Failed to {nameof(TargetBranches)} for repository: {repository.Name} because: {ex.Message}");
                return new List<GitBranchStats>();
            }
        }

        private async Task<GitCommit> GetMergeBasesAsync(Repository repository, string projectName, string targetCommitId, string sourceCommitId)
        {
            var bases = await _gitClient.GetMergeBasesAsync(projectName, repository.Name, sourceCommitId, targetCommitId);
            var firstBasesCommit = await _gitClient.GetCommitAsync(projectName, bases.FirstOrDefault().CommitId, repository.Id);
            return firstBasesCommit;
        }

        private IEnumerable<GitBranchStats> GetBranchesToCompare(GitBranchStats target, IEnumerable<GitBranchStats> branches)
            => GetTargetBranches(branches).Where(b => !b.NameStartsWith(target.Name));

        private IEnumerable<GitBranchStats> GetTargetBranches(IEnumerable<GitBranchStats> branches)
        {
            var targetBranches = new List<GitBranchStats>();
            var directoryTargetBranches = _adoOptions.TargetBranches.Where(b => b.EndsWith("/"));
            targetBranches.AddRange(branches.Where(n => _adoOptions.TargetBranches.Except(directoryTargetBranches).Contains(n.Name)));
            targetBranches.AddRange(branches.Where(n => directoryTargetBranches.Any(g => n.NameStartsWith(g))));

            return targetBranches.Where(b => b != null);
        }
    }
}