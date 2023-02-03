using System;

namespace ADO.Repos.Models
{
    public record Branch(
        string Name,
        DateTime LatestCommitDateTime,
        string CommiterName,
        string CommitMessage,
        string CommitUrl,
        string RepositoryName,
        string CommitId,
        bool IsAheadOfDev = false,
        bool IsLocked = false);
}