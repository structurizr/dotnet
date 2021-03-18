using System;
using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class SoftwareSystemInstanceTests : AbstractTestBase
    {

        private SoftwareSystem _softwareSystem;
        private DeploymentNode _deploymentNode;

        public SoftwareSystemInstanceTests()
        {
            _softwareSystem = Model.AddSoftwareSystem(Location.External, "System", "Description");
            _deploymentNode = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
        }
        
        [Fact]
        public void Test_construction()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.Same(_softwareSystem, softwareSystemInstance.SoftwareSystem);
            Assert.Equal(_softwareSystem.Id, softwareSystemInstance.SoftwareSystemId);
            Assert.Equal(1, softwareSystemInstance.InstanceId);
        }

        [Fact]
        public void test_SoftwareSystemId()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.Equal(_softwareSystem.Id, softwareSystemInstance.SoftwareSystemId);
            softwareSystemInstance.SoftwareSystem = null;
            softwareSystemInstance.SoftwareSystemId = "1234";
            Assert.Equal("1234", softwareSystemInstance.SoftwareSystemId);
        }

        [Fact]
        public void test_Name()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.Equal(_softwareSystem.Name, softwareSystemInstance.Name);
    
            softwareSystemInstance.Name = "foo";
            Assert.Equal(_softwareSystem.Name, softwareSystemInstance.Name);
        }
    
        [Fact]
        public void test_CanonicalName()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.Equal("SoftwareSystemInstance://Default/Deployment Node/System[1]", softwareSystemInstance.CanonicalName);
        }
    
        [Fact]
        public void test_Parent_ReturnsTheParentDeploymentNode()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.Equal(_deploymentNode, softwareSystemInstance.Parent);
        }
    
        [Fact]
        public void test_RequiredTags()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.Equal(0, softwareSystemInstance.GetRequiredTags().Count);
        }
    
        [Fact]
        public void test_Tags()
        {
            _softwareSystem.AddTags("Tag 1");
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
            softwareSystemInstance.AddTags("Primary Instance");
    
            Assert.Equal("Software System Instance,Primary Instance", softwareSystemInstance.Tags);
        }
    
        [Fact]
        public void test_RemoveTags_DoesNotRemoveRequiredTags()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
    
            Assert.True(softwareSystemInstance.Tags.Contains(Tags.SoftwareSystemInstance));
    
            softwareSystemInstance.RemoveTag(Tags.SoftwareSystemInstance);
    
            Assert.True(softwareSystemInstance.Tags.Contains(Tags.SoftwareSystemInstance));
        }
    
        [Fact]
        public void test_AddHealthCheck()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");
            Assert.Equal(0, softwareSystemInstance.HealthChecks.Count);

            HttpHealthCheck healthCheck = softwareSystemInstance.AddHealthCheck("Test web application is working", "http://localhost:8080");
            Assert.Equal("Test web application is working", healthCheck.Name);
            Assert.Equal("http://localhost:8080", healthCheck.Url);
            Assert.Equal(60, healthCheck.Interval);
            Assert.Equal(0, healthCheck.Timeout);
            Assert.Equal(1, softwareSystemInstance.HealthChecks.Count);
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheNameIsNull()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck(null, "http://localhost");
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The name must not be null or empty.", ae.Message);
            }
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheNameIsEmpty()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck(" ", "http://localhost");
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The name must not be null or empty.", ae.Message);
            }
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheUrlIsNull()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck("Name", null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The URL must not be null or empty.", ae.Message);
            }
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheUrlIsEmpty()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck("Name", " ");
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The URL must not be null or empty.", ae.Message);
            }
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheUrlIsInvalid()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck("Name", "localhost");
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("localhost is not a valid URL.", ae.Message);
            }
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheIntervalIsLessThanZero()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck("Name", "https://localhost", -1, 0);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The polling interval must be zero or a positive integer.", ae.Message);
            }
        }

        [Fact]
        public void test_AddHealthCheck_ThrowsAnException_WhenTheTimeoutIsLessThanZero()
        {
            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(_deploymentNode, _softwareSystem, "Default");

            try
            {
                softwareSystemInstance.AddHealthCheck("Name", "https://localhost", 60, -1);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The timeout must be zero or a positive integer.", ae.Message);
            }
        }

    }

}