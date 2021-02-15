using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A container view.
    /// </summary>
    [DataContract]
    public sealed class ContainerView : StaticView
    {

        public override string Name
        {
            get
            {
                return SoftwareSystem.Name + " - Containers";
            }
        }

        /// <summary>
        /// Determines whether software system boundaries should be visible for "external" containers (those outside the software system in scope).
        /// </summary>
        [DataMember(Name = "externalSoftwareSystemBoundariesVisible", EmitDefaultValue = false)]
        public bool? ExternalSoftwareSystemBoundariesVisible { get; set; }

        internal ContainerView() : base()
        {
        }

        internal ContainerView(SoftwareSystem softwareSystem, string key, string description) : base(softwareSystem, key, description)
        {
        }

        protected override void CheckElementCanBeAdded(Element element)
        {
            if (element is Person)
            {
                return;
            }

            if (element is SoftwareSystem)
            {
                if (element.Equals(SoftwareSystem))
                {
                    throw new ElementNotPermittedInViewException("The software system in scope cannot be added to a container view.");
                }
                else
                {
                    return;
                }
            }

            if (element is Container)
            {
                return;
            }

            throw new ElementNotPermittedInViewException("Only people, software systems, and containers can be added to a container view.");
        }

        /// <summary>
        /// Adds all software systems, people and containers to this view.
        /// </summary>
        public override void AddAllElements()
        {
            AddAllSoftwareSystems();
            AddAllPeople();
            AddAllContainers();
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
            AddElement(container, true);
        }

        public void Remove(Container container)
        {
            RemoveElement(container);
        }

        /// <summary>
        /// Adds people, software systems and containers that are directly related to the given element.
        /// </summary>
        public override void AddNearestNeighbours(Element element)
        {
            AddNearestNeighbours(element, typeof(Person));
            AddNearestNeighbours(element, typeof(SoftwareSystem));
            AddNearestNeighbours(element, typeof(Container));
        }
        
        /// <summary>
        /// Adds the default set of elements to this view. 
        /// </summary>
        public override void AddDefaultElements()
        {
            foreach (Container container in SoftwareSystem.Containers)
            {
                Add(container);
                AddNearestNeighbours(container, typeof(Person));
                AddNearestNeighbours(container, typeof(SoftwareSystem));
            }
        }
        
    }
}
