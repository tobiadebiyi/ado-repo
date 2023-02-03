using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADO.Repos.Models;

namespace ADO.Repos.Application
{
    public interface IRepositoryService
    {
        Task<Dictionary<string, Repository>> GetAll(Filters filters);

        Task<Repository> Get(Guid repositoryId);

        Task<IEnumerable<Repository>> GetAll();
    }
}
