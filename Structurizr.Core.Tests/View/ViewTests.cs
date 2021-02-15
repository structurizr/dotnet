using System;
using Xunit;

namespace Structurizr.Core.Tests.View
{
    public class ViewTests : AbstractTestBase
    {
        
        [Fact]
        public void Test_addElement_ThrowsAnException_WhenTheSpecifiedElementDoesNotExistInTheModel()
        {
            try
            {
                Workspace workspace = new Workspace("1", "");
                SoftwareSystem softwareSystem = workspace.Model.AddSoftwareSystem("Software System");

                SystemLandscapeView view = new Workspace("", "").Views.CreateSystemLandscapeView("key", "Description");
                view.Add(softwareSystem);
                throw new TestFailedException();
            }
            catch (ArgumentException ae)
            {
                Assert.Equal("The element named Software System does not exist in the model associated with this view.", ae.Message);
            }
        }

        [Fact]
        public void Test_EnableAutomaticLayout_EnablesAutoLayoutWithSomeDefaultValues()
        {
            SystemLandscapeView view = new Workspace("", "").Views.CreateSystemLandscapeView("key", "Description");
            view.EnableAutomaticLayout();

            Assert.NotNull(view.AutomaticLayout);
            Assert.Equal(RankDirection.TopBottom, view.AutomaticLayout.RankDirection);
            Assert.Equal(300, view.AutomaticLayout.RankSeparation);
            Assert.Equal(600, view.AutomaticLayout.NodeSeparation);
            Assert.Equal(200, view.AutomaticLayout.EdgeSeparation);
            Assert.False(view.AutomaticLayout.Vertices);
        }

        [Fact]
        public void Test_DisableAutomaticLayout_DisablesAutoLayout()
        {
            SystemLandscapeView view = new Workspace("", "").Views.CreateSystemLandscapeView("key", "Description");
            view.EnableAutomaticLayout();
            Assert.NotNull(view.AutomaticLayout);

            view.DisableAutomaticLayout();
            Assert.Null(view.AutomaticLayout);
        }

        [Fact]
        public void Test_EnableAutomaticLayout()
        {
            SystemLandscapeView view = new Workspace("", "").Views.CreateSystemLandscapeView("key", "Description");
            view.EnableAutomaticLayout(RankDirection.LeftRight, 100, 200, 300, true);

            Assert.NotNull(view.AutomaticLayout);
            Assert.Equal(RankDirection.LeftRight, view.AutomaticLayout.RankDirection);
            Assert.Equal(100, view.AutomaticLayout.RankSeparation);
            Assert.Equal(200, view.AutomaticLayout.NodeSeparation);
            Assert.Equal(300, view.AutomaticLayout.EdgeSeparation);
            Assert.True(view.AutomaticLayout.Vertices);
        }
    }
}