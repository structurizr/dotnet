using Structurizr.Encryption;
using Structurizr.IO.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Structurizr.Api
{

    /// <summary>
    /// A client for the Structurizr API (https://api.structurizr.com)
    /// that allows you to get and put Structurizr workspaces in a JSON format.
    /// </summary>
    public class StructurizrClient
    {

        private string _version;
        private const string WorkspacePath = "/workspace/";

        private string _url;

        public string Url
        {
            get { return _url; }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    throw new ArgumentException("The API URL must not be null or empty.");
                }

                if (value.EndsWith("/"))
                {
                    _url = value.Substring(0, value.Length - 1);
                }
                else
                {
                    _url = value;
                }
            }
        }

        private string _apiKey;

        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    throw new ArgumentException("The API key must not be null or empty.");
                }

                _apiKey = value;
            }
        }

        private string _apiSecret;

        public string ApiSecret
        {
            get { return _apiSecret; }
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    throw new ArgumentException("The API secret must not be null or empty.");
                }

                _apiSecret = value;
            }
        }

        /// <summary>the location where a copy of the workspace will be archived when it is retrieved from the server</summary>
        public DirectoryInfo WorkspaceArchiveLocation { get; set; }

        /// <summary>
        /// The ID generator to use when parsing a JSON workspace definition.
        /// </summary>
        public IdGenerator IdGenerator;
        
        public EncryptionStrategy EncryptionStrategy { get; set; }

        public bool MergeFromRemote { get; set; }

        /// <summary>
        /// Creates a new Structurizr API client with the specified API key and secret,
        /// for the default API URL(https://api.structurizr.com).
        /// </summary>
        /// <param name="apiKey">The API key of your workspace.</param>
        /// <param name="apiSecret">The API secret of your workspace.</param>
        public StructurizrClient(string apiKey, string apiSecret) : this("https://api.structurizr.com", apiKey, apiSecret)
        {
        }

        /// <summary>
        /// Creates a new Structurizr client with the specified API URL, key and secret.
        /// </summary>
        /// <param name="apiUrl">The URL of your Structurizr instance.</param>
        /// <param name="apiKey">The API key of your workspace.</param>
        /// <param name="apiSecret">The API secret of your workspace.</param>
        public StructurizrClient(string apiUrl, string apiKey, string apiSecret)
        {
            Url = apiUrl;
            ApiKey = apiKey;
            ApiSecret = apiSecret;

            WorkspaceArchiveLocation = new DirectoryInfo(".");
            MergeFromRemote = true;
            
            _version = typeof(StructurizrClient).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; 
        }

        /// <summary>
        /// Locks the specified workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <returns>true if the workspace could be locked, false otherwise.</returns>
        public bool LockWorkspace(long workspaceId)
        {
            return manageLockForWorkspace(workspaceId, true);
        }

        /// <summary>
        /// Unlocks the specified workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <returns>true if the workspace could be unlocked, false otherwise.</returns>
        public bool UnlockWorkspace(long workspaceId)
        {
            return manageLockForWorkspace(workspaceId, false);
        }

        private bool manageLockForWorkspace(long workspaceId, bool toBeLocked)
        {
            if (workspaceId <= 0)
            {
                throw new ArgumentException("The workspace ID must be a positive integer.");
            }

            using (HttpClient httpClient = createHttpClient())
            {
                try
                {
                    string httpMethod = toBeLocked ? "PUT" : "DELETE";
                    string path = WorkspacePath + workspaceId + "/lock?user=" + getUser() + "&agent=" + getAgentName();
                    AddHeaders(httpClient, httpMethod, path, "", "");

                    Task<HttpResponseMessage> response;

                    if (toBeLocked)
                    {
                        HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
                        response = httpClient.PutAsync(Url + path, content);
                    }
                    else
                    {
                        response = httpClient.DeleteAsync(Url + path);
                    }

                    string json = response.Result.Content.ReadAsStringAsync().Result;
                    System.Console.WriteLine(json);
                    ApiResponse apiResponse = ApiResponse.Parse(json);

                    if (response.Result.StatusCode == HttpStatusCode.OK)
                    {
                        return apiResponse.Success;
                    }
                    else
                    {
                        throw new StructurizrClientException(apiResponse.Message);
                    }
                }
                catch (Exception e)
                {
                    throw new StructurizrClientException("There was an error putting the workspace: " + e.Message, e);
                }
            }
        }

        /// <summary>
        /// Gets the workspace with the given ID.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <returns>A Workspace object.</returns>
        public Workspace GetWorkspace(long workspaceId)
        {
            if (workspaceId <= 0)
            {
                throw new ArgumentException("The workspace ID must be a positive integer.");
            }

            using (HttpClient httpClient = createHttpClient())
            {
                string httpMethod = "GET";
                string path = WorkspacePath + workspaceId;

                AddHeaders(httpClient, httpMethod, new Uri(Url + path).AbsolutePath, "", "");

                var response = httpClient.GetAsync(Url + path);
                if (response.Result.StatusCode != HttpStatusCode.OK)
                {
                    string jsonResponse = response.Result.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = ApiResponse.Parse(jsonResponse);
                    throw new StructurizrClientException(apiResponse.Message);
                }

                string json = response.Result.Content.ReadAsStringAsync().Result;
                ArchiveWorkspace(workspaceId, json);

                if (EncryptionStrategy == null)
                {
                    JsonReader jsonReader = new JsonReader();
                    jsonReader.IdGenerator = IdGenerator;
                    return jsonReader.Read(new StringReader(json));
                }
                else
                {
                    EncryptedWorkspace encryptedWorkspace = new EncryptedJsonReader().Read(new StringReader(json));
                    if (encryptedWorkspace.EncryptionStrategy != null)
                    {
                        encryptedWorkspace.EncryptionStrategy.Passphrase = this.EncryptionStrategy.Passphrase;
                        return encryptedWorkspace.Workspace;
                    }
                    else
                    {
                        // this workspace isn't encrypted, even though the client has an encryption strategy set
                        JsonReader jsonReader = new JsonReader();
                        jsonReader.IdGenerator = IdGenerator;
                        return jsonReader.Read(new StringReader(json));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the given workspace.
        /// </summary>
        /// <param name="workspaceId">The workspace ID.</param>
        /// <param name="workspace">The workspace to be updated.</param>
        public void PutWorkspace(long workspaceId, Workspace workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentException("The workspace must not be null.");
            }
            else if (workspaceId <= 0)
            {
                throw new ArgumentException("The workspace ID must be a positive integer.");
            }

            if (MergeFromRemote)
            {
                Workspace remoteWorkspace = GetWorkspace(workspaceId);
                if (remoteWorkspace != null)
                {
                    workspace.Views.CopyLayoutInformationFrom(remoteWorkspace.Views);
                    workspace.Views.Configuration.CopyConfigurationFrom(remoteWorkspace.Views.Configuration);
                }
            }

            workspace.Id = workspaceId;
            workspace.LastModifiedDate = DateTime.UtcNow;
            workspace.LastModifiedAgent = getAgentName();
            workspace.LastModifiedUser = getUser();

            using (HttpClient httpClient = createHttpClient())
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

                    AddHeaders(httpClient, httpMethod, new Uri(Url + path).AbsolutePath, workspaceAsJson, "application/json; charset=UTF-8");

                    HttpContent content = new StringContent(workspaceAsJson, Encoding.UTF8, "application/json");
                    content.Headers.ContentType.CharSet = "UTF-8";
                    string contentMd5 = new Md5Digest().Generate(workspaceAsJson);
                    string contentMd5Base64Encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(contentMd5));
                    content.Headers.ContentMD5 = Encoding.UTF8.GetBytes(contentMd5);

                    var response = httpClient.PutAsync(this.Url + path, content);
                    string responseContent = response.Result.Content.ReadAsStringAsync().Result;
                    System.Console.WriteLine(responseContent);

                    if (response.Result.StatusCode != HttpStatusCode.OK)
                    {
                        ApiResponse apiResponse = ApiResponse.Parse(responseContent);
                        throw new StructurizrClientException(apiResponse.Message);
                    }
                }
                catch (Exception e)
                {
                    throw new StructurizrClientException("There was an error putting the workspace: " + e.Message, e);
                }
            }
        }

        protected virtual HttpClient createHttpClient()
        {
            return new HttpClient();
        }

        private void AddHeaders(HttpClient httpClient, string httpMethod, string path, string content, string contentType)
        {
            string contentMd5 = new Md5Digest().Generate(content);
            string nonce = "" + getCurrentTimeInMilliseconds();

            HashBasedMessageAuthenticationCode hmac = new HashBasedMessageAuthenticationCode(ApiSecret);
            HmacContent hmacContent = new HmacContent(httpMethod, path, contentMd5, contentType, nonce);
            string authorizationHeader = new HmacAuthorizationHeader(ApiKey, hmac.Generate(hmacContent.ToString())).ToString();

            httpClient.DefaultRequestHeaders.Add(HttpHeaders.XAuthorization, authorizationHeader);
            httpClient.DefaultRequestHeaders.Add(HttpHeaders.UserAgent, getAgentName());
            httpClient.DefaultRequestHeaders.Add(HttpHeaders.Nonce, nonce);
        }

        private string getAgentName()
        {
            return "structurizr-dotnet/" + _version;
        }

        private String getUser()
        {
            return Environment.UserName ?? Environment.GetEnvironmentVariable("USERNAME") ?? Environment.GetEnvironmentVariable("USER");
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
