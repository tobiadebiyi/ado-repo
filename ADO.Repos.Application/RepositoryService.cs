using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADO.Repos.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace ADO.Repos.Application
{
  public class RepositoryService : IRepositoryService
  {
    const string collectionUri = "https://dev.azure.com/***";
    const string projectName = "Retro";
    const string pat = "***";
    List<string> repoNames = new List<string>{ "Retro" };

    public async Task<IEnumerable<Repository>> GetAll()
    {
      var creds = new VssBasicCredential(string.Empty, pat);

      var connection = new VssConnection(new Uri(collectionUri), creds);
      using var gitClient = connection.GetClient<GitHttpClient>();

      var repositories = new List<Repository>();
      foreach(var repoName in repoNames)
      {
        var repo = await gitClient.GetRepositoryAsync(projectName, repoName);
        var branches = await gitClient.GetBranchesAsync(projectName, repo.Id);

        var devBranch = branches.SingleOrDefault(b => b.IsBranch("Ejected"));
        var mainBranch = branches.SingleOrDefault(b => b.IsBranch("Master"));
        List<GitCommitRef> mergeMainAndDevBases = null;

        if(devBranch != default && mainBranch != default)
        {
          mergeMainAndDevBases = await gitClient.GetMergeBasesAsync(projectName, repoName, mainBranch.Commit.CommitId, devBranch.Commit.CommitId);
        }

        var repository = new Repository
                { 
                  Name = repo.Name, 
                  Url = repo.RemoteUrl,
                  Main = mainBranch?.AsBranch(),
                  Dev = devBranch?.AsBranch(),
                  Test = branches.SingleOrDefault(b => b.IsBranch(BranchNames.Test))?.AsBranch(),
                  Master = branches.SingleOrDefault(b => b.IsBranch(BranchNames.Master))?.AsBranch(),
                  MainIsAheadOfDev = mergeMainAndDevBases?.FirstOrDefault()?.Committer.Date < mainBranch.Commit.Committer.Date
                };

        repositories.Add(repository);
      }

      return repositories;
    }
  }
}
