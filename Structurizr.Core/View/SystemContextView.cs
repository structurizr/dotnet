using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A system context view.
    /// </summary>
    [DataContract]
    public sealed class SystemContextView : StaticView
    {

        public override string Name
        {
            get
            {
                return SoftwareSystem.Name + " - System Context";
            }
        }

        /// <summary>
        /// Determines whether the enterprise boundary (to differentiate "internal" elements from "external" elements") should be visible on the resulting diagram.
        /// </summary>
        [DataMember(Name = "enterpriseBoundaryVisible", EmitDefaultValue = false)]
        public bool? EnterpriseBoundaryVisible { get; set; }

        internal SystemContextView() : base()
        {
        }

        internal SystemContextView(SoftwareSystem softwareSystem, string key, string description) : base(softwareSystem, key, description)
        {
            AddElement(softwareSystem, true);
        }

        /// <summary>
        /// Adds all software systems and all people to this view.
        /// </summary>
        public override void AddAllElements()
        {
            AddAllSoftwareSystems();
            AddAllPeople();
        }

        /// <summary>
        /// Adds people and software systems that are directly related to the given element.
        /// </summary>
        public override void AddNearestNeighbours(Element element)
        {
            AddNearestNeighbours(element, typeof(SoftwareSystem));
            AddNearestNeighbours(element, typeof(Person));
        }
        
        /// <summary>
        /// Adds the default set of elements to this view. 
        /// </summary>
        public override void AddDefaultElements()
        {
            AddNearestNeighbours(SoftwareSystem, typeof(Person));
            AddNearestNeighbours(SoftwareSystem, typeof(SoftwareSystem));
        }

    }
}
