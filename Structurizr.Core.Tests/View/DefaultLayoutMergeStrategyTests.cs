using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class DefaultLayoutMergeStrategyTests
    {
    
        [Fact]
        public void Test_CopyLayoutInformation_WhenCanonicalNamesHaveNotChanged()
        {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1 = workspace1.Model.AddSoftwareSystem("Software System");
            Container container1 = softwareSystem1.AddContainer("Container", "", "");
            ContainerView view1 = workspace1.Views.CreateContainerView(softwareSystem1, "key", "");
            view1.Add(container1);
            view1.GetElementView(container1).X = 123;
            view1.GetElementView(container1).Y = 456;

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2 = workspace2.Model.AddSoftwareSystem("Software System");
            Container container2 = softwareSystem2.AddContainer("Container", "", "");
            ContainerView view2 = workspace2.Views.CreateContainerView(softwareSystem2, "key", "");
            view2.Add(container2);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);

            Assert.Equal(123, view2.GetElementView(container2).X);
            Assert.Equal(456, view2.GetElementView(container2).Y);
        }

        [Fact]
        public void Test_CopyLayoutInformation_WhenAParentElementNameHasChanged()
        {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1 = workspace1.Model.AddSoftwareSystem("Software System");
            Container container1 = softwareSystem1.AddContainer("Container", "", "");
            ContainerView view1 = workspace1.Views.CreateContainerView(softwareSystem1, "key", "");
            view1.Add(container1);
            view1.GetElementView(container1).X = 123;
            view1.GetElementView(container1).Y = 456;

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2 = workspace2.Model.AddSoftwareSystem("Software System with a new name");
            Container container2 = softwareSystem2.AddContainer("Container", "", "");
            ContainerView view2 = workspace2.Views.CreateContainerView(softwareSystem2, "key", "");
            view2.Add(container2);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);

            Assert.Equal(123, view2.GetElementView(container2).X);
            Assert.Equal(456, view2.GetElementView(container2).Y);
        }

        [Fact]
        public void Test_CopyLayoutInformation_WhenAnElementNameHasChangedButTheDescriptionHasNotChanged()
        {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1 = workspace1.Model.AddSoftwareSystem("Software System");
            Container container1 = softwareSystem1.AddContainer("Container", "Container description", "");
            ContainerView view1 = workspace1.Views.CreateContainerView(softwareSystem1, "key", "");
            view1.Add(container1);
            view1.GetElementView(container1).X = 123;
            view1.GetElementView(container1).Y = 456;

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2 = workspace2.Model.AddSoftwareSystem("Software System");
            Container container2 = softwareSystem2.AddContainer("Container with a new name", "Container description", "");
            ContainerView view2 = workspace2.Views.CreateContainerView(softwareSystem2, "key", "");
            view2.Add(container2);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);

            Assert.Equal(123, view2.GetElementView(container2).X);
            Assert.Equal(456, view2.GetElementView(container2).Y);
        }

        [Fact]
        public void Test_CopyLayoutInformation_WhenAnElementNameAndDescriptionHaveChangedButTheIdHasNotChanged()
        {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1 = workspace1.Model.AddSoftwareSystem("Software System");
            Container container1 = softwareSystem1.AddContainer("Container", "Container description", "");
            ContainerView view1 = workspace1.Views.CreateContainerView(softwareSystem1, "key", "");
            view1.Add(container1);
            view1.GetElementView(container1).X = 123;
            view1.GetElementView(container1).Y = 456;

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2 = workspace2.Model.AddSoftwareSystem("Software System");
            Container container2 = softwareSystem2.AddContainer("Container with a new name", "Container with a new description", "");
            ContainerView view2 = workspace2.Views.CreateContainerView(softwareSystem2, "key", "");
            view2.Add(container2);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);

            Assert.Equal(123, view2.GetElementView(container2).X);
            Assert.Equal(456, view2.GetElementView(container2).Y);
        }

        [Fact]
        public void Test_CopyLayoutInformation_WhenAnElementNameAndDescriptionAndIdHaveChanged()
        {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1 = workspace1.Model.AddSoftwareSystem("Software System");
            Container container1 = softwareSystem1.AddContainer("Container", "Container description", "");
            ContainerView view1 = workspace1.Views.CreateContainerView(softwareSystem1, "key", "");
            view1.Add(container1);
            view1.GetElementView(container1).X = 123;
            view1.GetElementView(container1).Y = 456;

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2 = workspace2.Model.AddSoftwareSystem("Software System");
            softwareSystem2.AddContainer("Web Application", "Description", ""); // this element has ID 2
            Container container2 = softwareSystem2.AddContainer("Database", "Description", "");
            ContainerView view2 = workspace2.Views.CreateContainerView(softwareSystem2, "key", "");
            view2.Add(container2);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);

            Assert.Equal(0, view2.GetElementView(container2).X);
            Assert.Equal(0, view2.GetElementView(container2).Y);
        }

        [Fact]
        public void Test_CopyLayoutInformation_WhenAnElementNameAndDescriptionAndIdHaveChangedAndDescriptionWasNull()
        {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1 = workspace1.Model.AddSoftwareSystem("Software System");
            Container container1 = softwareSystem1.AddContainer("Container");
            container1.Description = null;
            ContainerView view1 = workspace1.Views.CreateContainerView(softwareSystem1, "key", "");
            view1.Add(container1);
            view1.GetElementView(container1).X = 123;
            view1.GetElementView(container1).Y = 456;

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2 = workspace2.Model.AddSoftwareSystem("Software System");
            softwareSystem2.AddContainer("Web Application", "Description", ""); // this element has ID 2
            Container container2 = softwareSystem2.AddContainer("Database", "Description", "");
            ContainerView view2 = workspace2.Views.CreateContainerView(softwareSystem2, "key", "");
            view2.Add(container2);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);

            Assert.Equal(0, view2.GetElementView(container2).X);
            Assert.Equal(0, view2.GetElementView(container2).Y);
        }
        
        [Fact]
        public void Test_CopyLayoutInformation_DoesNotThrowAnExceptionWhenAddingAnElementToAView() {
            Workspace workspace1 = new Workspace("1", "");
            SoftwareSystem softwareSystem1A = workspace1.Model.AddSoftwareSystem("Software System A");
            SoftwareSystem softwareSystem1B = workspace1.Model.AddSoftwareSystem("Software System B");
            softwareSystem1A.Uses(softwareSystem1B, "Uses");
            SystemLandscapeView view1 = workspace1.Views.CreateSystemLandscapeView("key", "description");
            view1.Add(softwareSystem1A);

            Workspace workspace2 = new Workspace("2", "");
            SoftwareSystem softwareSystem2A = workspace2.Model.AddSoftwareSystem("Software System A");
            SoftwareSystem softwareSystem2B = workspace2.Model.AddSoftwareSystem("Software System B");
            softwareSystem2A.Uses(softwareSystem2B, "Uses");
            SystemLandscapeView view2 = workspace2.Views.CreateSystemLandscapeView("key", "description");
            view2.Add(softwareSystem2A);
            view2.Add(softwareSystem2B);

            DefaultLayoutMergeStrategy strategy = new DefaultLayoutMergeStrategy();
            strategy.CopyLayoutInformation(view1, view2);
        }

    }
    
}