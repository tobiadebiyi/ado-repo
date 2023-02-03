using ADO.Repos.Models;

namespace ADO.Repos.Application
{
    public static class ReleaseExtensions
    {
        public static Branch AsNewBranch(this Branch branch, bool isAheadOfDev = false, bool isLocked = false)
        {
            return new Branch(branch.Name, branch.LatestCommitDateTime, branch.CommiterName, branch.CommitMessage, branch.CommitUrl, branch.RepositoryName, branch.CommitId, isAheadOfDev, isLocked);
        }
    }
}