﻿using Structurizr.Api;
using System.Net.Http;

namespace Structurizr.Client.Tests.Api
{
    internal class TestStructurizrClient : StructurizrClient
    {
        public TestStructurizrClient(string apiKey, string apiSecret)
            : base(apiKey, apiSecret)
        {
        }

        public TestStructurizrClient(string apiUrl, string apiKey, string apiSecret)
            : base(apiUrl, apiKey, apiSecret)
        {
        }

        public TestStructurizrClient(string apiUrl, string apiKey, string apiSecret, HttpClient httpClient)
            : base(apiUrl, apiKey, apiSecret, httpClient)
        {
        }

        public HttpClient GetClient() => base.createHttpClient();
    }
}
