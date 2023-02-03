using System;

namespace ADO.Repos.Models
{
    public record StaleBranch(string Name, DateTime LastCommitDate);
}
