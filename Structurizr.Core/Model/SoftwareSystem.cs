using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A software system.
    /// </summary>
    [DataContract]
    public sealed class SoftwareSystem : StaticStructureElement, IEquatable<SoftwareSystem>
    {

        /// <summary>
        /// The location of this software system.
        /// </summary>
        [DataMember(Name="location", EmitDefaultValue=true)]
        public Location Location { get; set; }

        private HashSet<Container> _containers;

        /// <summary>
        /// The set of containers within this software system.
        /// </summary>
        [DataMember(Name="containers", EmitDefaultValue=false)]
        public ISet<Container> Containers
        {
            get
            {
                return new HashSet<Container>(_containers);
            }

            internal set
            {
                _containers = new HashSet<Container>(value);
            }
        }
  
        public override string CanonicalName
        {
            get
            {
                return new CanonicalNameGenerator().Generate(this);
            }
        }

        public override Element Parent
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        internal SoftwareSystem()
        {
            _containers = new HashSet<Container>();
        }

        /// <summary>
        /// Adds a container with the specified name, description and technology
        /// (unless one exists with the same name already).
        /// </summary>
        /// <param name="name">the name of the container (e.g. "Web Application")</param>
        /// <param name="description">a short description/list of responsibilities</param>
        /// <param name="technology">the technoogy choice (e.g. "Spring MVC", "Java EE", etc)</param>
        public Container AddContainer(String name, String description, String technology)
        {
            return Model.AddContainer(this, name, description, technology);
        }

        internal void Add(Container container)
        {
            _containers.Add(container);
        }

        /// <summary>
        /// Gets the container with the specified name (or null if it doesn't exist).
        /// </summary>
        public Container GetContainerWithName(string name)
        {
            foreach (Container container in _containers)
            {
                if (container.Name == name)
                {
                    return container;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the container with the specified ID (or null if it doesn't exist).
        /// </summary>
        public Container GetContainerWithId(string id)
        {
            foreach (Container container in _containers)
            {
                if (container.Id == id)
                {
                    return container;
                }
            }

            return null;
        }

        public override List<string> GetRequiredTags()
        {
            return new List<string>
            {
                Structurizr.Tags.Element,
                Structurizr.Tags.SoftwareSystem
            };
        }

        public bool Equals(SoftwareSystem softwareSystem)
        {
            return this.Equals(softwareSystem as Element);
        }

    }
}