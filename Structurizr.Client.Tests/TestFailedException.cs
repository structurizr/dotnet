using System;

namespace Structurizr.Api.Tests
{
    public class TestFailedException : Exception
    {

        public TestFailedException()
        {
        }
        
        public TestFailedException(string message) : base(message)
        {
        }
        
    }
}