using System;
using System.Collections.Generic;
using System.Linq;
using ADO.Repos.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace ADO.Repos.ExternalServices
{
    public static class RepoExtensions
    {
        public static Branch AsBranch(this GitBranchStats model, Repository repository, bool isAheadOfDev = false)
        {
            var committer = model.Commit.Committer;
            return new Branch(model.Name, committer.Date, committer.Name, model.Commit.Comment, model.Commit.Url, repository.Name, model.Commit.CommitId, isAheadOfDev);
        }

        public static Repository AsRepository(this GitRepository model)
            => new(model.Id, model.Name, new Uri(model.Url));

        public static bool IsBranch(this GitBranchStats model, string branchName)
        => string.Equals(model.Name, branchName, StringComparison.InvariantCultureIgnoreCase);

        public static bool NameStartsWith(this GitBranchStats model, string prefix)
        => model.Name.StartsWith(prefix);

        public static bool NameIs(this GitBranchStats model, string prefix)
        => string.Equals(model.Name, prefix, StringComparison.InvariantCultureIgnoreCase);

        public static GitBranchStats Find(this IEnumerable<GitBranchStats> branches, string branchName)
            => branches.SingleOrDefault(b => b.IsBranch(branchName));

        public static string ToBranchRefName(this string branchName)
            => $"refs/heads/{branchName}";

        public static string ToBranchHeadName(this string branchName)
            => $"heads/{branchName}";
    }
}