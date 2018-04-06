
using System.IO;
using System.Linq;
using Structurizr.IO.Json;
using Structurizr.IO.PlantUML;
using Xunit;

namespace Structurizr.Core.Tests.IO.Json
{
    public class JsonReaderTests
    {
        [Fact]
        public void Deserialize_ContainerView_Success()
        {
            // arrange
            var workspace = new Workspace("", "");
            var softwareSystem = workspace.Model.AddSoftwareSystem("", "");
            workspace.Views.CreateContainerView(softwareSystem, "Container", "");

            var jsonWriter = new JsonWriter(true);
            var stringWriter = new StringWriter();
            jsonWriter.Write(workspace, stringWriter);
            var jsonWorkspace = stringWriter.ToString();

            // act
            var stringReader = new StringReader(jsonWorkspace);
            var workspaceDeserialized = new JsonReader().Read(stringReader);

            // assert
            var containerView = workspaceDeserialized.Views.ContainerViews.First();
            Assert.NotNull(containerView.Name);
            // e.g. PlantUMLWriter.Write() will throw if ContainerView.Name is null!
            new PlantUMLWriter().Write(workspaceDeserialized, new StringWriter());
        }
    }
}
