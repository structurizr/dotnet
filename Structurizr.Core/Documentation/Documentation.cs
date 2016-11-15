using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Structurizr
{
    [DataContract]
    public class Documentation
    {

        public Model Model { get; set; }

        [DataMember(Name = "sections", EmitDefaultValue = false)]
        public HashSet<Section> Sections { get; private set; }

        [DataMember(Name = "images", EmitDefaultValue = false)]
        public HashSet<Image> Images { get; private set; }

        internal Documentation() {
            this.Sections = new HashSet<Section>();
            this.Images = new HashSet<Image>();
        }

        internal Documentation(Model model) : this()
        {
            this.Model = model;
        }

        public void Hydrate()
        {
            foreach (Section section in Sections)
            {
                section.Element = Model.GetElement(section.ElementId);
            }

        }

        public Section Add(SoftwareSystem softwareSystem, SectionType type, DocumentationFormat format, FileInfo fileInfo)
        {
            string content = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
            return Add(softwareSystem, type, format, content);
        }

        public Section Add(SoftwareSystem softwareSystem, SectionType type, DocumentationFormat format, string content)
        {
            return AddSection(softwareSystem, type, format, content);

        }

        public Section Add(Container container, DocumentationFormat format, FileInfo fileInfo)
        {
            string content = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
            return Add(container, format, content);
        }

        public Section Add(Container container, DocumentationFormat format, string content)
        {
            return AddSection(container, SectionType.Components, format, content);
        }

        private Section AddSection(Element element, SectionType type, DocumentationFormat format, string content)
        {
            if (!(element is Container) && type == SectionType.Components)
            {
                throw new ArgumentException("Sections of type Components must be related to a container rather than a software system.");
            }


            Section section = new Section(element, type, format, content);
            if (!Sections.Contains(section))
            {
                Sections.Add(section);
                return section;
            } else
            {
                throw new ArgumentException("A section of type " + type + " for " + element.Name + " already exists.");
            }
        }

        public void AddImages(FileInfo directory)
        {
            if (directory == null)
            {
                throw new ArgumentException("Directory path must not be null.");
            }
            else if (Directory.Exists(directory.FullName))
            {
                AddImagesFromPath("", directory);
            }
        }

        private void AddImagesFromPath(string root, FileInfo directory)
        {
            foreach (string fileName in Directory.EnumerateFiles(directory.FullName, "*.png", SearchOption.TopDirectoryOnly))
            {
                AddImage(new FileInfo(fileName));
            }
            foreach (string fileName in Directory.EnumerateFiles(directory.FullName, "*.jpg", SearchOption.TopDirectoryOnly))
            {
                AddImage(new FileInfo(fileName));
            }
            foreach (string fileName in Directory.EnumerateFiles(directory.FullName, "*.jpeg", SearchOption.TopDirectoryOnly))
            {
                AddImage(new FileInfo(fileName));
            }
            foreach (string fileName in Directory.EnumerateFiles(directory.FullName, "*.gif", SearchOption.TopDirectoryOnly))
            {
                AddImage(new FileInfo(fileName));
            }

            foreach (string directoryName in Directory.EnumerateDirectories(directory.FullName))
            {
                AddImagesFromPath(new FileInfo(directoryName).Name + "/", new FileInfo(directoryName));
            }
        }

        public void AddImage(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentException("File must not be null.");
            }
            else if (Directory.Exists(file.FullName))
            {
                throw new ArgumentException(file.FullName + " is not a file.");
            }
            else if (!File.Exists(file.FullName))
            {
                throw new ArgumentException(file.FullName + " does not exist.");
            }
            else if (
                !file.FullName.ToLower().EndsWith(".png") &&
                !file.FullName.ToLower().EndsWith(".jpg") &&
                !file.FullName.ToLower().EndsWith(".jpeg") &&
                !file.FullName.ToLower().EndsWith(".gif")
                )
            {
                throw new ArgumentException(file.FullName + " is not a supported image file.");
            }

            string contentType = file.FullName.Substring(file.FullName.LastIndexOf(".")+1).ToLower();
            if (contentType.Equals("jpg"))
            {
                contentType = "jpeg";
            }

            using (System.Drawing.Image image = System.Drawing.Image.FromFile(file.FullName))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);

                    Images.Add(new Image(file.Name, base64String, "image/" + contentType));
                }
            }
        }

    }


}

