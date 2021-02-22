using System;
using System.Collections.Generic;
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
    
        [Fact]
        public void Test_CopyLayoutInformationFrom()
        {
            Workspace workspace1 = new Workspace("", "");
            Model model1 = workspace1.Model;
            SoftwareSystem softwareSystem1A = model1.AddSoftwareSystem("System A", "Description");
            SoftwareSystem softwareSystem1B = model1.AddSoftwareSystem("System B", "Description");
            Person person1 = model1.AddPerson("Person", "Description");
            Relationship personUsesSoftwareSystem1 = person1.Uses(softwareSystem1A, "Uses");

            // create a view with SystemA and Person (locations are set for both, relationship has vertices)
            StaticView staticView1 = new SystemContextView(softwareSystem1A, "context", "Description");
            staticView1.Add(softwareSystem1B);
            staticView1.GetElementView(softwareSystem1B).X = 123;
            staticView1.GetElementView(softwareSystem1B).Y = 321;
            staticView1.Add(person1);
            staticView1.GetElementView(person1).X = 456;
            staticView1.GetElementView(person1).Y = 654;
            staticView1.GetRelationshipView(personUsesSoftwareSystem1).Vertices = new List<Vertex>() { new Vertex(123, 456) };
            staticView1.GetRelationshipView(personUsesSoftwareSystem1).Position = 70;
            staticView1.GetRelationshipView(personUsesSoftwareSystem1).Routing = Routing.Orthogonal;

            // and create a dynamic view, as they are treated slightly differently
            DynamicView dynamicView1 = new DynamicView(model1, "dynamic", "Description");
            dynamicView1.Add(person1, "Overridden description", softwareSystem1A);
            dynamicView1.GetElementView(person1).X = 111;
            dynamicView1.GetElementView(person1).Y = 222;
            dynamicView1.GetElementView(softwareSystem1A).X = 333;
            dynamicView1.GetElementView(softwareSystem1A).Y = 444;
            dynamicView1.GetRelationshipView(personUsesSoftwareSystem1).Vertices = new List<Vertex>() { new Vertex(555, 666) };
            dynamicView1.GetRelationshipView(personUsesSoftwareSystem1).Position = 30;
            dynamicView1.GetRelationshipView(personUsesSoftwareSystem1).Routing = Routing.Direct;

            Workspace workspace2 = new Workspace("", "");
            Model model2 = workspace2.Model;
            // creating these in the opposite order will cause them to get different internal IDs
            SoftwareSystem softwareSystem2B = model2.AddSoftwareSystem("System B", "Description");
            SoftwareSystem softwareSystem2A = model2.AddSoftwareSystem("System A", "Description");
            Person person2 = model2.AddPerson("Person", "Description");
            Relationship personUsesSoftwareSystem2 = person2.Uses(softwareSystem2A, "Uses");

            // create a view with SystemB and Person (locations are 0,0 for both)
            StaticView staticView2 = new SystemContextView(softwareSystem2A, "context", "Description");
            staticView2.Add(softwareSystem2B);
            staticView2.Add(person2);
            Assert.Equal(0, staticView2.GetElementView(softwareSystem2B).X);
            Assert.Equal(0, staticView2.GetElementView(softwareSystem2B).Y);
            Assert.Equal(0, staticView2.GetElementView(softwareSystem2B).X);
            Assert.Equal(0, staticView2.GetElementView(softwareSystem2B).Y);
            Assert.Equal(0, staticView2.GetElementView(person2).X);
            Assert.Equal(0, staticView2.GetElementView(person2).Y);
            Assert.True(staticView2.GetRelationshipView(personUsesSoftwareSystem2).Vertices.Count == 0);

            // and create a dynamic view (locations are 0,0)
            DynamicView dynamicView2 = new DynamicView(model2, "dynamic", "Description");
            dynamicView2.Add(person2, "Overridden description", softwareSystem2A);

            staticView2.CopyLayoutInformationFrom(staticView1);
            Assert.Equal(0, staticView2.GetElementView(softwareSystem2A).X);
            Assert.Equal(0, staticView2.GetElementView(softwareSystem2A).Y);
            Assert.Equal(123, staticView2.GetElementView(softwareSystem2B).X);
            Assert.Equal(321, staticView2.GetElementView(softwareSystem2B).Y);
            Assert.Equal(456, staticView2.GetElementView(person2).X);
            Assert.Equal(654, staticView2.GetElementView(person2).Y);
            Vertex vertex1 = staticView2.GetRelationshipView(personUsesSoftwareSystem2).Vertices[0];
            Assert.Equal(123, vertex1.X);
            Assert.Equal(456, vertex1.Y);
            Assert.Equal(70, staticView2.GetRelationshipView(personUsesSoftwareSystem2).Position);
            Assert.Equal(Routing.Orthogonal, staticView2.GetRelationshipView(personUsesSoftwareSystem2).Routing);

            dynamicView2.CopyLayoutInformationFrom(dynamicView1);
            Assert.Equal(111, dynamicView2.GetElementView(person2).X);
            Assert.Equal(222, dynamicView2.GetElementView(person2).Y);
            Assert.Equal(333, dynamicView2.GetElementView(softwareSystem2A).X);
            Assert.Equal(444, dynamicView2.GetElementView(softwareSystem2A).Y);
            Vertex vertex2 = dynamicView2.GetRelationshipView(personUsesSoftwareSystem2).Vertices[0];
            Assert.Equal(555, vertex2.X);
            Assert.Equal(666, vertex2.Y);
            Assert.Equal(30, dynamicView2.GetRelationshipView(personUsesSoftwareSystem2).Position);
            Assert.Equal(Routing.Direct, dynamicView2.GetRelationshipView(personUsesSoftwareSystem2).Routing);
        }

    }

}