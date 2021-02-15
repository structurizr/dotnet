using Xunit;

namespace Structurizr.Core.Tests
{

    
    public class SystemContextViewTests : AbstractTestBase
    {

        private SoftwareSystem softwareSystem;
        private SystemContextView view;

        public SystemContextViewTests()
        {
            softwareSystem = Model.AddSoftwareSystem("The System", "Description");
            view = Workspace.Views.CreateSystemContextView(softwareSystem, "context", "Description");
        }

        [Fact]
        public void Test_Construction()
        {
            Assert.Equal("The System - System Context", view.Name);
            Assert.Equal(1, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
            Assert.Same(softwareSystem, view.SoftwareSystem);
            Assert.Equal(softwareSystem.Id, view.SoftwareSystemId);
            Assert.Same(Model, view.Model);
        }

        [Fact]
        public void test_AddAllSoftwareSystems_DoesNothing_WhenThereAreNoOtherSoftwareSystems()
        {
            Assert.Equal(1, view.Elements.Count);
            view.AddAllSoftwareSystems();
            Assert.Equal(1, view.Elements.Count);
        }

        [Fact]
        public void test_AddAllSoftwareSystems_AddsAllSoftwareSystems_WhenThereAreSomeSoftwareSystemsInTheModel()
        {
            SoftwareSystem softwareSystemA = Model.AddSoftwareSystem(Location.External, "System A", "Description");
            SoftwareSystem softwareSystemB = Model.AddSoftwareSystem(Location.External, "System B", "Description");

            view.AddAllSoftwareSystems();

            Assert.Equal(3, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemB)));
        }

        [Fact]
        public void test_AddAllPeople_DoesNothing_WhenThereAreNoPeople()
        {
            Assert.Equal(1, view.Elements.Count);
            view.AddAllPeople();
            Assert.Equal(1, view.Elements.Count);
        }

        [Fact]
        public void test_AddAllPeople_AddsAllPeople_WhenThereAreSomePeopleInTheModel()
        {
            Person userA = Model.AddPerson(Location.External, "User A", "Description");
            Person userB = Model.AddPerson(Location.External, "User B", "Description");

            view.AddAllPeople();

            Assert.Equal(3, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
            Assert.True(view.Elements.Contains(new ElementView(userA)));
            Assert.True(view.Elements.Contains(new ElementView(userB)));
        }

        [Fact]
        public void test_AddAllElements_DoesNothing_WhenThereAreNoSoftwareSystemsOrPeople()
        {
            Assert.Equal(1, view.Elements.Count);
            view.AddAllElements();
            Assert.Equal(1, view.Elements.Count);
        }

        [Fact]
        public void test_AddAllElements_AddsAllSoftwareSystemsAndPeople_WhenThereAreSomeSoftwareSystemsAndPeopleInTheModel()
        {
            SoftwareSystem softwareSystemA = Model.AddSoftwareSystem(Location.External, "System A", "Description");
            SoftwareSystem softwareSystemB = Model.AddSoftwareSystem(Location.External, "System B", "Description");
            Person userA = Model.AddPerson(Location.External, "User A", "Description");
            Person userB = Model.AddPerson(Location.External, "User B", "Description");

            view.AddAllElements();

            Assert.Equal(5, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemB)));
            Assert.True(view.Elements.Contains(new ElementView(userA)));
            Assert.True(view.Elements.Contains(new ElementView(userB)));
        }

        [Fact]
        public void Test_AddNearestNeightbours_DoesNothing_WhenANullElementIsSpecified()
        {
            view.AddNearestNeighbours(null);

            Assert.Equal(1, view.Elements.Count);
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

            view = new SystemContextView(softwareSystem, "context", "Description");
            view.AddNearestNeighbours(softwareSystemA);

            Assert.Equal(3, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(userA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystemA)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem)));
        }

        [Fact]
        public void Test_AddDefaultElements()
        {
            Person user1 = Model.AddPerson("User 1");
            Person user2 = Model.AddPerson("User 2");
            SoftwareSystem softwareSystem1 = Model.AddSoftwareSystem("Software System 1");
            SoftwareSystem softwareSystem2 = Model.AddSoftwareSystem("Software System 2");

            user1.Uses(softwareSystem1, "");
            softwareSystem1.Uses(softwareSystem2, "");
            user2.Uses(softwareSystem2, "");

            view = Views.CreateSystemContextView(softwareSystem1, "key", "description");
            view.AddDefaultElements();

            Assert.Equal(3, view.Elements.Count);
            Assert.True(view.Elements.Contains(new ElementView(user1)));
            Assert.False(view.Elements.Contains(new ElementView(user2)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem1)));
            Assert.True(view.Elements.Contains(new ElementView(softwareSystem2)));
        }

    }

}