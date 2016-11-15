using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace Structurizr.CoreTests
{

    [TestClass]
    public class DocumentationTests
    {

        private SoftwareSystem softwareSystem;
        private Container container;
        private Documentation documentation;

        public DocumentationTests()
        {
            Workspace workspace = new Workspace("Name", "Description");
            Model model = workspace.Model;
            softwareSystem = model.AddSoftwareSystem("Name", "Description");
            container = softwareSystem.AddContainer("Name", "Description", "Technology");
            documentation = workspace.Documentation;
        }

        [TestMethod]
        public void test_Construction()
        {
            Assert.AreEqual(0, documentation.Sections.Count);
            Assert.AreEqual(0, documentation.Images.Count);
        }

        [TestMethod]
        public void Test_AddWithContentForSoftwareSystem_AddsASectionWithTheSpecifiedContent_WhenThatSectionDoesNotExist()
        {
            documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");
            Section section = documentation.Add(softwareSystem, SectionType.FunctionalOverview, DocumentationFormat.Markdown, "Some more Markdown content");

            Assert.AreSame(softwareSystem, section.Element);
            Assert.AreEqual(softwareSystem.Id, section.ElementId);
            Assert.AreEqual(SectionType.FunctionalOverview, section.Type);
            Assert.AreEqual(DocumentationFormat.Markdown, section.Format);
            Assert.AreEqual("Some more Markdown content", section.Content);

            Assert.AreEqual(2, documentation.Sections.Count);
            Assert.IsTrue(documentation.Sections.Contains(section));
        }

        [TestMethod]
        public void Test_AddWithContentForSoftwareSystem_ThrowsAnException_WhenThatSectionAlreadyExists()
        {
            documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");
            Assert.AreEqual(1, documentation.Sections.Count);

            try
            {
                documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                // this is the expected exception
                Assert.AreEqual("A section of type Context for Name already exists.", ae.Message);
                Assert.AreEqual(1, documentation.Sections.Count);
            }
        }

        [TestMethod]
        public void Test_AddFromFileForSoftwareSystem_AddsASectionWithTheSpecifiedContent_WhenThatSectionDoesNotExist()
        {
            documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");

            FileInfo file = new FileInfo("..\\..\\Documentation\\example.md");
            Section section = documentation.Add(softwareSystem, SectionType.FunctionalOverview, DocumentationFormat.Markdown, file);

            Assert.AreSame(softwareSystem, section.Element);
            Assert.AreEqual(softwareSystem.Id, section.ElementId);
            Assert.AreEqual(SectionType.FunctionalOverview, section.Type);
            Assert.AreEqual(DocumentationFormat.Markdown, section.Format);
            Assert.AreEqual("## Heading\r\n" +
                    "\r\n" +
                    "Here is a paragraph.", section.Content);

            Assert.AreEqual(2, documentation.Sections.Count);
            Assert.IsTrue(documentation.Sections.Contains(section));
        }

        [TestMethod]
        public void Test_AddFromFileForSoftwareSystem_ThrowsAnException_WhenThatSectionAlreadyExists()
        {
            documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");
            Assert.AreEqual(1, documentation.Sections.Count);

            try
            {
                FileInfo file = new FileInfo("..\\..\\Documentation\\example.md");
                documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, file);
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                // this is the expected exception
                Assert.AreEqual("A section of type Context for Name already exists.", ae.Message);
                Assert.AreEqual(1, documentation.Sections.Count);
            }
        }

        [TestMethod]
        public void Test_AddWithContentForSoftwareSystem_ThrowsAnException_WhenAContainerIsNotSpecifiedForTheComponentType()
        {
            try
            {
                documentation.Add(softwareSystem, SectionType.Components, DocumentationFormat.Markdown, "Some Markdown content");
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Sections of type Components must be related to a container rather than a software system.", ae.Message);
            }
        }

        [TestMethod]
        public void Test_AddWithContentForContainer_AddsASectionWithTheSpecifiedContent_WhenThatSectionDoesNotExist()
        {
            documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");
            Section section = documentation.Add(container, DocumentationFormat.Markdown, "Some more Markdown content");

            Assert.AreSame(container, section.Element);
            Assert.AreEqual(container.Id, section.ElementId);
            Assert.AreEqual(SectionType.Components, section.Type);
            Assert.AreEqual(DocumentationFormat.Markdown, section.Format);
            Assert.AreEqual("Some more Markdown content", section.Content);

            Assert.AreEqual(2, documentation.Sections.Count);
            Assert.IsTrue(documentation.Sections.Contains(section));
        }

        [TestMethod]
        public void Test_AddWithContentForContainer_ThrowsAnException_WhenThatSectionAlreadyExists()
        {
            documentation.Add(container, DocumentationFormat.Markdown, "Some Markdown content");
            Assert.AreEqual(1, documentation.Sections.Count);

            try
            {
                documentation.Add(container, DocumentationFormat.Markdown, "Some Markdown content");
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                // this is the expected exception
                Assert.AreEqual("A section of type Components for Name already exists.", ae.Message);
                Assert.AreEqual(1, documentation.Sections.Count);
            }
        }

        [TestMethod]
        public void Test_AddFromFileForContainer_AddsASectionWithTheSpecifiedContent_WhenThatSectionDoesNotExist()
        {
            documentation.Add(softwareSystem, SectionType.Context, DocumentationFormat.Markdown, "Some Markdown content");

            FileInfo file = new FileInfo("..\\..\\Documentation\\example.md");
            Section section = documentation.Add(container, DocumentationFormat.Markdown, file);

            Assert.AreSame(container, section.Element);
            Assert.AreEqual(container.Id, section.ElementId);
            Assert.AreEqual(SectionType.Components, section.Type);
            Assert.AreEqual(DocumentationFormat.Markdown, section.Format);
            Assert.AreEqual("## Heading\r\n" +
                    "\r\n" +
                    "Here is a paragraph.", section.Content);

            Assert.AreEqual(2, documentation.Sections.Count);
            Assert.IsTrue(documentation.Sections.Contains(section));
        }

        [TestMethod]
        public void Test_AddFromFileForContainer_ThrowsAnException_WhenThatSectionAlreadyExists()
        {
            FileInfo file = new FileInfo("..\\..\\Documentation\\example.md");
            documentation.Add(container, DocumentationFormat.Markdown, file);
            Assert.AreEqual(1, documentation.Sections.Count);

            try
            {
                documentation.Add(container, DocumentationFormat.Markdown, file);
                Assert.Fail();
            }
            catch (ArgumentException ae)
            {
                // this is the expected exception
                Assert.AreEqual("A section of type Components for Name already exists.", ae.Message);
                Assert.AreEqual(1, documentation.Sections.Count);
            }
        }

        [TestMethod]
        public void Test_AddImages_DoesNothing_WhenThereAreNoImageFilesInTheSpecifiedDirectory()
        {
            documentation.AddImages(new FileInfo("..\\..\\Documentation\\noimages"));
            Assert.AreEqual(0, documentation.Images.Count);
        }

        [TestMethod]
        public void Test_AddImages_ThrowsAnException_WhenTheSpecifiedDirectoryIsNull()
        {
            try
            {
                documentation.AddImages(null);
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Directory path must not be null.", ae.Message);
            }
        }

        [TestMethod]
        public void Test_AddImages_ThrowsAnException_WhenTheSpecifiedDirectoryIsNotADirectory()
        {
            try
            {
                documentation.AddImages(new FileInfo("..\\..\\Documentation\\example.md"));
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.EndsWith("/structurizr-core/test/unit/com/structurizr/documentation/example.md is not a directory."));
            }
        }

        [TestMethod]
        public void Test_AddImages_ThrowsAnException_WhenTheSpecifiedDirectoryDoesNotExist()
        {
            try
            {
                documentation.AddImages(new FileInfo("..\\..\\Documentation\\blah"));
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.EndsWith("\\Documentation\\blah does not exist."));
            }
        }

        [TestMethod]
        public void Test_AddImages_AddsAllImagesFromTheSpecifiedDirectory_WhenThereAreImageFilesInTheSpecifiedDirectory()
        {
            Assert.AreEqual(0, documentation.Images.Count);
            documentation.AddImages(new FileInfo("..\\..\\Documentation\\images"));
            Assert.AreEqual(4, documentation.Images.Count);

            Image png = documentation.Images.Where(i => i.Name.Equals("image.png")).First();
            Assert.AreEqual("image/png", png.Type);
            Assert.AreEqual("iVBORw0KGgoAAAANSUhEUgAAACAAAAAaCAYAAADWm14/AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAAsTAAALEwEAmpwYAAAD1UlEQVRIS7VXSU9TURS+r9oytIW2lEJR1CARDSICxpUuNNForFgVEI1xYhTBISbqwnnCORoXzpo4AbpwgyKlQAtExRiHv2BMTBwCKis2n+fc1xIMdAQXX97t67nn++459557nhgcHERD4xMsLyyBSJ6NuLQcaFJm/xfEkm+RmoNlK4vx6HETmFs0ND6FEAJCmS4NFNvok8cD7FtnJxHaTMkpuVc4S+lHBtKz5lMEskedOJ4QtmxMmUlcYioczvX0TMlBDKsKi5zTMxaofpgrljnpnQgr7Pa50KTlQ5OaS5jje0YC3xz2wb7IJ3PKPTGC7B9QVGiCYrBCoZwpcfFQ4hOiQCKU2DjVh9HmE6FGPLgAJicH2vzNMKy9DuP6+4QHPjwcNg4H92FYcw3avI1Q9JahSAQWwOTGFGjnboC50gVLbQ8sNV5YdnTBTLASUmu7YK9jdAdEKv1vIzs5l3yYKluhzS2FkmCXHIEFpBXIkOmdV4m0W4owV7lhqmpDcrUbugoXxOaXEJtaQoBsylxyjkkupBv6VVekb94TQQTkQdEpMJbclerNRGyqbkfy9nbpcPpOD5afegfnuQ9YfZ7xcQT4P8eZ98jb1wOxzYUkEsERNBTfhjKRBNjzgqWABMRoYVx3jyapAmw+8gUHX+Nw83ec6fgdEvXtv3HS1Yeii58gtrxECqXDQItStCwgWApGCHCrq6eV7H3yBec8AzjR2iedhwILOdbyEzN2dWJCtQfmdVEI4PDFVboxra4Th5q/4bS7Xzo/1dYfEmzH9ouPvIEo74S19F6kAmjXkwBthRtZuz048vw7OfwVtgBGPdkvPd5LKewYgwCKgCrgR+QC2lnA2/ESEEUExldApBHoG6sA75CAGbuiiwDbj2EPqKcgngRMjfAUsA2D7RdFfwqGFSKqA3saP+O8dyBsAWc7/+Doix/IqOuArtoLU1h1QJZiQaX4zlAploWovA0F+3tw4NlXuSoOrR981EaO+yW5o/49xFZfJfSXYuII3JD4L6PCy/IC4cvIfxGJba1yvPDgKyw51itzOxqWEDjsvHIuw9Yq9VbVr7wke8LY9HlBWjIKj8Y0GROz18BU/ly9juk08FVsI0GJNV0QFR6Z06Agm5jtXlp5t3odlzVDmeVETNI0CE5B4KbU3w0lkYjViHdcpKbkBgxFN6EnJBTfRFLJLVhDgG1Mxbegp4ZG77hA5IVIz8gizkxqSok7eFuuitAkTlbbKYYyDP53oeCzZR6dZQrlfaYcNzQRt//DZFmwDxMWMalANhC8caJDvppzGqsfJo30YTKIvyz3JvYfp1uMAAAAAElFTkSuQmCC", png.Content);

            Image jpg = documentation.Images.Where(i => i.Name.Equals("image.jpg")).First();
            Assert.AreEqual("image/jpeg", jpg.Type);
            Assert.AreEqual("/9j/4AAQSkZJRgABAQEASABIAAD/4QCORXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAABIAAAAAQAAAEgAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAABoAAAAAAAD/2wBDAAICAgICAgMCAgMEAwMDBAUEBAQEBQcFBQUFBQcIBwcHBwcHCAgICAgICAgKCgoKCgoLCwsLCw0NDQ0NDQ0NDQ3/2wBDAQICAgMDAwYDAwYNCQcJDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ3/wAARCAAaACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/90ABAAC/9oADAMBAAIRAxEAPwD8cvGXjLxL4/8AEuo+MPF+o3Gp6rqlxLdXNzcyvKxaV2cgF2baibsIgwqqAAPW1Z/D3xxqFtHeWei3DwzKHjYtEm5TyCFeQMAfcCuIuebaYf8ATN//AEE1+uXwI8IaJ4w8TrZa9B9ptLTTjceQSVWRwURQ2CCVGScdziv2HhbIcLj41pYiUoxpqL923W/dPsflfEmd4nBSoxoRUpVG/iv0t2a7n5of8Ku+If8A0Arj/v5B/wDHau6NJ8Tfg94g0zxvpi3+gX+nXcM1rewzbcSxuHCMYZDlX24ZH+V1JFfvL/wp74Yf9C3Y/wDfDf8AxVfn1+2z4M0HwdpHk+H7cWttepZ3BgUkokiXaISmSSAwI46A9OtetjOGMs+q1Z0pz5oxlJX5bOyvZ2SZ52F4izH6zSjUjDllJJ25rq7tdXfmf//Q/Eq8gniSe3kjZZVWRCjDawdcqRg4wQwII7EYNfqX8Efif4f8IaxB4huJBd6fe2H2d2t3RpE3bGVgpYZwVwy9efavn39vfQ9F8P8A7UnjWx0HT7XTbZrsztDaQpBGZZmdpHKoFG525ZupPJr43a0tJPneGNmbqSgJP44r9RyDib+zadSUqXOqqV1e1rXe9n37H51nvDn1+rBRqcrpttO173t0uux+/H/DS/ww/wCet7/4Dj/4uvhX9sD4k6N8R7CJNCV8A2dpaxPtM87faVkdhGhYgdAB1J9yAfzv+w2P/PvF/wB8L/hX2V+wR4c8Paz+1F4KstX0uyvrcXizCK5t45o/NhZGjfa6kbkblTjIPIrsxXG1KWGq0qeHs5Rkrud7XVnpyroc2G4QqRxFOdSvdRknZRtezuteZ9T/2Q==", jpg.Content);

            Image jpeg = documentation.Images.Where(i => i.Name.Equals("image.jpeg")).First();
            Assert.AreEqual("image/jpeg", jpeg.Type);
            Assert.AreEqual("/9j/4AAQSkZJRgABAQEASABIAAD/4QCORXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAABIAAAAAQAAAEgAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAABoAAAAAAAD/2wBDAAICAgICAgMCAgMEAwMDBAUEBAQEBQcFBQUFBQcIBwcHBwcHCAgICAgICAgKCgoKCgoLCwsLCw0NDQ0NDQ0NDQ3/2wBDAQICAgMDAwYDAwYNCQcJDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ3/wAARCAAaACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/90ABAAC/9oADAMBAAIRAxEAPwD8cvGXjLxL4/8AEuo+MPF+o3Gp6rqlxLdXNzcyvKxaV2cgF2baibsIgwqqAAPW1Z/D3xxqFtHeWei3DwzKHjYtEm5TyCFeQMAfcCuIuebaYf8ATN//AEE1+uXwI8IaJ4w8TrZa9B9ptLTTjceQSVWRwURQ2CCVGScdziv2HhbIcLj41pYiUoxpqL923W/dPsflfEmd4nBSoxoRUpVG/iv0t2a7n5of8Ku+If8A0Arj/v5B/wDHau6NJ8Tfg94g0zxvpi3+gX+nXcM1rewzbcSxuHCMYZDlX24ZH+V1JFfvL/wp74Yf9C3Y/wDfDf8AxVfn1+2z4M0HwdpHk+H7cWttepZ3BgUkokiXaISmSSAwI46A9OtetjOGMs+q1Z0pz5oxlJX5bOyvZ2SZ52F4izH6zSjUjDllJJ25rq7tdXfmf//Q/Eq8gniSe3kjZZVWRCjDawdcqRg4wQwII7EYNfqX8Efif4f8IaxB4huJBd6fe2H2d2t3RpE3bGVgpYZwVwy9efavn39vfQ9F8P8A7UnjWx0HT7XTbZrsztDaQpBGZZmdpHKoFG525ZupPJr43a0tJPneGNmbqSgJP44r9RyDib+zadSUqXOqqV1e1rXe9n37H51nvDn1+rBRqcrpttO173t0uux+/H/DS/ww/wCet7/4Dj/4uvhX9sD4k6N8R7CJNCV8A2dpaxPtM87faVkdhGhYgdAB1J9yAfzv+w2P/PvF/wB8L/hX2V+wR4c8Paz+1F4KstX0uyvrcXizCK5t45o/NhZGjfa6kbkblTjIPIrsxXG1KWGq0qeHs5Rkrud7XVnpyroc2G4QqRxFOdSvdRknZRtezuteZ9T/2Q==", jpeg.Content);

            Image gif = documentation.Images.Where(i => i.Name.Equals("image.gif")).First();
            Assert.AreEqual("image/gif", gif.Type);
            Assert.AreEqual("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", gif.Content);
        }

        [TestMethod]
        public void Test_AddImages_AddsAllImagesFromTheSpecifiedDirectory_WhenThereAreImageFilesInTheSpecifiedDirectoryAndSubDirectories()
        {
            Assert.AreEqual(0, documentation.Images.Count);
            documentation.AddImages(new FileInfo("..\\..\\Documentation"));
            Assert.AreEqual(8, documentation.Images.Count);

            Image pngInDirectory = documentation.Images.Where(i => i.Name.Equals("image.png")).First();
            Assert.AreEqual("image/png", pngInDirectory.Type);
            Assert.AreEqual("iVBORw0KGgoAAAANSUhEUgAAACAAAAAaCAYAAADWm14/AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAAsTAAALEwEAmpwYAAAD1UlEQVRIS7VXSU9TURS+r9oytIW2lEJR1CARDSICxpUuNNForFgVEI1xYhTBISbqwnnCORoXzpo4AbpwgyKlQAtExRiHv2BMTBwCKis2n+fc1xIMdAQXX97t67nn++459557nhgcHERD4xMsLyyBSJ6NuLQcaFJm/xfEkm+RmoNlK4vx6HETmFs0ND6FEAJCmS4NFNvok8cD7FtnJxHaTMkpuVc4S+lHBtKz5lMEskedOJ4QtmxMmUlcYioczvX0TMlBDKsKi5zTMxaofpgrljnpnQgr7Pa50KTlQ5OaS5jje0YC3xz2wb7IJ3PKPTGC7B9QVGiCYrBCoZwpcfFQ4hOiQCKU2DjVh9HmE6FGPLgAJicH2vzNMKy9DuP6+4QHPjwcNg4H92FYcw3avI1Q9JahSAQWwOTGFGjnboC50gVLbQ8sNV5YdnTBTLASUmu7YK9jdAdEKv1vIzs5l3yYKluhzS2FkmCXHIEFpBXIkOmdV4m0W4owV7lhqmpDcrUbugoXxOaXEJtaQoBsylxyjkkupBv6VVekb94TQQTkQdEpMJbclerNRGyqbkfy9nbpcPpOD5afegfnuQ9YfZ7xcQT4P8eZ98jb1wOxzYUkEsERNBTfhjKRBNjzgqWABMRoYVx3jyapAmw+8gUHX+Nw83ec6fgdEvXtv3HS1Yeii58gtrxECqXDQItStCwgWApGCHCrq6eV7H3yBec8AzjR2iedhwILOdbyEzN2dWJCtQfmdVEI4PDFVboxra4Th5q/4bS7Xzo/1dYfEmzH9ouPvIEo74S19F6kAmjXkwBthRtZuz048vw7OfwVtgBGPdkvPd5LKewYgwCKgCrgR+QC2lnA2/ESEEUExldApBHoG6sA75CAGbuiiwDbj2EPqKcgngRMjfAUsA2D7RdFfwqGFSKqA3saP+O8dyBsAWc7/+Doix/IqOuArtoLU1h1QJZiQaX4zlAploWovA0F+3tw4NlXuSoOrR981EaO+yW5o/49xFZfJfSXYuII3JD4L6PCy/IC4cvIfxGJba1yvPDgKyw51itzOxqWEDjsvHIuw9Yq9VbVr7wke8LY9HlBWjIKj8Y0GROz18BU/ly9juk08FVsI0GJNV0QFR6Z06Agm5jtXlp5t3odlzVDmeVETNI0CE5B4KbU3w0lkYjViHdcpKbkBgxFN6EnJBTfRFLJLVhDgG1Mxbegp4ZG77hA5IVIz8gizkxqSok7eFuuitAkTlbbKYYyDP53oeCzZR6dZQrlfaYcNzQRt//DZFmwDxMWMalANhC8caJDvppzGqsfJo30YTKIvyz3JvYfp1uMAAAAAElFTkSuQmCC", pngInDirectory.Content);

            Image jpgInDirectory = documentation.Images.Where(i => i.Name.Equals("image.jpg")).First();
            Assert.AreEqual("image/jpeg", jpgInDirectory.Type);
            Assert.AreEqual("/9j/4AAQSkZJRgABAQEASABIAAD/4QCORXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAABIAAAAAQAAAEgAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAABoAAAAAAAD/2wBDAAICAgICAgMCAgMEAwMDBAUEBAQEBQcFBQUFBQcIBwcHBwcHCAgICAgICAgKCgoKCgoLCwsLCw0NDQ0NDQ0NDQ3/2wBDAQICAgMDAwYDAwYNCQcJDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ3/wAARCAAaACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/90ABAAC/9oADAMBAAIRAxEAPwD8cvGXjLxL4/8AEuo+MPF+o3Gp6rqlxLdXNzcyvKxaV2cgF2baibsIgwqqAAPW1Z/D3xxqFtHeWei3DwzKHjYtEm5TyCFeQMAfcCuIuebaYf8ATN//AEE1+uXwI8IaJ4w8TrZa9B9ptLTTjceQSVWRwURQ2CCVGScdziv2HhbIcLj41pYiUoxpqL923W/dPsflfEmd4nBSoxoRUpVG/iv0t2a7n5of8Ku+If8A0Arj/v5B/wDHau6NJ8Tfg94g0zxvpi3+gX+nXcM1rewzbcSxuHCMYZDlX24ZH+V1JFfvL/wp74Yf9C3Y/wDfDf8AxVfn1+2z4M0HwdpHk+H7cWttepZ3BgUkokiXaISmSSAwI46A9OtetjOGMs+q1Z0pz5oxlJX5bOyvZ2SZ52F4izH6zSjUjDllJJ25rq7tdXfmf//Q/Eq8gniSe3kjZZVWRCjDawdcqRg4wQwII7EYNfqX8Efif4f8IaxB4huJBd6fe2H2d2t3RpE3bGVgpYZwVwy9efavn39vfQ9F8P8A7UnjWx0HT7XTbZrsztDaQpBGZZmdpHKoFG525ZupPJr43a0tJPneGNmbqSgJP44r9RyDib+zadSUqXOqqV1e1rXe9n37H51nvDn1+rBRqcrpttO173t0uux+/H/DS/ww/wCet7/4Dj/4uvhX9sD4k6N8R7CJNCV8A2dpaxPtM87faVkdhGhYgdAB1J9yAfzv+w2P/PvF/wB8L/hX2V+wR4c8Paz+1F4KstX0uyvrcXizCK5t45o/NhZGjfa6kbkblTjIPIrsxXG1KWGq0qeHs5Rkrud7XVnpyroc2G4QqRxFOdSvdRknZRtezuteZ9T/2Q==", jpgInDirectory.Content);

            Image jpegInDirectory = documentation.Images.Where(i => i.Name.Equals("image.jpeg")).First();
            Assert.AreEqual("image/jpeg", jpegInDirectory.Type);
            Assert.AreEqual("/9j/4AAQSkZJRgABAQEASABIAAD/4QCORXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAABIAAAAAQAAAEgAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAABoAAAAAAAD/2wBDAAICAgICAgMCAgMEAwMDBAUEBAQEBQcFBQUFBQcIBwcHBwcHCAgICAgICAgKCgoKCgoLCwsLCw0NDQ0NDQ0NDQ3/2wBDAQICAgMDAwYDAwYNCQcJDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ3/wAARCAAaACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/90ABAAC/9oADAMBAAIRAxEAPwD8cvGXjLxL4/8AEuo+MPF+o3Gp6rqlxLdXNzcyvKxaV2cgF2baibsIgwqqAAPW1Z/D3xxqFtHeWei3DwzKHjYtEm5TyCFeQMAfcCuIuebaYf8ATN//AEE1+uXwI8IaJ4w8TrZa9B9ptLTTjceQSVWRwURQ2CCVGScdziv2HhbIcLj41pYiUoxpqL923W/dPsflfEmd4nBSoxoRUpVG/iv0t2a7n5of8Ku+If8A0Arj/v5B/wDHau6NJ8Tfg94g0zxvpi3+gX+nXcM1rewzbcSxuHCMYZDlX24ZH+V1JFfvL/wp74Yf9C3Y/wDfDf8AxVfn1+2z4M0HwdpHk+H7cWttepZ3BgUkokiXaISmSSAwI46A9OtetjOGMs+q1Z0pz5oxlJX5bOyvZ2SZ52F4izH6zSjUjDllJJ25rq7tdXfmf//Q/Eq8gniSe3kjZZVWRCjDawdcqRg4wQwII7EYNfqX8Efif4f8IaxB4huJBd6fe2H2d2t3RpE3bGVgpYZwVwy9efavn39vfQ9F8P8A7UnjWx0HT7XTbZrsztDaQpBGZZmdpHKoFG525ZupPJr43a0tJPneGNmbqSgJP44r9RyDib+zadSUqXOqqV1e1rXe9n37H51nvDn1+rBRqcrpttO173t0uux+/H/DS/ww/wCet7/4Dj/4uvhX9sD4k6N8R7CJNCV8A2dpaxPtM87faVkdhGhYgdAB1J9yAfzv+w2P/PvF/wB8L/hX2V+wR4c8Paz+1F4KstX0uyvrcXizCK5t45o/NhZGjfa6kbkblTjIPIrsxXG1KWGq0qeHs5Rkrud7XVnpyroc2G4QqRxFOdSvdRknZRtezuteZ9T/2Q==", jpegInDirectory.Content);

            Image gifInDirectory = documentation.Images.Where(i => i.Name.Equals("image.gif")).First();
            Assert.AreEqual("image/gif", gifInDirectory.Type);
            Assert.AreEqual("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", gifInDirectory.Content);

            Image pngInSubdirectory = documentation.Images.Where(i => i.Name.Equals("image.png")).First();
            Assert.AreEqual("image/png", pngInSubdirectory.Type);
            Assert.AreEqual("iVBORw0KGgoAAAANSUhEUgAAACAAAAAaCAYAAADWm14/AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAAsTAAALEwEAmpwYAAAD1UlEQVRIS7VXSU9TURS+r9oytIW2lEJR1CARDSICxpUuNNForFgVEI1xYhTBISbqwnnCORoXzpo4AbpwgyKlQAtExRiHv2BMTBwCKis2n+fc1xIMdAQXX97t67nn++459557nhgcHERD4xMsLyyBSJ6NuLQcaFJm/xfEkm+RmoNlK4vx6HETmFs0ND6FEAJCmS4NFNvok8cD7FtnJxHaTMkpuVc4S+lHBtKz5lMEskedOJ4QtmxMmUlcYioczvX0TMlBDKsKi5zTMxaofpgrljnpnQgr7Pa50KTlQ5OaS5jje0YC3xz2wb7IJ3PKPTGC7B9QVGiCYrBCoZwpcfFQ4hOiQCKU2DjVh9HmE6FGPLgAJicH2vzNMKy9DuP6+4QHPjwcNg4H92FYcw3avI1Q9JahSAQWwOTGFGjnboC50gVLbQ8sNV5YdnTBTLASUmu7YK9jdAdEKv1vIzs5l3yYKluhzS2FkmCXHIEFpBXIkOmdV4m0W4owV7lhqmpDcrUbugoXxOaXEJtaQoBsylxyjkkupBv6VVekb94TQQTkQdEpMJbclerNRGyqbkfy9nbpcPpOD5afegfnuQ9YfZ7xcQT4P8eZ98jb1wOxzYUkEsERNBTfhjKRBNjzgqWABMRoYVx3jyapAmw+8gUHX+Nw83ec6fgdEvXtv3HS1Yeii58gtrxECqXDQItStCwgWApGCHCrq6eV7H3yBec8AzjR2iedhwILOdbyEzN2dWJCtQfmdVEI4PDFVboxra4Th5q/4bS7Xzo/1dYfEmzH9ouPvIEo74S19F6kAmjXkwBthRtZuz048vw7OfwVtgBGPdkvPd5LKewYgwCKgCrgR+QC2lnA2/ESEEUExldApBHoG6sA75CAGbuiiwDbj2EPqKcgngRMjfAUsA2D7RdFfwqGFSKqA3saP+O8dyBsAWc7/+Doix/IqOuArtoLU1h1QJZiQaX4zlAploWovA0F+3tw4NlXuSoOrR981EaO+yW5o/49xFZfJfSXYuII3JD4L6PCy/IC4cvIfxGJba1yvPDgKyw51itzOxqWEDjsvHIuw9Yq9VbVr7wke8LY9HlBWjIKj8Y0GROz18BU/ly9juk08FVsI0GJNV0QFR6Z06Agm5jtXlp5t3odlzVDmeVETNI0CE5B4KbU3w0lkYjViHdcpKbkBgxFN6EnJBTfRFLJLVhDgG1Mxbegp4ZG77hA5IVIz8gizkxqSok7eFuuitAkTlbbKYYyDP53oeCzZR6dZQrlfaYcNzQRt//DZFmwDxMWMalANhC8caJDvppzGqsfJo30YTKIvyz3JvYfp1uMAAAAAElFTkSuQmCC", pngInSubdirectory.Content);

            Image jpgInSubdirectory = documentation.Images.Where(i => i.Name.Equals("image.jpg")).First();
            Assert.AreEqual("image/jpeg", jpgInSubdirectory.Type);
            Assert.AreEqual("/9j/4AAQSkZJRgABAQEASABIAAD/4QCORXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAABIAAAAAQAAAEgAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAABoAAAAAAAD/2wBDAAICAgICAgMCAgMEAwMDBAUEBAQEBQcFBQUFBQcIBwcHBwcHCAgICAgICAgKCgoKCgoLCwsLCw0NDQ0NDQ0NDQ3/2wBDAQICAgMDAwYDAwYNCQcJDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ3/wAARCAAaACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/90ABAAC/9oADAMBAAIRAxEAPwD8cvGXjLxL4/8AEuo+MPF+o3Gp6rqlxLdXNzcyvKxaV2cgF2baibsIgwqqAAPW1Z/D3xxqFtHeWei3DwzKHjYtEm5TyCFeQMAfcCuIuebaYf8ATN//AEE1+uXwI8IaJ4w8TrZa9B9ptLTTjceQSVWRwURQ2CCVGScdziv2HhbIcLj41pYiUoxpqL923W/dPsflfEmd4nBSoxoRUpVG/iv0t2a7n5of8Ku+If8A0Arj/v5B/wDHau6NJ8Tfg94g0zxvpi3+gX+nXcM1rewzbcSxuHCMYZDlX24ZH+V1JFfvL/wp74Yf9C3Y/wDfDf8AxVfn1+2z4M0HwdpHk+H7cWttepZ3BgUkokiXaISmSSAwI46A9OtetjOGMs+q1Z0pz5oxlJX5bOyvZ2SZ52F4izH6zSjUjDllJJ25rq7tdXfmf//Q/Eq8gniSe3kjZZVWRCjDawdcqRg4wQwII7EYNfqX8Efif4f8IaxB4huJBd6fe2H2d2t3RpE3bGVgpYZwVwy9efavn39vfQ9F8P8A7UnjWx0HT7XTbZrsztDaQpBGZZmdpHKoFG525ZupPJr43a0tJPneGNmbqSgJP44r9RyDib+zadSUqXOqqV1e1rXe9n37H51nvDn1+rBRqcrpttO173t0uux+/H/DS/ww/wCet7/4Dj/4uvhX9sD4k6N8R7CJNCV8A2dpaxPtM87faVkdhGhYgdAB1J9yAfzv+w2P/PvF/wB8L/hX2V+wR4c8Paz+1F4KstX0uyvrcXizCK5t45o/NhZGjfa6kbkblTjIPIrsxXG1KWGq0qeHs5Rkrud7XVnpyroc2G4QqRxFOdSvdRknZRtezuteZ9T/2Q==", jpgInSubdirectory.Content);

            Image jpegInSubdirectory = documentation.Images.Where(i => i.Name.Equals("image.jpeg")).First();
            Assert.AreEqual("image/jpeg", jpegInSubdirectory.Type);
            Assert.AreEqual("/9j/4AAQSkZJRgABAQEASABIAAD/4QCORXhpZgAATU0AKgAAAAgABQESAAMAAAABAAEAAAEaAAUAAAABAAAASgEbAAUAAAABAAAAUgEoAAMAAAABAAIAAIdpAAQAAAABAAAAWgAAAAAAAABIAAAAAQAAAEgAAAABAAOgAQADAAAAAQABAACgAgAEAAAAAQAAACCgAwAEAAAAAQAAABoAAAAAAAD/2wBDAAICAgICAgMCAgMEAwMDBAUEBAQEBQcFBQUFBQcIBwcHBwcHCAgICAgICAgKCgoKCgoLCwsLCw0NDQ0NDQ0NDQ3/2wBDAQICAgMDAwYDAwYNCQcJDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ0NDQ3/wAARCAAaACADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/90ABAAC/9oADAMBAAIRAxEAPwD8cvGXjLxL4/8AEuo+MPF+o3Gp6rqlxLdXNzcyvKxaV2cgF2baibsIgwqqAAPW1Z/D3xxqFtHeWei3DwzKHjYtEm5TyCFeQMAfcCuIuebaYf8ATN//AEE1+uXwI8IaJ4w8TrZa9B9ptLTTjceQSVWRwURQ2CCVGScdziv2HhbIcLj41pYiUoxpqL923W/dPsflfEmd4nBSoxoRUpVG/iv0t2a7n5of8Ku+If8A0Arj/v5B/wDHau6NJ8Tfg94g0zxvpi3+gX+nXcM1rewzbcSxuHCMYZDlX24ZH+V1JFfvL/wp74Yf9C3Y/wDfDf8AxVfn1+2z4M0HwdpHk+H7cWttepZ3BgUkokiXaISmSSAwI46A9OtetjOGMs+q1Z0pz5oxlJX5bOyvZ2SZ52F4izH6zSjUjDllJJ25rq7tdXfmf//Q/Eq8gniSe3kjZZVWRCjDawdcqRg4wQwII7EYNfqX8Efif4f8IaxB4huJBd6fe2H2d2t3RpE3bGVgpYZwVwy9efavn39vfQ9F8P8A7UnjWx0HT7XTbZrsztDaQpBGZZmdpHKoFG525ZupPJr43a0tJPneGNmbqSgJP44r9RyDib+zadSUqXOqqV1e1rXe9n37H51nvDn1+rBRqcrpttO173t0uux+/H/DS/ww/wCet7/4Dj/4uvhX9sD4k6N8R7CJNCV8A2dpaxPtM87faVkdhGhYgdAB1J9yAfzv+w2P/PvF/wB8L/hX2V+wR4c8Paz+1F4KstX0uyvrcXizCK5t45o/NhZGjfa6kbkblTjIPIrsxXG1KWGq0qeHs5Rkrud7XVnpyroc2G4QqRxFOdSvdRknZRtezuteZ9T/2Q==", jpegInSubdirectory.Content);

            Image gifInSubdirectory = documentation.Images.Where(i => i.Name.Equals("image.gif")).First();
            Assert.AreEqual("image/gif", gifInSubdirectory.Type);
            Assert.AreEqual("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", gifInSubdirectory.Content);
        }

        [TestMethod]
        public void Test_AddImage_AddsTheSpecifiedImage_WhenTheSpecifiedFileExists()
        {
            Assert.AreEqual(0, documentation.Images.Count);
            documentation.AddImage(new FileInfo("..\\..\\Documentation\\image.png"));
            Assert.AreEqual(1, documentation.Images.Count);

            Image png = documentation.Images.Where(i => i.Name.Equals("image.png")).First();
            Assert.AreEqual("image/png", png.Type);
            Assert.AreEqual("iVBORw0KGgoAAAANSUhEUgAAACAAAAAaCAYAAADWm14/AAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAACXBIWXMAAAsTAAALEwEAmpwYAAAD1UlEQVRIS7VXSU9TURS+r9oytIW2lEJR1CARDSICxpUuNNForFgVEI1xYhTBISbqwnnCORoXzpo4AbpwgyKlQAtExRiHv2BMTBwCKis2n+fc1xIMdAQXX97t67nn++459557nhgcHERD4xMsLyyBSJ6NuLQcaFJm/xfEkm+RmoNlK4vx6HETmFs0ND6FEAJCmS4NFNvok8cD7FtnJxHaTMkpuVc4S+lHBtKz5lMEskedOJ4QtmxMmUlcYioczvX0TMlBDKsKi5zTMxaofpgrljnpnQgr7Pa50KTlQ5OaS5jje0YC3xz2wb7IJ3PKPTGC7B9QVGiCYrBCoZwpcfFQ4hOiQCKU2DjVh9HmE6FGPLgAJicH2vzNMKy9DuP6+4QHPjwcNg4H92FYcw3avI1Q9JahSAQWwOTGFGjnboC50gVLbQ8sNV5YdnTBTLASUmu7YK9jdAdEKv1vIzs5l3yYKluhzS2FkmCXHIEFpBXIkOmdV4m0W4owV7lhqmpDcrUbugoXxOaXEJtaQoBsylxyjkkupBv6VVekb94TQQTkQdEpMJbclerNRGyqbkfy9nbpcPpOD5afegfnuQ9YfZ7xcQT4P8eZ98jb1wOxzYUkEsERNBTfhjKRBNjzgqWABMRoYVx3jyapAmw+8gUHX+Nw83ec6fgdEvXtv3HS1Yeii58gtrxECqXDQItStCwgWApGCHCrq6eV7H3yBec8AzjR2iedhwILOdbyEzN2dWJCtQfmdVEI4PDFVboxra4Th5q/4bS7Xzo/1dYfEmzH9ouPvIEo74S19F6kAmjXkwBthRtZuz048vw7OfwVtgBGPdkvPd5LKewYgwCKgCrgR+QC2lnA2/ESEEUExldApBHoG6sA75CAGbuiiwDbj2EPqKcgngRMjfAUsA2D7RdFfwqGFSKqA3saP+O8dyBsAWc7/+Doix/IqOuArtoLU1h1QJZiQaX4zlAploWovA0F+3tw4NlXuSoOrR981EaO+yW5o/49xFZfJfSXYuII3JD4L6PCy/IC4cvIfxGJba1yvPDgKyw51itzOxqWEDjsvHIuw9Yq9VbVr7wke8LY9HlBWjIKj8Y0GROz18BU/ly9juk08FVsI0GJNV0QFR6Z06Agm5jtXlp5t3odlzVDmeVETNI0CE5B4KbU3w0lkYjViHdcpKbkBgxFN6EnJBTfRFLJLVhDgG1Mxbegp4ZG77hA5IVIz8gizkxqSok7eFuuitAkTlbbKYYyDP53oeCzZR6dZQrlfaYcNzQRt//DZFmwDxMWMalANhC8caJDvppzGqsfJo30YTKIvyz3JvYfp1uMAAAAAElFTkSuQmCC", png.Content);
        }

        [TestMethod]
        public void Test_AddImage_ThrowsAnException_WhenTheSpecifiedFileIsNull()
        {
            try
            {
                documentation.AddImage(null);
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("File must not be null.", ae.Message);
            }
        }

        [TestMethod]
        public void Test_AddImage_ThrowsAnException_WhenTheSpecifiedFileIsNotAFile()
        {
            try
            {
                documentation.AddImage(new FileInfo("..\\..\\Documentation"));
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.EndsWith("\\Documentation is not a file."));
            }
        }

        [TestMethod]
        public void test_addImage_ThrowsAnException_WhenTheSpecifiedFileDoesNotExist()
        {
            try
            {
                documentation.AddImage(new FileInfo("..\\..\\Documentation\\some-other-image.png"));
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.EndsWith("\\Documentation\\some-other-image.png does not exist."));
            }
        }

        [TestMethod]
        public void test_addImage_ThrowsAnException_WhenTheSpecifiedFileIsNotAnImage()
        {
            try
            {
                documentation.AddImage(new FileInfo("..\\..\\Documentation\\example.md"));
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.EndsWith("\\Documentation\\example.md is not a supported image file."));
            }
        }

    }
}
