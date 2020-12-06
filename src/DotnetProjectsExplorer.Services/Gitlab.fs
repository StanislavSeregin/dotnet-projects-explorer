module Gitlab

open GitLabApiClient;

[<Struct>]
type Repository =
    { Id: int;
      Name: string;
      Description: string;
      DefaultBranch: string;
      ProjectUrl: string; }

let repositories (client: IGitLabClient) = async {
    let! projects = client.Projects.GetAsync () |> Async.AwaitTask
    return projects |> Seq.map (fun project ->
    { Id = project.Id;
      Name = project.Name;
      Description = project.Description;
      DefaultBranch = project.DefaultBranch;
      ProjectUrl = project.WebUrl; })
}

let rootFileNames (client: IGitLabClient) projectId = async {
    let! trees = client.Trees.GetAsync projectId |> Async.AwaitTask
    return trees |> Seq.map (fun tree -> tree.Name)
}

let fileContent (client: IGitLabClient) projectId filePath defaultBranch = async {
    let! solutionFile = client.Files.GetAsync (projectId, filePath, defaultBranch) |> Async.AwaitTask
    return solutionFile.ContentDecoded
}
