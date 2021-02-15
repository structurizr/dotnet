using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A system context view.
    /// </summary>
    [DataContract]
    public sealed class ComponentView : StaticView
    {

        public override string Name
        {
            get
            {
                return SoftwareSystem.Name + " - " + Container.Name + " - Components";
            }
        }

        public Container Container { get; set; }

        private string containerId;

        /// <summary>
        /// The ID of the container this view is associated with.
        /// </summary>
        [DataMember(Name="containerId", EmitDefaultValue=false)]
        public string ContainerId {
            get
            {
                if (Container != null)
                {
                    return Container.Id;
                } else
                {
                    return containerId;
                }
            }
            set
            {
                this.containerId = value;
            }
        }
        
        /// <summary>
        /// Determines whether container boundaries should be visible for "external" components (those outside the container in scope).
        /// </summary>
        [DataMember(Name = "externalContainerBoundariesVisible", EmitDefaultValue = false)]
        public bool? ExternalContainerBoundariesVisible { get; set; }

        internal ComponentView() : base()
        {
        }

        internal ComponentView(Container container, string key, string description) : base(container.SoftwareSystem,key,  description)
        {
            this.Container = container;
        }

        public override void AddAllElements()
        {
            AddAllSoftwareSystems();
            AddAllPeople();
            AddAllContainers();
            AddAllComponents();
        }

        public override void Add(SoftwareSystem softwareSystem)
        {
            if (softwareSystem != null && !softwareSystem.Equals(SoftwareSystem))
            {
                AddElement(softwareSystem, true);
            }
        }

        public void AddAllContainers()
        {
            foreach (Container container in SoftwareSystem.Containers)
            {
                Add(container);
            }
        }

        public void Add(Container container)
        {
            if (container != null && !container.Equals(Container))
            {
                AddElement(container, true);
            }
        }

        public void Remove(Container container)
        {
            RemoveElement(container);
        }

        public void AddAllComponents()
        {
            foreach (Component component in Container.Components)
            {
                Add(component);
            }
        }

        public void Add(Component component)
        {
            if (component != null)
            {
                AddElement(component, true);
            }
        }

        public void Remove(Component component)
        {
            RemoveElement(component);
        }

        /// <summary>
        /// Adds people, software systems, containers and components that are directly related to the given element.
        /// </summary>
        public override void AddNearestNeighbours(Element element)
        {
            AddNearestNeighbours(element, typeof(Person));
            AddNearestNeighbours(element, typeof(SoftwareSystem));
            AddNearestNeighbours(element, typeof(Container));
            AddNearestNeighbours(element, typeof(Component));
        }
        
        /// <summary>
        /// Adds the default set of elements to this view.
        /// </summary>
        public override void AddDefaultElements()
        {
            foreach (Component component in Container.Components)
            {
                Add(component);

                foreach (Container container in SoftwareSystem.Containers)
                {
                    if (container.HasEfferentRelationshipWith(component) || component.HasEfferentRelationshipWith(container))
                    {
                        Add(container);
                    }
                };

                AddNearestNeighbours(component, typeof(Person));
                AddNearestNeighbours(component, typeof(SoftwareSystem));
            }
        }
        
    }
}
