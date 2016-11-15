using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Structurizr.Client;
using Structurizr.Encryption;
using Structurizr.IO.Json;

namespace Structurizr.Core.Async
{
    public class AsyncStructurizrClient
    {

        private const string WorkspacePath = "/workspace/";

        public string Url { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        /// <summary>the location where a copy of the workspace will be archived when it is retrieved from the server</summary>
        public DirectoryInfo WorkspaceArchiveLocation { get; set; }

        public EncryptionStrategy EncryptionStrategy { get; set; }

        public AsyncStructurizrClient(string apiKey, string apiSecret)
            : this("https://api.structurizr.com", apiKey, apiSecret)
        {
        }

        public AsyncStructurizrClient(string apiUrl, string apiKey, string apiSecret)
        {
            this.Url = apiUrl;
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;

            this.WorkspaceArchiveLocation = new DirectoryInfo(".");
        }

        public async Task<Workspace> GetWorkspaceAsync(long workspaceId)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string httpMethod = "GET";
                    string path = WorkspacePath + workspaceId;

                    HttpHeadersHelper.AddHeaders(webClient, httpMethod, path, "", "", ApiSecret, ApiKey);

                    string response = await webClient.DownloadStringTaskAsync(this.Url + path);
                    ArchiveWorkspace(workspaceId, response);

                    StringReader stringReader = new StringReader(response);
                    if (EncryptionStrategy == null)
                    {
                        return new JsonReader().Read(stringReader);
                    }
                    else
                    {
                        EncryptedWorkspace encryptedWorkspace = new EncryptedJsonReader().Read(stringReader);
                        encryptedWorkspace.EncryptionStrategy.Passphrase = this.EncryptionStrategy.Passphrase;
                        return encryptedWorkspace.Workspace;
                    }
                }
                catch (Exception e)
                {
                    throw new StructurizrClientException("There was an error getting the workspace: " + e.Message, e);
                }
            }
        }

        public async Task PutWorkspaceAsync(long workspaceId, Workspace workspace)
        {
            workspace.Id = workspaceId;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string httpMethod = "PUT";
                    string path = WorkspacePath + workspaceId;
                    string workspaceAsJson = "";

                    using (StringWriter stringWriter = new StringWriter())
                    {
                        if (EncryptionStrategy == null)
                        {
                            JsonWriter jsonWriter = new JsonWriter(false);
                            jsonWriter.Write(workspace, stringWriter);
                        }
                        else
                        {
                            EncryptedWorkspace encryptedWorkspace = new EncryptedWorkspace(workspace, EncryptionStrategy);
                            EncryptedJsonWriter jsonWriter = new EncryptedJsonWriter(false);
                            jsonWriter.Write(encryptedWorkspace, stringWriter);
                        }
                        stringWriter.Flush();
                        workspaceAsJson = stringWriter.ToString();
                        System.Console.WriteLine(workspaceAsJson);
                    }

                    HttpHeadersHelper.AddHeaders(webClient, httpMethod, path, workspaceAsJson, "application/json; charset=UTF-8", ApiSecret, ApiKey);

                    string response = await webClient.UploadStringTaskAsync(this.Url + path, httpMethod, workspaceAsJson);
                    System.Console.WriteLine(response);
                }
                catch (Exception e)
                {
                    throw new StructurizrClientException("There was an error putting the workspace: " + e.Message, e);
                }
            }
        }

        public async Task MergeWorkspaceAsync(long workspaceId, Workspace workspace)
        {
            Workspace currentWorkspace = await GetWorkspaceAsync(workspaceId);
            var mergedWorkspace = MergeWorkspaces(currentWorkspace, workspace);
            await PutWorkspaceAsync(workspaceId, mergedWorkspace);
        }

        public Workspace MergeWorkspaces(Workspace currentWorkspace, Workspace newWorkspace)
        {
            if (currentWorkspace != null)
            {
                newWorkspace.Views.CopyLayoutInformationFrom(currentWorkspace.Views);
            }
            return newWorkspace;
        }

        private void ArchiveWorkspace(long workspaceId, string workspaceAsJson)
        {
            if (WorkspaceArchiveLocation != null)
            {
                File.WriteAllText(CreateArchiveFileName(workspaceId), workspaceAsJson);
            }
        }

        private string CreateArchiveFileName(long workspaceId)
        {
            return Path.Combine(
                WorkspaceArchiveLocation.FullName,
                "structurizr-" + workspaceId + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".json");
        }
    }
}