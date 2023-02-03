using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADO.Repos.Models;
using Microsoft.Extensions.Options;

namespace ADO.Repos.Application
{
    public class RepositoryService : IRepositoryService
    {
        private readonly AdoConfig _adoOptions;
        private readonly IAdoExternalService _adoExternalService;

        public RepositoryService(IOptions<AdoConfig> adoOptions, IAdoExternalService adoExternalService)
        {
            _adoOptions = adoOptions?.Value ?? throw new ArgumentNullException(nameof(adoOptions));
            this._adoExternalService = adoExternalService;
        }

        public async Task<Dictionary<string, Repository>> GetAll(Filters filters = null)
        {
            var repositories = new Dictionary<string, Repository>();
            filters ??= _adoOptions.DefaultFilters;

            foreach (var repo in _adoOptions.Repositories)
            {
                var repository = await _adoExternalService.GetRepository(repo);

                repository.TargetBranches = await _adoExternalService.TargetBranches(repository);

                if (filters.HasStaleBranches)
                    repository.StaleBranches = await _adoExternalService.StaleBranches(repository);

                if (filters.HasBranchesAheadOfDev || filters.MainIsAheadOfDev)
                    repository.BranchesAheadOfDev = await _adoExternalService.GetBranchesAheadOf(BranchNames.Dev, repository);

                if (repository.MatchesFilter(filters))
                    repositories.Add(repo.Name, repository);
            }

            return repositories;
        }

        public async Task<Repository> Get(Guid repositoryId)
        {
            var repoConfig = _adoOptions.Repositories.FirstOrDefault(r => r.Id == repositoryId);
            var repository = await _adoExternalService.GetRepository(repoConfig);
            return repository;
        }

        public Task<IEnumerable<Repository>> GetAll()
         => _adoExternalService.GetAllRepositories();
    }
}
