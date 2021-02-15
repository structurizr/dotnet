using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A container (e.g. an application or data store).
    /// </summary>
    [DataContract]
    public sealed class Container : StaticStructureElement, IEquatable<Container>
    {

        public override Element Parent { get; set; }

        public SoftwareSystem SoftwareSystem
        {
            get
            {
                return Parent as SoftwareSystem;
            }
        }

        /// <summary>
        /// The technology associated with this container (e.g. Windows Service).
        /// </summary>
        [DataMember(Name="technology", EmitDefaultValue=false)]
        public string Technology { get; set; }

        private HashSet<Component> _components;

        /// <summary>
        /// The set of components within this container.
        /// </summary>
        [DataMember(Name="components", EmitDefaultValue=false)]
        public ISet<Component> Components
        {
            get
            {
                return new HashSet<Component>(_components);
            }

            set
            {
                _components = new HashSet<Component>(value);
            }
        }
  
        public override string CanonicalName
        {
            get
            {
                return new CanonicalNameGenerator().Generate(this);
            }
        }

        internal Container()
        {
            _components = new HashSet<Component>();
        }

        /// <summary>
        /// Adds a component with the specified name (unless one exists with the same name already).
        /// </summary>
        /// <param name="name">the name of the component</param>
        /// <returns>a Component instance</returns>
        public Component AddComponent(string name)
        {
            return AddComponent(name, "");
        }

        /// <summary>
        /// Adds a component with the specified name and description (unless one exists with the same name already).
        /// </summary>
        /// <param name="name">the name of the component</param>
        /// <param name="description">a short description/list of responsibilities</param>
        /// <returns>a Component instance</returns>
        public Component AddComponent(string name, string description)
        {
            return AddComponent(name, description, "");
        }

        /// <summary>
        /// Adds a component with the specified name, description and technology (unless one exists with the same name already).
        /// </summary>
        /// <param name="name">the name of the component</param>
        /// <param name="description">a short description/list of responsibilities</param>
        /// <param name="technology">the technology choice</param>
        /// <returns>a Component instance</returns>
        public Component AddComponent(string name, string description, string technology)
        {
            return AddComponent(name, (String)null, description, technology);
        }

        public Component AddComponent(string name, Type type, string description, string technology)
        {
           return AddComponent(name, type.AssemblyQualifiedName, description, technology);
        }

        public Component AddComponent(string name, string type, string description, string technology)
        {
            return Model.AddComponent(this, name, type, description, technology);
        }

        internal void Add(Component component)
        {
            if (GetComponentWithName(component.Name) == null)
            {
                _components.Add(component);
            }
        }

        public Component GetComponentWithName(string name)
        {
            if (name == null)
            {
                return null;
            }

            foreach (Component component in Components)
            {
                if (component.Name == name)
                {
                    return component;
                }
            }

            return null;
        }

        public Component GetComponentOfType(string type)
        {
            if (type == null)
            {
                return null;
            }

            return _components.Where(c => c.Type == type).FirstOrDefault();
        }


        public override List<string> GetRequiredTags()
        {
            return new List<string>
            {
                Structurizr.Tags.Element,
                Structurizr.Tags.Container
            };
        }

        public bool Equals(Container container)
        {
            return this.Equals(container as Element);
        }

    }
}