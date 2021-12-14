using System.Collections.Generic;
using System.Threading.Tasks;
using ADO.Repos.Application;
using ADO.Repos.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class RepositoriesController
{
  private readonly IRepositoryService _reposiroryService;

  public RepositoriesController(IRepositoryService reposiroryService)
  {
    _reposiroryService = reposiroryService;
  }
  [HttpGet]
  public async  Task<IEnumerable<Repository>> Get()
  {
    return await _reposiroryService.GetAll();
  }
}