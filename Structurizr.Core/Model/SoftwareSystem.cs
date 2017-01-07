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
    public class SoftwareSystem : Element, IEquatable<SoftwareSystem>
    {

        /// <summary>
        /// The location of this software system.
        /// </summary>
        [DataMember(Name="location", EmitDefaultValue=false)]
        public Location Location { get; set; }
  
        
        /// <summary>
        /// The set of containers within this software system.
        /// </summary>
        [DataMember(Name="containers", EmitDefaultValue=false)]
        public HashSet<Container> Containers { get; set; }
  
        public override string CanonicalName
        {
            get
            {
                return CanonicalNameSeparator + FormatForCanonicalName(Name);
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

        public SoftwareSystem()
        {
            this.Containers = new HashSet<Container>();
        }

        /// <summary>
        /// Adds a unidirectional relationship between this software system and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        public Relationship Delivers(Person destination, string description)
        {
            Relationship relationship = new Relationship(this, destination, description);
            Model.AddRelationship(relationship);

            return relationship;
        }

        /// <summary>
        /// Adds a unidirectional relationship between this software system and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        public Relationship Delivers(Person destination, string description, string technology)
        {
            Relationship relationship = new Relationship(this, destination, description, technology);
            Model.AddRelationship(relationship);

            return relationship;
        }

        /// <summary>
        /// Adds a unidirectional relationship between this software system and a person.
        /// </summary>
        /// <param name="destination">the target of the relationship</param>
        /// <param name="description">a description of the relationship (e.g. "sends e-mail to")</param>
        /// <param name="technology">the technology details (e.g. JSON/HTTPS)</param>
        /// <param name="interactionStyle">the interaction style (sync vs async)</param>
        public Relationship Delivers(Person destination, string description, string technology, InteractionStyle interactionStyle)
        {
            Relationship relationship = new Relationship(this, destination, description, technology, interactionStyle);
            Model.AddRelationship(relationship);

            return relationship;
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

        /// <summary>
        /// Adds a container
        /// (unless one exists with the same name already).
        /// </summary>
        /// <param name="container">the container to be added</param>
        public Container AddContainer(Container container)
        {
            return Model.AddContainer(this, container);
        }

        internal void Add(Container container)
        {
            Containers.Add(container);
        }

        /// <summary>
        /// Gets the container with the specified name (or null if it doesn't exist).
        /// </summary>
        public Container GetContainerWithName(string name)
        {
            foreach (Container container in Containers)
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
            foreach (Container container in Containers)
            {
                if (container.Id == id)
                {
                    return container;
                }
            }

            return null;
        }

        public override List<string> getRequiredTags()
        {
            string[] tags = {
                Structurizr.Tags.Element,
                Structurizr.Tags.SoftwareSystem
            };
            return tags.ToList();
        }

        public bool Equals(SoftwareSystem softwareSystem)
        {
            return this.Equals(softwareSystem as Element);
        }

    }
}
