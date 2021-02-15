using System;
using Structurizr.Core.Util;
using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class DeploymentNodeTests : AbstractTestBase
    {
    
        [Fact]
        public void Test_CanonicalName_WhenTheDeploymentNodeHasNoParent()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            deploymentNode.Name = "Ubuntu Server";

            Assert.Equal("DeploymentNode://Default/Ubuntu Server", deploymentNode.CanonicalName);
        }

        [Fact]
        public void Test_CanonicalName_WhenTheDeploymentNodeHasAParent()
        {
            DeploymentNode parent = new DeploymentNode();
            parent.Name = "Ubuntu Server";

            DeploymentNode child = new DeploymentNode();
            child.Name = "Apache Tomcat";
            child.Parent = parent;

            Assert.Equal("DeploymentNode://Default/Ubuntu Server/Apache Tomcat", child.CanonicalName);
        }

        [Fact]
        public void Test_Parent_ReturnsTheParentDeploymentNode()
        {
            DeploymentNode parent = new DeploymentNode();
            Assert.Null(parent.Parent);

            DeploymentNode child = new DeploymentNode();
            child.Parent = parent;
            Assert.Same(parent, child.Parent);
        }

        [Fact]
        public void Test_RequiredTags()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            Assert.Equal(2, deploymentNode.GetRequiredTags().Count);
        }

        [Fact]
        public void Test_Tags()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            Assert.Equal("Element,Deployment Node", deploymentNode.Tags);
        }

        
        [Fact]
        public void Test_RemoveTags_DoesNotRemoveRequiredTags()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            Assert.True(deploymentNode.Tags.Contains(Tags.Element));
            Assert.True(deploymentNode.Tags.Contains(Tags.DeploymentNode));

            deploymentNode.RemoveTag(Tags.DeploymentNode);
            deploymentNode.RemoveTag(Tags.Element);

            Assert.True(deploymentNode.Tags.Contains(Tags.Element));
            Assert.True(deploymentNode.Tags.Contains(Tags.DeploymentNode));
        }
        
        [Fact]
        public void Test_Add_ThrowsAnException_WhenAContainerIsNotSpecified()
        {
            try
            {
                DeploymentNode deploymentNode = new DeploymentNode();
                deploymentNode.Add((Container)null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("A container must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_Add_AddsAContainerInstance_WhenAContainerIsSpecified()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "", "");
            DeploymentNode deploymentNode = Model.AddDeploymentNode("Deployment Node", "", "");
            ContainerInstance containerInstance = deploymentNode.Add(container);

            Assert.NotNull(containerInstance);
            Assert.Same(container, containerInstance.Container);
            Assert.True(deploymentNode.ContainerInstances.Contains(containerInstance));
        }

        [Fact]
        public void Test_AddDeploymentNode_ThrowsAnException_WhenANameIsNotSpecified()
        {
            try
            {
                DeploymentNode parent = Model.AddDeploymentNode("Parent", "", "");
                parent.AddDeploymentNode(null, "", "");
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("A name must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_AddDeploymentNode_AddsAChildDeploymentNode_WhenANameIsSpecified()
        {
            DeploymentNode parent = Model.AddDeploymentNode("Parent", "", "");

            DeploymentNode child = parent.AddDeploymentNode("Child 1", "Description", "Technology");
            Assert.NotNull(child);
            Assert.Equal("Child 1", child.Name);
            Assert.Equal("Description", child.Description);
            Assert.Equal("Technology", child.Technology);
            Assert.Equal(1, child.Instances);
            Assert.Equal(0, child.Properties.Count);
            Assert.True(parent.Children.Contains(child));

            child = parent.AddDeploymentNode("Child 2", "Description", "Technology", 4);
            Assert.NotNull(child);
            Assert.Equal("Child 2", child.Name);
            Assert.Equal("Description", child.Description);
            Assert.Equal("Technology", child.Technology);
            Assert.Equal(4, child.Instances);
            Assert.Equal(0, child.Properties.Count);
            Assert.True(parent.Children.Contains(child));

            child = parent.AddDeploymentNode("Child 3", "Description", "Technology", 4, DictionaryUtils.Create("name=value"));
            Assert.NotNull(child);
            Assert.Equal("Child 3", child.Name);
            Assert.Equal("Description", child.Description);
            Assert.Equal("Technology", child.Technology);
            Assert.Equal(4, child.Instances);
            Assert.Equal(1, child.Properties.Count);
            Assert.Equal("value", child.Properties["name"]);
            Assert.True(parent.Children.Contains(child));
        }

        [Fact]
        public void Test_Uses_ThrowsAnException_WhenANullDestinationIsSpecified()
        {
            try
            {
                DeploymentNode deploymentNode = Model.AddDeploymentNode("Deployment Node", "", "");
                deploymentNode.Uses(null, "", "");
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The destination must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_Uses_AddsARelationship()
        {
            DeploymentNode primaryNode = Model.AddDeploymentNode("MySQL - Primary", "", "");
            DeploymentNode secondaryNode = Model.AddDeploymentNode("MySQL - Secondary", "", "");
            Relationship relationship = primaryNode.Uses(secondaryNode, "Replicates data to", "Some technology");

            Assert.NotNull(relationship);
            Assert.Same(primaryNode, relationship.Source);
            Assert.Same(secondaryNode, relationship.Destination);
            Assert.Equal("Replicates data to", relationship.Description);
            Assert.Equal("Some technology", relationship.Technology);
        }

        [Fact]
        public void Test_GetDeploymentNodeWithName_ThrowsAnException_WhenANameIsNotSpecified()
        {
            try
            {
                DeploymentNode deploymentNode = new DeploymentNode();
                deploymentNode.GetDeploymentNodeWithName(null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("A name must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_GetDeploymentNodeWithName_ReturnsNull_WhenThereIsNoDeploymentNodeWithTheSpecifiedName()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            Assert.Null(deploymentNode.GetDeploymentNodeWithName("foo"));
        }

        [Fact]
        public void Test_GetDeploymentNodeWithName_ReturnsTheNamedDeploymentNode_WhenThereIsADeploymentNodeWithTheSpecifiedName()
        {
            DeploymentNode parent = Model.AddDeploymentNode("parent", "", "");
            DeploymentNode child = parent.AddDeploymentNode("child", "", "");
            Assert.Same(child, parent.GetDeploymentNodeWithName("child"));
        }
            
        [Fact]
        public void Test_GetInfrastructureNodeWithName_ReturnsNull_WhenThereIsNoInfrastructureNodeWithTheSpecifiedName()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            Assert.Null(deploymentNode.GetInfrastructureNodeWithName("foo"));
        }

        [Fact]
        public void Test_GetInfrastructureNodeWithName_ReturnsTheNamedDeploymentNode_WhenThereIsAInfrastructureNodeWithTheSpecifiedName()
        {
            DeploymentNode parent = Model.AddDeploymentNode("parent", "", "");
            InfrastructureNode child = parent.AddInfrastructureNode("child", "", "");
            Assert.Same(child, parent.GetInfrastructureNodeWithName("child"));
        }

        [Fact]
        public void Test_Instances()
        {
            DeploymentNode deploymentNode = new DeploymentNode();
            deploymentNode.Instances = 8;

            Assert.Equal(8, deploymentNode.Instances);
        }

        [Fact]
        public void Test_Instances_ThrowsAnException_WhenZeroIsSpecified()
        {
            try
            {
                DeploymentNode deploymentNode = new DeploymentNode();
                deploymentNode.Instances = 0;
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("Number of instances must be a positive integer.", ae.Message);
            }
        }

        [Fact]
        public void Test_Instances_ThrowsAnException_WhenANegativeNumberIsSpecified()
        {
            try
            {
                DeploymentNode deploymentNode = new DeploymentNode();
                deploymentNode.Instances = -1;
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("Number of instances must be a positive integer.", ae.Message);
            }
        }

    }

}