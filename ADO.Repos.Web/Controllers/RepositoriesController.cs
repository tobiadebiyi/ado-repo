using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ADO.Repos.Application;
using ADO.Repos.Models;
using KLP.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADO.Repos.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoriesController
    {
        private readonly IRepositoryService _repositoryService;

        public RepositoriesController(IRepositoryService reposiroryService)
        {
            _repositoryService = reposiroryService;
        }

        [HttpGet]
        public async Task<Result<Dictionary<string, Repository>>> GetAll(
            [FromQuery] bool hasBranchesAheadOfDev = false,
            [FromQuery] bool mainIsAheadOfDev = false,
            [FromQuery] bool hasStaleBranches = false
            )
        {
            try
            {
                var filters = new Filters
                {
                    HasBranchesAheadOfDev = hasBranchesAheadOfDev,
                    MainIsAheadOfDev = mainIsAheadOfDev,
                    HasStaleBranches = hasStaleBranches,
                };

                var result = await _repositoryService.GetAll(filters);
                return Result<Dictionary<string, Repository>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                return Result<Dictionary<string, Repository>>
                    .ErrorResult($"Get Repositories Operation Failed: {ex.Message}");
            }
        }

        [HttpGet("{repositoryId}")]
        public async Task<Result<Repository>> Get(Guid repositoryId)
        {
            try
            {
                var repository = await _repositoryService.Get(repositoryId);
                return Result<Repository>.SuccessResult(repository);
            }
            catch (Exception ex)
            {
                return Result<Repository>.ErrorResult($"Get Repo Operation Failed: {ex.Message}");
            }
        }

        [HttpGet("all")]
        public async Task<Result<IEnumerable<Repository>>> GetAll()
        {
            try
            {
                return Result<IEnumerable<Repository>>.SuccessResult(await _repositoryService.GetAll());
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Repository>>.ErrorResult($"Get all repositories failed: {ex.Message}");
            }
        }
    }
}