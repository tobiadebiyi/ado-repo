using System;

namespace ADO.Repos.Models
{
  public record Branch(string name, DateTime latestCommit, string commiterName, string commitMessage);
}