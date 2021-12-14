using System.Collections.Generic;
using System.Threading.Tasks;
using ADO.Repos.Models;

namespace ADO.Repos.Application
{
  public interface IRepositoryService
  {
    Task<IEnumerable<Repository>> GetAll();
  }
}
