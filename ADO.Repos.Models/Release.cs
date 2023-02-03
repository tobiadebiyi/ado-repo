using System.Collections.Generic;
using KLP.Core.Extensions;

namespace ADO.Repos.Models
{
    public class Release
    {
        private readonly List<Branch> _inScope = new();
        private readonly List<Repository> _outOfScope = new();

        public void AddInScope(Branch branch)
        {
            branch.Guard(nameof(branch));
            _inScope.Add(branch);
        }

        public void AddOutOfScope(Repository repository)
        {
            repository.Guard(nameof(repository));
            _outOfScope.Add(repository);
        }

        public IReadOnlyCollection<Branch> InScope => _inScope;

        public IReadOnlyCollection<Repository> OutOfScope => _outOfScope;
    }
}