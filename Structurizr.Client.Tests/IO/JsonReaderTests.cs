using System;
using System.IO;
using Structurizr.IO.Json;
using Xunit;

namespace Structurizr.Api.Tests.IO
{
    public class JsonReaderTests
    {
        
        [Fact]
        public void Test_Write_and_Read()
        {
            Workspace workspace1 = new Workspace("Name", "Description");

            // output the model as JSON
            JsonWriter jsonWriter = new JsonWriter(true);
            StringWriter stringWriter = new StringWriter();
            jsonWriter.Write(workspace1, stringWriter);

            // and read it back again
            JsonReader jsonReader = new JsonReader();
            StringReader stringReader = new StringReader(stringWriter.ToString());
            Workspace workspace2 = jsonReader.Read(stringReader);
            Assert.Equal("Name", workspace2.Name);
            Assert.Equal("Description", workspace2.Description);
        }
    
        [Fact]
        public void Test_DeserializationOfProperties()
        {
            Workspace workspace = new Workspace("Name", "Description");
            SoftwareSystem softwareSystem = workspace.Model.AddSoftwareSystem("Name", "Description");
            softwareSystem.AddProperty("Name", "Value");
            
            StringWriter stringWriter = new StringWriter();
            new JsonWriter(false).Write(workspace, stringWriter);
                        
            StringReader stringReader = new StringReader(stringWriter.ToString());
            workspace = new JsonReader().Read(stringReader);
            Assert.Equal("Value", workspace.Model.GetSoftwareSystemWithName("Name").Properties["Name"]);
        }
        
        [Fact]
        public void Test_write_and_read_withCustomIdGenerator()
        {
            Workspace workspace1 = new Workspace("Name", "Description");
            workspace1.Model.IdGenerator = new CustomIdGenerator();
            Person user = workspace1.Model.AddPerson("User");
            SoftwareSystem softwareSystem = workspace1.Model.AddSoftwareSystem("Software System");
            user.Uses(softwareSystem, "Uses");

            // output the model as JSON
            JsonWriter jsonWriter = new JsonWriter(true);
            StringWriter stringWriter = new StringWriter();
            jsonWriter.Write(workspace1, stringWriter);
            
            // and read it back again
            JsonReader jsonReader = new JsonReader();
            jsonReader.IdGenerator = new CustomIdGenerator();
            StringReader stringReader = new StringReader(stringWriter.ToString());
            
            Workspace workspace2 = jsonReader.Read(stringReader);
            Assert.Equal(user.Id, workspace2.Model.GetPersonWithName("User").Id);
            Assert.NotNull(workspace2.Model.GetElement(user.Id));
        }
        
    }
    
    class CustomIdGenerator : IdGenerator
    {
        public string GenerateId(Element element)
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateId(Relationship relationship)
        {
            return Guid.NewGuid().ToString();
        }

        public void Found(string id)
        {
        }
    }
    
}