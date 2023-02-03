using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADO.Repos.Models;
using Microsoft.Extensions.Options;

namespace ADO.Repos.Application
{
    public class ReleaseService : IReleaseService
    {
        private readonly AdoConfig _adoOptions;
        private readonly IAdoExternalService _adoExternalService;

        public ReleaseService(IOptions<AdoConfig> adoOptions, IAdoExternalService adoExternalService)
        {
            _adoOptions = adoOptions?.Value ?? throw new ArgumentNullException(nameof(adoOptions));
            _adoExternalService = adoExternalService;
        }

        public async Task<Release> Get(ReleaseId releaseId)
        {
            var release = new Release();

            foreach (var repoName in _adoOptions.Repositories)
            {
                var repository = await _adoExternalService.GetRepository(repoName);
                var releaseBranch = await _adoExternalService.GetBranch(repository, releaseId.GetId());

                if (releaseBranch != null)
                {
                    var devBranch = await _adoExternalService.GetBranch(repository, BranchNames.Dev);
                    var isAheadOfDev = devBranch != null && await _adoExternalService.SourceIsAheadOfTarget(repository, releaseBranch, devBranch);
                    var isLocked = await _adoExternalService.BranchIsLocked(repository, releaseBranch);

                    release.AddInScope(releaseBranch.AsNewBranch(isAheadOfDev, isLocked));
                }
                else
                {
                    release.AddOutOfScope(repository);
                }
            }

            return release;
        }

        public async Task Release(Guid repositoryId, string branchName)
        {
            var targetBranches = new List<string> { "temp/demo-dev" };

            foreach (var targetBranch in targetBranches)
            {
                await _adoExternalService.CreatePullRequest(branchName, targetBranch, repositoryId);
            }
        }

        public async Task Lock(ReleaseId releaseId)
        {
            var release = await Get(releaseId);
            foreach (var branch in release.InScope)
            {
                await _adoExternalService.LockBranch(_adoOptions.Repositories.Single(r => r.Name == branch.RepositoryName).Id, branch);
            }
        }

        public async Task Unlock(ReleaseId releaseId)
        {
            var release = await Get(releaseId);
            foreach (var branch in release.InScope)
            {
                await _adoExternalService.UnlockBranch(_adoOptions.Repositories.Single(r => r.Name == branch.RepositoryName).Id, branch);
            }
        }
    }
}
