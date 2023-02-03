using System.Collections.Generic;

namespace ADO.Repos.Models
{
    public class GetRepositoryPayload
    {
        public Repository Repository { get; set; }
        public IReadOnlyCollection<Branch> Branches { get; set; }
    }
}