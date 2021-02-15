using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Structurizr.Core.Tests
{
    
    public class DynamicViewTests : AbstractTestBase
    {

        private Person person;
        private SoftwareSystem softwareSystemA;
        private Container containerA1;
        private Container containerA2;
        private Container containerA3;
        private Component componentA1;
        private Component componentA2;

        private SoftwareSystem softwareSystemB;
        private Container containerB1;
        private Component componentB1;

        private Relationship relationship;

        public DynamicViewTests()
        {
            person = Model.AddPerson("Person", "");
            softwareSystemA = Model.AddSoftwareSystem("Software System A", "");
            containerA1 = softwareSystemA.AddContainer("Container A1", "", "");
            componentA1 = containerA1.AddComponent("Component A1", "");
            containerA2 = softwareSystemA.AddContainer("Container A2", "", "");
            componentA2 = containerA2.AddComponent("Component A2", "");
            containerA3 = softwareSystemA.AddContainer("Container A3", "", "");
            relationship = containerA1.Uses(containerA2, "uses");

            softwareSystemB = Model.AddSoftwareSystem("Software System B", "");
            containerB1 = softwareSystemB.AddContainer("Container B1", "", "");
        }

        [Fact]
        public void Test_Add_ThrowsAnException_WhenTheScopeOfTheDynamicViewIsNotSpecifiedButAContainerIsAdded()
        {
            try
            {
                DynamicView dynamicView = Workspace.Views.CreateDynamicView("key", "Description");
                dynamicView.Add(containerA1, containerA1);
                throw new TestFailedException();
            }
            catch (ElementNotPermittedInViewException iae)
            {
                Assert.Equal("Only people and software systems can be added to this dynamic view.", iae.Message);
            }
        }

        [Fact]
        public void Test_Add_ThrowsAnException_WhenTheScopeOfTheDynamicViewIsNotSpecifiedButAComponentIsAdded()
        {
            try
            {
                DynamicView dynamicView = Workspace.Views.CreateDynamicView("key", "Description");
                dynamicView.Add(componentA1, componentA1);
                throw new TestFailedException();
            }
            catch (ElementNotPermittedInViewException iae)
            {
                Assert.Equal("Only people and software systems can be added to this dynamic view.", iae.Message);
            }
        }
        
        [Fact]
        public void Test_Add_ThrowsAnException_WhenTheScopeOfTheDynamicViewIsASoftwareSystemButAComponentIsAdded()
        {
            try
            {
                DynamicView dynamicView = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
                dynamicView.Add(componentA1, containerA1);
                throw new TestFailedException();
            }
            catch (Exception e)
            {
                Assert.Equal("Components can't be added to a dynamic view when the scope is a software system.", e.Message);
            }
        }

        [Fact]
        public void Test_Add_ThrowsAnException_WhenTheScopeOfTheDynamicViewIsASoftwareSystemAndTheSameSoftwareSystemIsAdded()
        {
            try
            {
                DynamicView dynamicView = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
                dynamicView.Add(softwareSystemA, containerA1);
                throw new TestFailedException();
            }
            catch (Exception e)
            {
                Assert.Equal("Software System A is already the scope of this view and cannot be added to it.", e.Message);
            }
        }

        [Fact]
        public void Test_Add_ThrowsAnException_WhenTheScopeOfTheDynamicViewIsAContainerAndTheSameContainerIsAdded()
        {
            try
            {
                DynamicView dynamicView = Workspace.Views.CreateDynamicView(containerA1, "key", "Description");
                dynamicView.Add(containerA1, containerA2);
                throw new TestFailedException();
            }
            catch (Exception e)
            {
                Assert.Equal("Container A1 is already the scope of this view and cannot be added to it.", e.Message);
            }
        }

        [Fact]
        public void Test_Add_ThrowsAnException_WhenTheScopeOfTheDynamicViewIsAContainerAndTheParentSoftwareSystemIsAdded()
        {
            try
            {
                DynamicView dynamicView = Workspace.Views.CreateDynamicView(containerA1, "key", "Description");
                dynamicView.Add(softwareSystemA, containerA2);
                throw new TestFailedException();
            }
            catch (Exception e)
            {
                Assert.Equal("Software System A is already the scope of this view and cannot be added to it.", e.Message);
            }
        }

        [Fact]
        public void Test_Add_AddsTheSourceAndDestinationElements_WhenARelationshipBetweenThemExists()
        {
            DynamicView dynamicView = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            dynamicView.Add(containerA1, containerA2);
            Assert.Equal(2, dynamicView.Elements.Count);
        }

        [Fact]
        public void Test_Add_ThrowsAnException_WhenThereIsNoRelationshipBetweenTheSourceAndDestinationElements()
        {
            DynamicView dynamicView = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            Assert.Throws<ArgumentException>(() =>
                dynamicView.Add(containerA1, containerA3)
            );
        }

        [Fact]
        public void Test_AddRelationshipDirectly()
        {
            DynamicView dynamicView = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            dynamicView.Add(relationship);
            Assert.Equal(2, dynamicView.Elements.Count);
        }

        [Fact]
        public void Test_Add_AddsTheSourceAndDestinationElements_WhenARelationshipBetweenThemExistsAndTheDestinationIsAnExternalSoftwareSystem()
        {
            DynamicView dynamicView = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            containerA2.Uses(softwareSystemB, "", "");
            dynamicView.Add(containerA2, softwareSystemB);
            Assert.Equal(2, dynamicView.Elements.Count);
        }

        [Fact]
        public void Test_NormalSequence()
        {
            Workspace = new Workspace("Name", "Description");
            Model = Workspace.Model;

            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "Description");
            Container container1 = softwareSystem.AddContainer("Container 1", "Description", "Technology");
            Container container2 = softwareSystem.AddContainer("Container 2", "Description", "Technology");
            Container container3 = softwareSystem.AddContainer("Container 3", "Description", "Technology");

            container1.Uses(container2, "Uses");
            container1.Uses(container3, "Uses");

            DynamicView view = Workspace.Views.CreateDynamicView(softwareSystem, "key", "Description");

            view.Add(container1, container2);
            view.Add(container1, container3);

            Assert.Same(container2, view.Relationships.First(rv => rv.Order.Equals("1")).Relationship.Destination);
            Assert.Same(container3, view.Relationships.First(rv => rv.Order.Equals("2")).Relationship.Destination);
        }

        [Fact]
        public void Test_ParallelSequence()
        {
            Workspace = new Workspace("Name", "Description");
            Model = Workspace.Model;
            SoftwareSystem softwareSystemA = Model.AddSoftwareSystem("A", "");
            SoftwareSystem softwareSystemB = Model.AddSoftwareSystem("B", "");
            SoftwareSystem softwareSystemC1 = Model.AddSoftwareSystem("C1", "");
            SoftwareSystem softwareSystemC2 = Model.AddSoftwareSystem("C2", "");
            SoftwareSystem softwareSystemD = Model.AddSoftwareSystem("D", "");
            SoftwareSystem softwareSystemE = Model.AddSoftwareSystem("E", "");

            // A -> B -> C1 -> D -> E
            // A -> B -> C2 -> D -> E
            softwareSystemA.Uses(softwareSystemB, "uses");
            softwareSystemB.Uses(softwareSystemC1, "uses");
            softwareSystemC1.Uses(softwareSystemD, "uses");
            softwareSystemB.Uses(softwareSystemC2, "uses");
            softwareSystemC2.Uses(softwareSystemD, "uses");
            softwareSystemD.Uses(softwareSystemE, "uses");

            DynamicView view = Workspace.Views.CreateDynamicView("key", "Description");

            view.Add(softwareSystemA, softwareSystemB);
            view.StartParallelSequence();
            view.Add(softwareSystemB, softwareSystemC1);
            view.Add(softwareSystemC1, softwareSystemD);
            view.EndParallelSequence();
            view.StartParallelSequence();
            view.Add(softwareSystemB, softwareSystemC2);
            view.Add(softwareSystemC2, softwareSystemD);
            view.EndParallelSequence(true);
            view.Add(softwareSystemD, softwareSystemE);

            Assert.Equal(1, view.Relationships.Count(r=>r.Order.Equals("1")));
            Assert.Equal(2, view.Relationships.Count(r => r.Order.Equals("2")));
            Assert.Equal(2, view.Relationships.Count(r => r.Order.Equals("3")));
            Assert.Equal(1, view.Relationships.Count(r => r.Order.Equals("4")));
        }

        [Fact]
        public void Test_GetRelationships_WhenTheOrderPropertyIsAnInteger()
        {
            containerA1.Uses(containerA2, "uses");
            DynamicView view = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            for (int i = 0; i < 10; i++) {
                view.Add(containerA1, containerA2);
            }

            List<RelationshipView> relationships = new List<RelationshipView>(view.Relationships);
            Assert.Equal("1", relationships[0].Order);
            Assert.Equal("2", relationships[1].Order);
            Assert.Equal("3", relationships[2].Order);
            Assert.Equal("4", relationships[3].Order);
            Assert.Equal("5", relationships[4].Order);
            Assert.Equal("6", relationships[5].Order);
            Assert.Equal("7", relationships[6].Order);
            Assert.Equal("8", relationships[7].Order);
            Assert.Equal("9", relationships[8].Order);
            Assert.Equal("10", relationships[9].Order);
        }

        [Fact]
        public void Test_GetRelationships_WhenTheOrderPropertyIsADecimal()
        {
            containerA1.Uses(containerA2, "uses");
            DynamicView view = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            for (int i = 0; i < 10; i++)
            {
                RelationshipView relationshipView = view.Add(containerA1, containerA2);
                relationshipView.Order = "1." + i;
            }

            List<RelationshipView> relationships = new List<RelationshipView>(view.Relationships);
            Assert.Equal("1.0", relationships[0].Order);
            Assert.Equal("1.1", relationships[1].Order);
            Assert.Equal("1.2", relationships[2].Order);
            Assert.Equal("1.3", relationships[3].Order);
            Assert.Equal("1.4", relationships[4].Order);
            Assert.Equal("1.5", relationships[5].Order);
            Assert.Equal("1.6", relationships[6].Order);
            Assert.Equal("1.7", relationships[7].Order);
            Assert.Equal("1.8", relationships[8].Order);
            Assert.Equal("1.9", relationships[9].Order);
        }

        [Fact]
        public void Test_GetRelationships_WhenTheOrderPropertyIsAString()
        {
            string characters = "abcdefghij";
            containerA1.Uses(containerA2, "uses");
            DynamicView view = Workspace.Views.CreateDynamicView(softwareSystemA, "key", "Description");
            for (int i = 0; i < 10; i++)
            {
                RelationshipView relationshipView = view.Add(containerA1, containerA2);
                relationshipView.Order = "1" + characters.ToCharArray()[i];
            }

            List<RelationshipView> relationships = new List<RelationshipView>(view.Relationships);
            Assert.Equal("1a", relationships[0].Order);
            Assert.Equal("1b", relationships[1].Order);
            Assert.Equal("1c", relationships[2].Order);
            Assert.Equal("1d", relationships[3].Order);
            Assert.Equal("1e", relationships[4].Order);
            Assert.Equal("1f", relationships[5].Order);
            Assert.Equal("1g", relationships[6].Order);
            Assert.Equal("1h", relationships[7].Order);
            Assert.Equal("1i", relationships[8].Order);
            Assert.Equal("1j", relationships[9].Order);
        }

    }
}