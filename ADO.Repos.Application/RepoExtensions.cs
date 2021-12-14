using System;
using ADO.Repos.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace ADO.Repos.Application
{
    public static class RepoExtensions
    {
        public static Branch AsBranch(this GitBranchStats model)
        {
            var committer = model.Commit.Committer;
            return new Branch(model.Name, committer.Date, committer.Name, model.Commit.Comment);
        }

        public static bool IsBranch(this GitBranchStats model, string branchName)
        => String.Equals(model.Name, branchName, StringComparison.InvariantCultureIgnoreCase);
    }
}