using System.IO;
using System.Threading.Tasks;
using Structurizr.Client;
using Structurizr.Encryption;

namespace Structurizr.Core.Async
{
    public class AsyncStructurizrClient
    {
        private readonly StructurizrClient _privateClient;

        public string Url
        {
            get { return _privateClient.Url; }
            set { _privateClient.Url = value; }
        }

        public string ApiKey
        {
            get { return _privateClient.ApiKey; }
            set { _privateClient.ApiKey = value; }
        }

        public string ApiSecret
        {
            get { return _privateClient.ApiSecret; }
            set { _privateClient.ApiSecret = value; }
        }

        /// <summary>the location where a copy of the workspace will be archived when it is retrieved from the server</summary>
        public DirectoryInfo WorkspaceArchiveLocation
        {
            get { return _privateClient.WorkspaceArchiveLocation; }
            set { _privateClient.WorkspaceArchiveLocation = value; }
        }

        public EncryptionStrategy EncryptionStrategy
        {
            get { return _privateClient.EncryptionStrategy; }
            set { _privateClient.EncryptionStrategy = value; }
        }

        public AsyncStructurizrClient(string apiKey, string apiSecret)
            : this("https://api.structurizr.com", apiKey, apiSecret)
        {
        }

        public AsyncStructurizrClient(string apiUrl, string apiKey, string apiSecret)
        {
            _privateClient = new StructurizrClient(apiUrl, apiKey, apiSecret);
        }

        public Task<Workspace> GetWorkspaceAsync(long workspaceId)
        {
            return Task.Run(() => _privateClient.GetWorkspace(workspaceId));
        }

        public Task PutWorkspaceAsync(long workspaceId, Workspace workspace)
        {
            return Task.Run(() => _privateClient.PutWorkspace(workspaceId, workspace));
        }

        public Workspace MergeWorkspaces(Workspace currentWorkspace, Workspace newWorkspace)
        {
            if (currentWorkspace != null)
            {
                newWorkspace.Views.CopyLayoutInformationFrom(currentWorkspace.Views);
            }
            return newWorkspace;
        }
    }
}