using System;
using System.Collections.Generic;
using System.Linq;

namespace ADO.Repos.Models
{
    public class Repository
    {
        public Repository(Guid id, string name, Uri url)
        {
            Id = id;
            Name = name;
            Url = url;
        }

        public Guid Id { get; }

        public Uri Url { get; }

        public string Name { get; }

        public IEnumerable<Branch> TargetBranches { get; set; } = new List<Branch>();

        public IEnumerable<Branch> StaleBranches { get; set; } = new List<Branch>();

        public IEnumerable<Branch> BranchesAheadOfDev { get; set; } = new List<Branch>();

        public bool MainIsAheadOfDev
            => BranchesAheadOfDev.Any(b => string.Equals(b.Name, BranchNames.Main, StringComparison.InvariantCultureIgnoreCase));

        public bool MatchesFilter(Filters filters)
        {
            if (filters.HasBranchesAheadOfDev && !BranchesAheadOfDev.Any())
                return false;

            if (filters.HasStaleBranches && !StaleBranches.Any())
                return false;

            if (filters.MainIsAheadOfDev && !MainIsAheadOfDev)
                return false;

            return true;
        }
    }
}
