namespace ADO.Repos.Models
{
    public class Filters
    {
        public bool HasStaleBranches { get; set; }

        public bool HasBranchesAheadOfDev { get; set; }

        public bool EagerLoadRepositories { get; set; }

        public bool MainIsAheadOfDev { get; set; }
    }
}