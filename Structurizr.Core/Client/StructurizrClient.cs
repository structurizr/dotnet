using Structurizr.Encryption;
using Structurizr.IO.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Structurizr.Client
{
    public class StructurizrClient
    {

        private const string WorkspacePath = "/workspace/";

        public string Url { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }

        /// <summary>the location where a copy of the workspace will be archived when it is retrieved from the server</summary>
        public DirectoryInfo WorkspaceArchiveLocation { get; set; }

        public EncryptionStrategy EncryptionStrategy { get; set; }

        public StructurizrClient(string apiKey, string apiSecret) : this("https://api.structurizr.com", apiKey, apiSecret)
        {
        }

        public StructurizrClient(string apiUrl, string apiKey, string apiSecret)
        {
            this.Url = apiUrl;
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;

            this.WorkspaceArchiveLocation = new DirectoryInfo(".");
        }

       public Workspace GetWorkspace(long workspaceId)
       {
          var getTask = GetWorkspaceAsync(workspaceId);
          getTask.Wait();
          return getTask.Result;
       }

       public async Task<Workspace> GetWorkspaceAsync(long workspaceId)
       {
          using (WebClient webClient = new WebClient())
          {
             try
             {
                string httpMethod = "GET";
                string path = WorkspacePath + workspaceId;

                AddHeaders(webClient, httpMethod, path, "", "");

                string response = await webClient.DownloadStringTaskAsync(this.Url + path);
                ArchiveWorkspace(workspaceId, response);

                StringReader stringReader = new StringReader(response);
                if (EncryptionStrategy == null)
                {
                   return new JsonReader().Read(stringReader);
                }
                else {
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

       public void PutWorkspace(long workspaceId, Workspace workspace)
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

                    AddHeaders(webClient, httpMethod, path, workspaceAsJson, "application/json; charset=UTF-8");

                    string response = webClient.UploadString(this.Url + path, httpMethod, workspaceAsJson);
                    System.Console.WriteLine(response);
                }
                catch (Exception e)
                {
                    throw new StructurizrClientException("There was an error putting the workspace: " + e.Message, e);
                }
            }
        }

        public void MergeWorkspace(long workspaceId, Workspace workspace)
        {
            Workspace currentWorkspace = GetWorkspace(workspaceId);
            if (currentWorkspace != null)
            {
                workspace.Views.CopyLayoutInformationFrom(currentWorkspace.Views);
            }
            PutWorkspace(workspaceId, workspace);
        }

        private void AddHeaders(WebClient webClient, string httpMethod, string path, string content, string contentType)
        {
            webClient.Encoding = Encoding.UTF8;
            string contentMd5 = new Md5Digest().Generate(content);
            string contentMd5Base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(contentMd5));
            string nonce = "" + getCurrentTimeInMilliseconds();

            HashBasedMessageAuthenticationCode hmac = new HashBasedMessageAuthenticationCode(ApiSecret);
            HmacContent hmacContent = new HmacContent(httpMethod, path, contentMd5, contentType, nonce);
            string authorizationHeader = new HmacAuthorizationHeader(ApiKey, hmac.Generate(hmacContent.ToString())).ToString();

            webClient.Headers.Add(HttpHeaders.UserAgent, "structurizr-dotnet/" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            webClient.Headers.Add(HttpHeaders.Authorization, authorizationHeader);
            webClient.Headers.Add(HttpHeaders.Nonce, nonce);
            webClient.Headers.Add(HttpHeaders.ContentMd5, contentMd5Base64Encoded);
            webClient.Headers.Add(HttpHeaders.ContentType, contentType);
        }

        private long getCurrentTimeInMilliseconds()
        {
            DateTime Jan1st1970Utc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - Jan1st1970Utc).TotalMilliseconds;
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
