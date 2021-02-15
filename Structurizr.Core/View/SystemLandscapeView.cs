using System.Runtime.Serialization;

namespace Structurizr
{ 

    /// <summary>
    /// Represents an System Landscape view that sits above the C4 model. This is the "big picture" view,
    /// showing the software systems and people in an given environment.
    /// The permitted elements in this view are software systems and people.
    /// </summary>
    [DataContract]
    public sealed class SystemLandscapeView : StaticView
    {

        public override string Name
        {
            get
            {
                Enterprise enterprise = Model.Enterprise;
                return "System Landscape" + (enterprise != null && enterprise.Name.Trim().Length > 0 ? " for " + enterprise.Name : "");
            }
        }

        public sealed override Model Model { get; set; }

        /// <summary>
        /// Determines whether the enterprise boundary (to differentiate "internal" elements from "external" elements") should be visible on the resulting diagram.
        /// </summary>
        [DataMember(Name = "enterpriseBoundaryVisible", EmitDefaultValue = false)]
        public bool? EnterpriseBoundaryVisible { get; set; }

        internal SystemLandscapeView() : base()
        {
        }

        internal SystemLandscapeView(Model model, string key, string description) : base(null, key, description)
        {
            Model = model;
        }
        
        protected override void CheckElementCanBeAdded(Element element)
        {
            if (element is Person || element is SoftwareSystem)
            {
                // all good
            }
            else
            {
                throw new ElementNotPermittedInViewException("Only people and software systems can be added to a system landscape view.");
            }
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
            AddAllSoftwareSystems();
            AddAllPeople();
        }
        
    }
}