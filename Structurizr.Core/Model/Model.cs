using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A software architecture model.
    /// </summary>
    [DataContract]
    public class Model
    {

        [DataMember(Name = "enterprise", EmitDefaultValue = false)]
        public Enterprise Enterprise { get; set; }

        [DataMember(Name = "people", EmitDefaultValue = false)]
        public HashSet<Person> People { get; set; }

        [DataMember(Name = "softwareSystems", EmitDefaultValue = false)]
        public HashSet<SoftwareSystem> SoftwareSystems { get; }

        private Dictionary<string, Element> elementsById = new Dictionary<string, Element>();
        private Dictionary<string, Relationship> relationshipsById = new Dictionary<string, Relationship>();

        public ICollection<Relationship> Relationships
        {
            get
            {
                return this.relationshipsById.Values;
            }
        }

        private SequentialIntegerIdGeneratorStrategy idGenerator = new SequentialIntegerIdGeneratorStrategy();

        internal Model()
        {
            this.People = new HashSet<Person>();
            this.SoftwareSystems = new HashSet<SoftwareSystem>();
        }

        /// <summary>
        /// Creates a software system (location is unspecified) and adds it to the model
        /// (unless one exists with the same name already).
        /// </summary>
        /// <param name="Name">The name of the software system</param>
        /// <param name="Description">A short description of the software syste.</param>
        /// <returns>the SoftwareSystem instance created and added to the model (or null)</returns>
        public SoftwareSystem AddSoftwareSystem(string name, string description)
        {
            return AddSoftwareSystem(Location.Unspecified, name, description);
        }

        /// <summary>
        /// Creates a software system (location is unspecified) and adds it to the model
        /// (unless one exists with the same name already).
        /// </summary>
        /// <param name="location">The location of the software system (e.g. internal, external, etc)</param>
        /// <param name="name">The name of the software system</param>
        /// <param name="description">A short description of the software syste.</param>
        /// <returns>the SoftwareSystem instance created and added to the model (or null)</returns>
        public SoftwareSystem AddSoftwareSystem(Location location, string name, string description)
        {
            if (GetSoftwareSystemWithName(name) == null)
            {
                SoftwareSystem softwareSystem = new SoftwareSystem();
                softwareSystem.Location = location;
                softwareSystem.Name = name;
                softwareSystem.Description = description;

                Add(softwareSystem);

                return softwareSystem;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a software system to the model
        /// (unless one exists with the same name already).
        /// </summary>
        /// <param name="softwareSystem">The SoftwareSystem instance to be added to the model</param>
        public void Add(SoftwareSystem softwareSystem)
        {
            if (GetSoftwareSystemWithName(softwareSystem.Name) == null)
            {
                SoftwareSystems.Add(softwareSystem);
                softwareSystem.Id = idGenerator.GenerateId(softwareSystem);
                AddElementToInternalStructures(softwareSystem);
            }
        }

        /// <summary>
        /// Creates a person (location is unspecified) and adds it to the model
        /// (unless one exists with the same name already.
        /// </summary>
        /// <param name="name">the name of the person (e.g. "Admin User" or "Bob the Business User")</param>
        /// <param name="description">a short description of the person</param>
        /// <returns>the Person instance created and added to the model (or null)</returns>
        public Person AddPerson(string name, string description)
        {
            return AddPerson(Location.Unspecified, name, description);
        }

        /// <summary>
        /// Creates a person (location is unspecified) and adds it to the model
        /// (unless one exisrs with the same name already.
        /// </summary>
        /// <param name="location">the location of the person (e.g. internal, external, etc)</param>
        /// <param name="name">the name of the person (e.g. "Admin User" or "Bob the Business User")</param>
        /// <param name="description">a short description of the person</param>
        /// <returns>the Person instance created and added to the model (or null)</returns>
        public Person AddPerson(Location location, string name, string description)
        {
            if (GetPersonWithName(name) == null)
            {
                Person person = new Person();
                person.Location = location;
                person.Name = name;
                person.Description = description;

                People.Add(person);

                person.Id = idGenerator.GenerateId(person);
                AddElementToInternalStructures(person);

                return person;
            }
            else {
                return null;
            }
        }

        internal Container AddContainer(SoftwareSystem parent, string name, string description, string technology)
        {
            if (parent.GetContainerWithName(name) == null)
            {
                Container container = new Container();
                container.Name = name;
                container.Description = description;
                container.Technology = technology;

                return this.AddContainer(parent, container);
            }
            else {
                return null;
            }
        }

        internal Container AddContainer(SoftwareSystem parent, Container container)
        {
            if (parent.GetContainerWithName(container.Name) == null)
            {
                container.Parent = parent;
                parent.Add(container);

                container.Id = idGenerator.GenerateId(container);
                AddElementToInternalStructures(container);

                return container;
            }
            else
            {
                return null;
            }
        }

        internal Component AddComponent(Container parent, string name, string description, string technology)
        {
            return this.AddComponent(parent, name, null, description, technology);
        }

        internal Component AddComponent(Container parent, string name, string type, string description, string technology)
        {
            Component component = new Component();
            component.Name = name;
            component.Type = type;
            component.Description = description;
            component.Technology = technology;

            component.Parent = parent;
            parent.Add(component);

            component.Id = idGenerator.GenerateId(component);
            AddElementToInternalStructures(component);

            return component;
        }

        internal void AddRelationship(Relationship relationship)
        {
            if (!relationship.Source.Has(relationship))
            {
                relationship.Id = idGenerator.GenerateId(relationship);
                relationship.Source.AddRelationship(relationship);

                AddRelationshipToInternalStructures(relationship);
            }
        }

        private void AddRelationshipToInternalStructures(Relationship relationship)
        {
            relationshipsById.Add(relationship.Id, relationship);
            idGenerator.Found(relationship.Id);
        }

        /// <summary>
        /// Gets the SoftwareSystem instance with the specified name.
        /// </summary>
        /// <returns>A SoftwareSystem instance, or null if one doesn't exist.</returns>
        public SoftwareSystem GetSoftwareSystemWithName(string name)
        {
            foreach (SoftwareSystem softwareSystem in SoftwareSystems)
            {
                if (softwareSystem.Name == name)
                {
                    return softwareSystem;
                }
            }

            return null;
        }

        public SoftwareSystem GetSoftwareSystemWithId(string id)
        {
            foreach (SoftwareSystem softwareSystem in SoftwareSystems)
            {
                if (softwareSystem.Id == id)
                {
                    return softwareSystem;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the Person instance with the specified name.
        /// </summary>
        /// <returns>A Person instance, or null if one doesn't exist.</returns>
        public Person GetPersonWithName(string name)
        {
            foreach (Person person in People)
            {
                if (person.Name == name)
                {
                    return person;
                }
            }

            return null;
        }

        private void AddElementToInternalStructures(Element element)
        {
            elementsById.Add(element.Id, element);
            element.Model = this;
            idGenerator.Found(element.Id);
        }

        public bool Contains(Element element)
        {
            return this.elementsById.Values.Contains(element);
        }

        internal void Hydrate()
        {
            // add all of the elements to the model
            foreach (Person person in People)
            {
                AddElementToInternalStructures(person);
            }

            foreach (SoftwareSystem softwareSystem in SoftwareSystems)
            {
                AddElementToInternalStructures(softwareSystem);
                foreach (Container container in softwareSystem.Containers)
                {
                    softwareSystem.Add(container);
                    AddElementToInternalStructures(container);
                    container.Parent = softwareSystem;
                    foreach (Component component in container.Components)
                    {
                        container.Add(component);
                        AddElementToInternalStructures(component);
                        component.Parent = container;
                    }
                }
            }

            // now hydrate the relationships
            foreach (Person person in People)
            {
                HydrateRelationships(person);
            }

            foreach (SoftwareSystem softwareSystem in SoftwareSystems)
            {
                HydrateRelationships(softwareSystem);
                foreach (Container container in softwareSystem.Containers)
                {
                    HydrateRelationships(container);
                    foreach (Component component in container.Components)
                    {
                        HydrateRelationships(component);
                    }
                }
            }
        }

        private void HydrateRelationships(Element element)
        {
            foreach (Relationship relationship in element.Relationships)
            {
                relationship.Source = GetElement(relationship.SourceId);
                relationship.Destination = GetElement(relationship.DestinationId);
                AddRelationshipToInternalStructures(relationship);
            }
        }

        public Element GetElement(String id)
        {
            return this.elementsById[id];
        }

        public Relationship GetRelationship(String id)
        {
            return this.relationshipsById[id];
        }

    }

}
