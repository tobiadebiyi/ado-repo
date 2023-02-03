using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADO.Repos.Models;

namespace ADO.Repos.Application
{
    public interface IAdoExternalService
    {
        Task<IEnumerable<Branch>> GetBranches(Repository repository);

        Task<IEnumerable<Branch>> GetBranchesAheadOf(string targetBranchName, Repository repository);

        Task<Repository> GetRepository(RepoConfig repositoryName);

        Task<IEnumerable<Branch>> StaleBranches(Repository repository);

        Task CreatePullRequest(string sourceBranch, string targetBranch, Guid repositoryId);

        Task<IEnumerable<Branch>> TargetBranches(Repository repository);

        Task<bool> SourceIsAheadOfTarget(Repository repository, Branch sourceBranch, Branch targetBranch);

        Task<Branch> GetBranch(Repository repository, string branchName);

        Task<IEnumerable<Repository>> GetAllRepositories();

        Task<bool> BranchIsLocked(Repository repository, Branch branch);

        Task LockBranch(Guid repositoryId, Branch branch);

        Task UnlockBranch(Guid repositoryId, Branch branch);
    }
}
