using System;
using System.Collections.Generic;
using Xunit;

namespace Structurizr.Core.Tests
{
    public class ModelItemTests : AbstractTestBase
    {

        [Fact]
        public void Test_Construction()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            Assert.Equal("Name", element.Name);
            Assert.Equal("Description", element.Description);
        }

        [Fact]
        public void Test_GetTags_WhenThereAreNoTags()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            Assert.Equal("Element,Software System", element.Tags);
        }

        [Fact]
        public void Test_GetTags_ReturnsTheListOfTags_WhenThereAreSomeTags()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.AddTags("tag1", "tag2", "tag3");
            Assert.Equal("Element,Software System,tag1,tag2,tag3", element.Tags);
        }

        [Fact]
        public void Test_setTags_DoesNotDoAnything_WhenPassedNull()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.Tags = null;
            Assert.Equal("Element,Software System", element.Tags);
        }

        [Fact]
        public void Test_AddTags_DoesNotDoAnything_WhenPassedNull()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.AddTags((String)null);
            Assert.Equal("Element,Software System", element.Tags);

            element.AddTags(null, null, null);
            Assert.Equal("Element,Software System", element.Tags);
        }

        [Fact]
        public void Test_AddTags_AddsTags_WhenPassedSomeTags()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.AddTags(null, "tag1", null, "tag2");
            Assert.Equal("Element,Software System,tag1,tag2", element.Tags);
        }

        [Fact]
        public void Test_AddTags_AddsTags_WhenPassedSomeTagsAndThereAreDuplicateTags()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.AddTags(null, "tag1", null, "tag2", "tag2");
            Assert.Equal("Element,Software System,tag1,tag2", element.Tags);
        }

        [Fact]
        public void Test_GetAllTags_NoTagAdded_RequiredTagsAreReturned()
        {
            Person user = Model.AddPerson("Person", "Description");
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "Description");
            var relation = user.Uses(softwareSystem, "Uses", "");
            // Relationship.GetRequiredTags() == new List<string> { Structurizr.Tags.Relationship, Structurizr.Tags.Synchronous }
            Assert.Equal(new List<string> { Structurizr.Tags.Relationship }, relation.GetAllTags());
        }

        [Fact]
        public void Test_GetAllTags_TagsAdded_AddedTagsAndRequiredTagsAreReturned()
        {
            Person user = Model.AddPerson("Person", "Description");
            SoftwareSystem softwareSystem = Model.AddSoftwareSystem("Software System", "Description");
            var relation = user.Uses(softwareSystem, "Uses", "");
            relation.AddTags("TagA","TagB");
            // Relationship.GetRequiredTags() == new List<string> { Structurizr.Tags.Relationship, Structurizr.Tags.Synchronous }
            Assert.Equal(new List<string> { Structurizr.Tags.Relationship, "TagA","TagB" }, relation.GetAllTags());
        }

        [Fact]
        public void Test_GetProperties_ReturnsAnEmptyList_WhenNoPropertiesHaveBeenAdded()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            Assert.Equal(0, element.Properties.Count);
        }

        [Fact]
        public void Test_AddProperty_ThrowsAnException_WhenTheNameIsNull()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddProperty(null, "value");
                throw new TestFailedException();
            }
            catch (ArgumentException e)
            {
                Assert.Equal("A property name must be specified.", e.Message);
            }
        }

        [Fact]
        public void Test_AddProperty_ThrowsAnException_WhenTheNameIsEmpty()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddProperty(" ", "value");
                throw new TestFailedException();
            }
            catch (ArgumentException e)
            {
                Assert.Equal("A property name must be specified.", e.Message);
            }
        }

        [Fact]
        public void Test_AddProperty_ThrowsAnException_WhenTheValueIsNull()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddProperty("name", null);
                throw new TestFailedException();
            }
            catch (ArgumentException e)
            {
                Assert.Equal("A property value must be specified.", e.Message);
            }
        }

        [Fact]
        public void Test_AddProperty_ThrowsAnException_WhenTheValueIsEmpty()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddProperty("name", " ");
                throw new TestFailedException();
            }
            catch (ArgumentException e)
            {
                Assert.Equal("A property value must be specified.", e.Message);
            }
        }

        [Fact]
        public void Test_AddProperty_AddsTheProperty_WhenANameAndValueAreSpecified()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.AddProperty("AWS region", "us-east-1");
            Assert.Equal("us-east-1", element.Properties["AWS region"]);
        }

        [Fact]
        public void Test_SetProperties_DoesNothing_WhenNullIsSpecified()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            element.Properties = null;
            Assert.Equal(0, element.Properties.Count);
        }

        [Fact]
        public void Test_SetProperties_SetsTheProperties_WhenANonEmptyMapIsSpecified()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            Dictionary<string,string> properties = new Dictionary<string,string>();
            properties.Add("name", "value");
            element.Properties = properties;
            Assert.Equal(1, element.Properties.Count);
            Assert.Equal("value", element.Properties["name"]);
        }

        [Fact]
        public void Test_AddPerspective_ThrowsAnException_WhenANameIsNotSpecified()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddPerspective(null, null);
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("A name must be specified.", iae.Message);
            }
        }

        [Fact]
        public void Test_AddPerspective_ThrowsAnException_WhenAnEmptyNameIsSpecified()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddPerspective(" ", null);
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("A name must be specified.", iae.Message);
            }
        }
        [Fact]
        public void Test_AddPerspective_ThrowsAnException_WhenADescriptionIsNotSpecified()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddPerspective("Security", null);
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("A description must be specified.", iae.Message);
            }
        }

        [Fact]
        public void Test_AddPerspective_ThrowsAnException_WhenAnEmptyDescriptionIsSpecified()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddPerspective("Security", " ");
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("A description must be specified.", iae.Message);
            }
        }

        [Fact]
        public void Test_AddPerspective_AddsAPerspective()
        {
            Element element = Model.AddSoftwareSystem("Name", "Description");
            Perspective perspective = element.AddPerspective("Security", "Data is encrypted at rest.");
            Assert.Equal("Security", perspective.Name);
            Assert.Equal("Data is encrypted at rest.", perspective.Description);
            Assert.Equal(1, element.Perspectives.Count);
            Assert.True(element.Perspectives.Contains(perspective));
        }

        [Fact]
        public void Test_AddPerspective_ThrowsAnException_WhenTheNamedPerspectiveAlreadyExists()
        {
            try
            {
                Element element = Model.AddSoftwareSystem("Name", "Description");
                element.AddPerspective("Security", "Data is encrypted at rest.");
                element.AddPerspective("Security", "Data is encrypted at rest.");
                throw new TestFailedException();
            }
            catch (ArgumentException iae)
            {
                Assert.Equal("A perspective named \"Security\" already exists.", iae.Message);
            }
        }


    }
}