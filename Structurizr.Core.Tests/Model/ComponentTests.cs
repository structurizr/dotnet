using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class ComponentTests
    {

        private Workspace workspace;
        private Model model;
        private SoftwareSystem softwareSystem;
        private Container container;

        public ComponentTests()
        {
            workspace = new Workspace("Name", "Description");
            model = workspace.Model;
            softwareSystem = model.AddSoftwareSystem("System", "Description");
            container = softwareSystem.AddContainer("Container", "Description", "Some technology");
        }

        [Fact]
        public void Test_Name_ReturnsTheGivenName_WhenANameIsGiven()
        {
            Component component = new Component();
            component.Name = "Some name";
            Assert.Equal("Some name", component.Name);
        }

        [Fact]
        public void Test_CanonicalName()
        {
            Component component = container.AddComponent("Component", "Description");
            Assert.Equal("Component://System.Container.Component", component.CanonicalName);
        }

        [Fact]
        public void Test_CanonicalName_WhenNameContainsASlashCharacter()
        {
            Component component = container.AddComponent("Name1/Name2", "Description");
            Assert.Equal("Component://System.Container.Name1Name2", component.CanonicalName);
        }

        [Fact]
        public void Test_Parent_ReturnsTheParentContainer()
        {
            Component component = container.AddComponent("Component", "Description");
            Assert.Equal(container, component.Parent);
        }

        [Fact]
        public void Test_Container_ReturnsTheParentContainer()
        {
            Component component = container.AddComponent("Name", "Description");
            Assert.Equal(container, component.Container);
        }

        [Fact]
        public void Test_RemoveTags_DoesNotRemoveRequiredTags()
        {
            Component component = new Component();
            Assert.True(component.Tags.Contains(Tags.Element));
            Assert.True(component.Tags.Contains(Tags.Component));

            component.RemoveTag(Tags.Component);
            component.RemoveTag(Tags.Element);

            Assert.True(component.Tags.Contains(Tags.Element));
            Assert.True(component.Tags.Contains(Tags.Component));
        }

    }
}