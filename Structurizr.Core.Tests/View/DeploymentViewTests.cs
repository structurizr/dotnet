using System;
using System.Linq;
using Xunit;

namespace Structurizr.Core.Tests
{

    public class DeploymentViewTests : AbstractTestBase
    {


        private DeploymentView deploymentView;

        [Fact]
        public void Test_Name_WithNoSoftwareSystemAndNoEnvironment()
        {
            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            Assert.Equal("Deployment - Default", deploymentView.Name);
        }

        [Fact]
        public void Test_Name_WithNoSoftwareSystemAndAnEnvironment()
        {
            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.Environment = "Live";
            Assert.Equal("Deployment - Live", deploymentView.Name);
        }

        [Fact]
        public void Test_Name_WithASoftwareSystemAndNoEnvironment()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            Assert.Equal("Software System - Deployment - Default", deploymentView.Name);
        }

        [Fact]
        public void Test_Name_WithASoftwareSystemAndAnEnvironment()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.Environment = "Live";
            Assert.Equal("Software System - Deployment - Live", deploymentView.Name);
        }

        [Fact]
        public void Test_AddDeploymentNode_ThrowsAnException_WhenPassedNull()
        {
            try
            {
                deploymentView = Views.CreateDeploymentView("key", "Description");
                deploymentView.Add((DeploymentNode)null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("A deployment node must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_AddRelationship_ThrowsAnException_WhenPassedNull()
        {
            try
            {
                deploymentView = Views.CreateDeploymentView("key", "Description");
                deploymentView.Add((Relationship)null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("A relationship must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_AddAllDeploymentNodes_DoesNothing_WhenThereAreNoTopLevelDeploymentNodes()
        {
            deploymentView = Views.CreateDeploymentView("deployment", "Description");

            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(0, deploymentView.Elements.Count);
        }

        [Fact]
        public void Test_AddAllDeploymentNodes_DoesNothing_WhenThereAreTopLevelDeploymentNodesButNoContainerInstances()
        {
            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            Model.AddDeploymentNode("Deployment Node", "Description", "Technology");

            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(0, deploymentView.Elements.Count);
        }

        [Fact]
        public void Test_AddAllDeploymentNodes_DoesNothing_WhenThereNoDeploymentNodesForTheDeploymentEnvironment()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNode = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            ContainerInstance containerInstance = deploymentNode.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.Environment = "Live";
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(0, deploymentView.Elements.Count);
        }

        [Fact]
        public void Test_AddAllDeploymentNodes_AddsDeploymentNodesAndContainerInstances_WhenThereAreTopLevelDeploymentNodesWithContainerInstances()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNode = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            ContainerInstance containerInstance = deploymentNode.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(2, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNode)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(containerInstance)));
        }

        [Fact]
        public void Test_AddAllDeploymentNodes_AddsDeploymentNodesAndContainerInstances_WhenThereAreChildDeploymentNodesWithContainerInstances()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(containerInstance)));
        }

        [Fact]
        public void Test_AddAllDeploymentNodes_AddsDeploymentNodesAndContainerInstancesOnlyForTheSoftwareSystemInScope()
        {
            SoftwareSystem softwareSystem1 = Model.AddSoftwareSystem("Software System 1", "");
            Container container1 = softwareSystem1.AddContainer("Container 1", "Description", "Technology");
            DeploymentNode deploymentNode1 = Model.AddDeploymentNode("Deployment Node 1", "Description", "Technology");
            ContainerInstance containerInstance1 = deploymentNode1.Add(container1);

            SoftwareSystem softwareSystem2 = Model.AddSoftwareSystem("Software System 2", "");
            Container container2 = softwareSystem2.AddContainer("Container 2", "Description", "Technology");
            DeploymentNode deploymentNode2 = Model.AddDeploymentNode("Deployment Node 2", "Description", "Technology");
            ContainerInstance containerInstance2 = deploymentNode2.Add(container2);

            // two containers from different software systems on the same deployment node
            deploymentNode1.Add(container2);

            deploymentView = Views.CreateDeploymentView(softwareSystem1, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();

            Assert.Equal(2, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNode1)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(containerInstance1)));
        }

        [Fact]
        public void Test_AddDeploymentNode_AddsTheParentToo()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.Add(deploymentNodeChild);
            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(containerInstance)));
        }

        [Fact]
        public void Test_AddAnimationStep_ThrowsAnException_WhenNoContainerInstancesAreSpecified()
        {
            try
            {
                deploymentView = Views.CreateDeploymentView("deployment", "Description");
                deploymentView.AddAnimation((ContainerInstance[])null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("One or more software system/container instances must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_AddAnimationStep_ThrowsAnException_WhenNoInfrastructureNodesAreSpecified()
        {
            try
            {
                deploymentView = Views.CreateDeploymentView("deployment", "Description");
                deploymentView.AddAnimation((InfrastructureNode[])null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("One or more infrastructure nodes must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_AddAnimationStep_ThrowsAnException_WhenNoContainerInstancesOrInfrastructureNodesAreSpecified()
        {
            try
            {
                deploymentView = Views.CreateDeploymentView("deployment", "Description");
                deploymentView.AddAnimation((ContainerInstance[])null, (InfrastructureNode[])null);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("One or more software system/container instances and/or infrastructure nodes must be specified.", ae.Message);
            }
        }

        [Fact]
        public void Test_AddAnimationStep()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container webApplication = softwareSystem.AddContainer("Web Application", "Description", "Technology");
            Container database = softwareSystem.AddContainer("Database", "Description", "Technology");
            webApplication.Uses(database, "Reads from and writes to", "JDBC/HTTPS");

            DeploymentNode developerLaptop = Model.AddDeploymentNode("Developer Laptop", "Description", "Technology");
            DeploymentNode apacheTomcat = developerLaptop.AddDeploymentNode("Apache Tomcat", "Description", "Technology");
            DeploymentNode oracle = developerLaptop.AddDeploymentNode("Oracle", "Description", "Technology");
            ContainerInstance webApplicationInstance = apacheTomcat.Add(webApplication);
            ContainerInstance databaseInstance = oracle.Add(database);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.Add(developerLaptop);

            deploymentView.AddAnimation(webApplicationInstance);
            deploymentView.AddAnimation(databaseInstance);

            Animation step1 = deploymentView.Animations.First(step => step.Order == 1);
            Assert.Equal(3, step1.Elements.Count);
            Assert.True(step1.Elements.Contains(developerLaptop.Id));
            Assert.True(step1.Elements.Contains(apacheTomcat.Id));
            Assert.True(step1.Elements.Contains(webApplicationInstance.Id));
            Assert.Equal(0, step1.Relationships.Count);

            Animation step2 = deploymentView.Animations.First(step => step.Order == 2);
            Assert.Equal(2, step2.Elements.Count);
            Assert.True(step2.Elements.Contains(oracle.Id));
            Assert.True(step2.Elements.Contains(databaseInstance.Id));
            Assert.Equal(1, step2.Relationships.Count);
            Assert.True(step2.Relationships.Contains(webApplicationInstance.Relationships.First().Id));
        }

        [Fact]
        public void Test_AddAnimationStep_IgnoresContainerInstancesThatDoNotExistInTheView()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container webApplication = softwareSystem.AddContainer("Web Application", "Description", "Technology");
            Container database = softwareSystem.AddContainer("Database", "Description", "Technology");
            webApplication.Uses(database, "Reads from and writes to", "JDBC/HTTPS");

            DeploymentNode developerLaptop = Model.AddDeploymentNode("Developer Laptop", "Description", "Technology");
            DeploymentNode apacheTomcat = developerLaptop.AddDeploymentNode("Apache Tomcat", "Description", "Technology");
            DeploymentNode oracle = developerLaptop.AddDeploymentNode("Oracle", "Description", "Technology");
            ContainerInstance webApplicationInstance = apacheTomcat.Add(webApplication);
            ContainerInstance databaseInstance = oracle.Add(database);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.Add(apacheTomcat);

            deploymentView.AddAnimation(webApplicationInstance, databaseInstance);

            Animation step1 = deploymentView.Animations.First(step => step.Order == 1);
            Assert.Equal(3, step1.Elements.Count);
            Assert.True(step1.Elements.Contains(developerLaptop.Id));
            Assert.True(step1.Elements.Contains(apacheTomcat.Id));
            Assert.True(step1.Elements.Contains(webApplicationInstance.Id));
            Assert.Equal(0, step1.Relationships.Count);
        }

        [Fact]
        public void Test_AddAnimationStep_ThrowsAnException_WhenContainerInstancesAreSpecifiedButNoneOfThemExistInTheView()
        {
            try
            {
                SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
                Container webApplication = softwareSystem.AddContainer("Web Application", "Description", "Technology");
                Container database = softwareSystem.AddContainer("Database", "Description", "Technology");
                webApplication.Uses(database, "Reads from and writes to", "JDBC/HTTPS");

                DeploymentNode developerLaptop = Model.AddDeploymentNode("Developer Laptop", "Description", "Technology");
                DeploymentNode apacheTomcat = developerLaptop.AddDeploymentNode("Apache Tomcat", "Description", "Technology");
                DeploymentNode oracle = developerLaptop.AddDeploymentNode("Oracle", "Description", "Technology");
                ContainerInstance webApplicationInstance = apacheTomcat.Add(webApplication);
                ContainerInstance databaseInstance = oracle.Add(database);

                deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");

                deploymentView.AddAnimation(webApplicationInstance, databaseInstance);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("None of the specified container instances exist in this view.", ae.Message);
            }
        }
        
        [Fact]
        public void Test_Add_AddsTheInfrastructureNode()
        {
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode1 = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node 1");
            InfrastructureNode infrastructureNode2 = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node 2");

            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.Add(infrastructureNode1);

            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(infrastructureNode1)));
        }

        [Fact]
        public void Test_Add_AddsTheSoftwareSystemInstance()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node ");
            SoftwareSystemInstance softwareSystemInstance = deploymentNodeChild.Add(softwareSystem);

            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.Add(softwareSystemInstance);

            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(softwareSystemInstance)));
        }
        
        [Fact]
        public void Test_AddSoftwareSystemInstance_ThrowsAnException_WhenAChildContainerInstanceHasAlreadyBeenAdded()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System");
            Container container = softwareSystem.AddContainer("Container");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            SoftwareSystemInstance softwareSystemInstance = deploymentNodeChild.Add(softwareSystem);
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.Add(containerInstance);

            try 
            {
                deploymentView.Add(softwareSystemInstance);
                throw new TestFailedException();
            }
            catch (ElementNotPermittedInViewException e)
            {
                Assert.Equal("A child of Software System is already in this view.", e.Message);
            }
        }

        [Fact]
        public void Test_Add_AddsTheContainerInstance()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System");
            Container container = softwareSystem.AddContainer("Container");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node ");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.Add(containerInstance);

            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(containerInstance)));
        }

        [Fact]
        public void Test_AddContainerInstance_ThrowsAnException_WhenTheParentSoftwareSystemInstanceHasAlreadyBeenAdded()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System");
            Container container = softwareSystem.AddContainer("Container");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            SoftwareSystemInstance softwareSystemInstance = deploymentNodeChild.Add(softwareSystem);
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.Add(softwareSystemInstance);

            try 
            {
                deploymentView.Add(containerInstance);
                throw new TestFailedException();
            }
            catch (ElementNotPermittedInViewException e)
            {
                Assert.Equal("The parent of Container is already in this view.", e.Message);
            }
        }

        [Fact]
        public void Test_Remove_RemovesTheInfrastructureNode()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(4, deploymentView.Elements.Count);

            deploymentView.Remove(infrastructureNode);
            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(containerInstance)));
        }

        [Fact]
        public void Test_Remove_RemovesTheContainerInstance()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(4, deploymentView.Elements.Count);

            deploymentView.Remove(containerInstance);
            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(infrastructureNode)));
        }

        [Fact]
        public void Test_Remove_RemovesTheSoftwareSystemInstance()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node");
            SoftwareSystemInstance softwareSystemInstance = deploymentNodeChild.Add(softwareSystem);

            deploymentView = Views.CreateDeploymentView("deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(4, deploymentView.Elements.Count);

            deploymentView.Remove(softwareSystemInstance);
            Assert.Equal(3, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeChild)));
            Assert.True(deploymentView.Elements.Contains(new ElementView(infrastructureNode)));
        }

        [Fact]
        public void Test_Remove_RemovesTheDeploymentNodeAndChildren()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(4, deploymentView.Elements.Count);

            deploymentView.Remove(deploymentNodeChild);
            Assert.Equal(1, deploymentView.Elements.Count);
            Assert.True(deploymentView.Elements.Contains(new ElementView(deploymentNodeParent)));
        }

        [Fact]
        public void Test_Remove_RemovesTheChildDeploymentNodeAndChildren()
        {
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "");
            Container container = softwareSystem.AddContainer("Container", "Description", "Technology");
            DeploymentNode deploymentNodeParent = Model.AddDeploymentNode("Deployment Node", "Description", "Technology");
            DeploymentNode deploymentNodeChild = deploymentNodeParent.AddDeploymentNode("Deployment Node", "Description", "Technology");
            InfrastructureNode infrastructureNode = deploymentNodeChild.AddInfrastructureNode("Infrastructure Node");
            ContainerInstance containerInstance = deploymentNodeChild.Add(container);

            deploymentView = Views.CreateDeploymentView(softwareSystem, "deployment", "Description");
            deploymentView.AddAllDeploymentNodes();
            Assert.Equal(4, deploymentView.Elements.Count);

            deploymentView.Remove(deploymentNodeParent);
            Assert.Equal(0, deploymentView.Elements.Count);
        }

    }
}