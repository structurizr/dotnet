using Xunit;

namespace Structurizr.Core.Tests
{

    
    public class ContainerViewTests : AbstractTestBase
    {

        private SoftwareSystem softwareSystem;
        private ContainerView view;

        public ContainerViewTests()
        {
            softwareSystem = Model.AddSoftwareSystem("The System", "Description");
            view = Workspace.Views.CreateContainerView(softwareSystem, "containers", "Description");
        }

        [Fact]
        public void Test_Construction()
        {
            Assert.Equal("The System - Containers", view.Name);
            Assert.Equal("Description", view.Description);
            Assert.Equal(0, view.Elements.Count);
            Assert.Same(softwareSystem, view.SoftwareSystem);
            Assert.Equal(softwareSystem.Id, view.SoftwareSystemId);
            Assert.Same(Model, view.Model);
        }

        [Fact]
        public void Test_AddAllSoftwareSystems_DoesNothing_WhenThereAreNoOtherSoftwareSystems()
        {
            Assert.Equal(0, view.Elements.Count);
            view.AddAllSoftwareSystems();
            Assert.Equal(0, view.Elements.Count);
        }

        [Fact]
        public void Test_AddAllSoftwareSystems_AddsAllSoftwareSystems_WhenThereAreSomeSoftwareSystemsInTheModel()
        {
            SoftwareSystem softwareSystemA = Model.AddSoftwareSystem(Location.External, "System A", "Description");
            SoftwareSystem softwareSystemB = Model.AddSoftwareSystem(Location.External, "System B", "Description");

            view.AddAllSoftwareSystems();

            Assert.Equal(2, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemB)));
        }

        [Fact]
        public void Test_AddAllPeople_DoesNothing_WhenThereAreNoPeople()
        {
            Assert.Equal(0, view.Elements.Count);
            view.AddAllPeople();
            Assert.Equal(0, view.Elements.Count);
        }

        [Fact]
        public void Test_AddAllPeople_AddsAllPeople_WhenThereAreSomePeopleInTheModel()
        {
            Person userA = Model.AddPerson(Location.External, "User A", "Description");
            Person userB = Model.AddPerson(Location.External, "User B", "Description");

            view.AddAllPeople();

            Assert.Equal(2, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(userA)));
            Assert.True(view.Elements.Contains(new ElementView(userB)));
        }

        [Fact]
        public void Test_AddAllElements_DoesNothing_WhenThereAreNoSoftwareSystemsOrPeople()
        {
            Assert.Equal(0, view.Elements.Count);
            view.AddAllElements();
            Assert.Equal(0, view.Elements.Count);
        }

        [Fact]
        public void Test_AddAllElements_AddsAllSoftwareSystemsAndPeopleAndContainers_WhenThereAreSomeSoftwareSystemsAndPeopleAndContainersInTheModel()
        {
            SoftwareSystem softwareSystemA = Model.AddSoftwareSystem(Location.External, "System A", "Description");
            SoftwareSystem softwareSystemB = Model.AddSoftwareSystem(Location.External, "System B", "Description");
            Person userA = Model.AddPerson(Location.External, "User A", "Description");
            Person userB = Model.AddPerson(Location.External, "User B", "Description");
            Container webApplication = softwareSystem.AddContainer("Web Application", "Does something", "Apache Tomcat");
            Container database = softwareSystem.AddContainer("Database", "Does something", "MySQL");

            view.AddAllElements();

            Assert.Equal(6, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemB)));
            Assert.True(view.Elements.Contains(new ElementView(userA)));
            Assert.True(view.Elements.Contains(new ElementView(userB)));
            Assert.True(view.Elements.Contains(new ElementView(webApplication)));
            Assert.True(view.Elements.Contains(new ElementView(database)));
        }

        [Fact]
        public void Test_AddAllContainers_DoesNothing_WhenThereAreNoContainers()
        {
            Assert.Equal(0, view.Elements.Count);
            view.AddAllContainers();
            Assert.Equal(0, view.Elements.Count);
        }

        [Fact]
        public void Test_AddAllContainers_AddsAllContainers_WhenThereAreSomeContainers()
        {
            Container webApplication = softwareSystem.AddContainer("Web Application", "Does something", "Apache Tomcat");
            Container database = softwareSystem.AddContainer("Database", "Does something", "MySQL");

            view.AddAllContainers();

            Assert.Equal(2, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(webApplication)));
            Assert.True(view.Elements.Contains(new ElementView(database)));
        }

        [Fact]
        public void Test_AddNearestNeightbours_DoesNothing_WhenANullElementIsSpecified()
        {
            view.AddNearestNeighbours(null);

            Assert.Equal(0, view.Elements.Count);
        }

        [Fact]
        public void Test_AddNearestNeighbours_DoesNothing_WhenThereAreNoNeighbours()
        {
            view.AddNearestNeighbours(softwareSystem);

            Assert.Equal(1, view.Elements.Count);
        }

        [Fact]
        public void Test_AddNearestNeighbours_AddsNearestNeighbours_WhenThereAreSomeNearestNeighbours()
        {
            SoftwareSystem softwareSystemA = Model.AddSoftwareSystem("System A", "Description");
            SoftwareSystem softwareSystemB = Model.AddSoftwareSystem("System B", "Description");
            Person userA = Model.AddPerson("User A", "Description");
            Person userB = Model.AddPerson("User B", "Description");

            // userA -> systemA -> system -> systemB -> userB
            userA.Uses(softwareSystemA, "");
            softwareSystemA.Uses(softwareSystem, "");
            softwareSystem.Uses(softwareSystemB, "");
            softwareSystemB.Delivers(userB, "");

            // userA -> systemA -> web application -> systemB -> userB
            // web application -> database
            Container webApplication = softwareSystem.AddContainer("Web Application", "", "");
            Container database = softwareSystem.AddContainer("Database", "", "");
            softwareSystemA.Uses(webApplication, "");
            webApplication.Uses(softwareSystemB, "");
            webApplication.Uses(database, "");

            // userA -> systemA -> controller -> service -> repository -> database
            Component controller = webApplication.AddComponent("Controller", "");
            Component service = webApplication.AddComponent("Service", "");
            Component repository = webApplication.AddComponent("Repository", "");
            softwareSystemA.Uses(controller, "");
            controller.Uses(service, "");
            service.Uses(repository, "");
            repository.Uses(database, "");

            // userA -> systemA -> controller -> service -> systemB -> userB
            service.Uses(softwareSystemB, "");

            view.AddNearestNeighbours(softwareSystem);

            Assert.Equal(3, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemB)));

            view = new ContainerView(softwareSystem, "containers", "Description");
            view.AddNearestNeighbours(softwareSystemA);

            Assert.Equal(4, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(userA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
            Assert.True(view.Elements.Contains(new ElementView(webApplication)));

            view = new ContainerView(softwareSystem, "containers", "Description");
            view.AddNearestNeighbours(webApplication);

            Assert.Equal(4, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(webApplication)));
            Assert.True(view.Elements.Contains(new ElementView(database)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemB)));
        }

        [Fact]
        public void Test_Remove_RemovesContainer()
        {
            Container webApplication = softwareSystem.AddContainer("Web Application", "", "");
            Container database = softwareSystem.AddContainer("Database", "", "");

            view.AddAllContainers();
            Assert.Equal(2, view.Elements.Count);

            view.Remove(webApplication);
            Assert.Equal(1, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(database)));
        }

        [Fact]
        public void Test_AddDefaultElements()
        {
            Model.ImpliedRelationshipsStrategy = new CreateImpliedRelationshipsUnlessAnyRelationshipExistsStrategy();

            Person user1 = Model.AddPerson("User 1");
            Person user2 = Model.AddPerson("User 2");
            SoftwareSystem softwareSystem1 = Model.AddSoftwareSystem("Software System 1");
            Container container1 = softwareSystem1.AddContainer("Container 1", "", "");
            SoftwareSystem softwareSystem2 = Model.AddSoftwareSystem("Software System 2");
            Container container2 = softwareSystem2.AddContainer("Container 2", "", "");

            user1.Uses(container1, "Uses");
            user2.Uses(container2, "Uses");
            container1.Uses(container2, "Uses");

            view = new ContainerView(softwareSystem1, "containers", "Description");
            view.AddDefaultElements();

            Assert.Equal(3, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(user1)));
            Assert.False(view.Elements.Contains(new ElementView(user2)));
            Assert.False(view.Elements.Contains(new ElementView(softwareSystem1)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem2)));
            Assert.True(view.Elements.Contains(new ElementView(container1)));
            Assert.False(view.Elements.Contains(new ElementView(container2)));
        }

    }

}
