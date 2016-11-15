using System.Runtime.Serialization;

namespace Structurizr
{ 

    /// <summary>
    /// Represents an Enterprise Context view that sits above the C4 model. This is the "big picture" view,
    /// showing the software systems and people in an given environment.
    /// The permitted elements in this view are software systems and people.
    /// </summary>
    [DataContract]
    public class EnterpriseContextView : StaticView
    {

        public override string Name
        {
            get
            {
                Enterprise enterprise = Model.Enterprise;
                return "Enterprise Context" + (enterprise != null && enterprise.Name.Trim().Length > 0 ? " for " + enterprise.Name : "");
            }
        }

        public sealed override Model Model { get; set; }

        internal EnterpriseContextView() : base()
        {
        }

        internal EnterpriseContextView(Model model, string key, string description) : base(null, key, description)
        {
            Model = model;
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

    }
}
