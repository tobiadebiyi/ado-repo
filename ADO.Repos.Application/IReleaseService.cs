using System;
using System.Threading.Tasks;
using ADO.Repos.Models;

namespace ADO.Repos.Application
{
    public interface IReleaseService
    {
        Task<Release> Get(ReleaseId releaseId);

        Task Lock(ReleaseId releaseId);

        Task Release(Guid repositoryId, string branchName);

        Task Unlock(ReleaseId releaseId);
    }
}
