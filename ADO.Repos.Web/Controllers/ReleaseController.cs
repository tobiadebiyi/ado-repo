using System;
using System.Threading.Tasks;
using ADO.Repos.Application;
using ADO.Repos.Models;
using KLP.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADO.Repos.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReleaseController
    {
        private readonly IReleaseService _releaseService;

        public ReleaseController(IReleaseService releaseService)
        {
            _releaseService = releaseService;
        }

        [HttpGet("{releaseName}")]
        public async Task<Result<Release>> GetRelease(string releaseName)
        {
            try
            {
                var release = await _releaseService.Get(new ReleaseId(releaseName));
                return Result<Release>.SuccessResult(release);
            }
            catch (Exception ex)
            {
                return Result<Release>.ErrorResult($"{nameof(GetRelease)} Operation Failed: {ex.Message}");
            }
        }

        [HttpGet("{releaseName}/lock")]
        public async Task<Result> LockRelease(string releaseName)
        {
            try
            {
                await _releaseService.Lock(new ReleaseId(releaseName));
                return Result.SuccessResult();
            }
            catch (Exception ex)
            {
                return Result.ErrorResult($"{nameof(LockRelease)} Operation Failed: {ex.Message}");
            }
        }

        [HttpGet("{releaseName}/unlock")]
        public async Task<Result> UnlockRelease(string releaseName)
        {
            try
            {
                await _releaseService.Unlock(new ReleaseId(releaseName));
                return Result.SuccessResult();
            }
            catch (Exception ex)
            {
                return Result.ErrorResult($"{nameof(UnlockRelease)} Operation Failed: {ex.Message}");
            }
        }

        [HttpPost("{releaseId}/deployed/{repositoryId}")]
        public async Task<Result> Deployed(Guid repositoryId, string releaseId)
        {
            try
            {
                await _releaseService.Release(repositoryId, releaseId);
                return Result.SuccessResult();
            }
            catch (Exception ex)
            {
                return Result.ErrorResult($"{nameof(Deployed)} Operation Failed: {ex.Message}");
            }
        }
    }
}