using System.Collections.Generic;

namespace ADO.Repos.Models
{
    public class AdoConfig
    {
        public string BaseUrl { get; set; }

        public string ProjectName { get; set; }

        public IReadOnlyCollection<RepoConfig> Repositories { get; set; }

        public string Pat { get; set; }

        public int StaleBranchThreshholdInDays { get; set; } = 14;

        public Filters DefaultFilters { get; set; }

        public IReadOnlyCollection<string> TargetBranches { get; set; }

        public object BaseRepositoryUrl { get; set; }
    }
}