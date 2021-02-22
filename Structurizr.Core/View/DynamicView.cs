using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A dynamic view, used to describe behaviour between static elements at runtime.
    /// </summary>
    [DataContract]
    public sealed class DynamicView : View
    {

        public override Model Model { get; set; }

        public override ISet<RelationshipView> Relationships
        {
            get
            {
                List<RelationshipView> list = new List<RelationshipView>(base.Relationships);
                bool ordersAreNumeric = true;

                foreach (RelationshipView relationshipView in list)
                {
                    ordersAreNumeric = ordersAreNumeric && isNumeric(relationshipView.Order);
                }

                if (ordersAreNumeric)
                {
                    list.Sort(CompareAsNumber);
                }
                else
                {
                    list.Sort(CompareAsString);
                }

                return new HashSet<RelationshipView>(list);
            }
        }

        private bool isNumeric(string str)
        {
            try
            {
                double.Parse(str);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private int CompareAsNumber(RelationshipView x, RelationshipView y)
        {
            return double.Parse(x.Order).CompareTo(double.Parse(y.Order));
        }

        private int CompareAsString(RelationshipView x, RelationshipView y)
        {
            return x.Order.CompareTo(y.Order); 
        }

        public override string Name
        {
            get
            {
                if (Element != null)
                {
                    return Element.Name + " - Dynamic";
                }
                else
                {
                    return "Dynamic";
                }
            }
        }

        public Element Element { get; set; }

        private string _elementId;

        /// <summary>
        /// The ID of the container this view is associated with.
        /// </summary>
        [DataMember(Name="elementId", EmitDefaultValue=false)]
        public string ElementId {
            get {
                return Element != null ? Element.Id : _elementId;
            }
            set
            {
                _elementId = value;
            }
        }

        private readonly SequenceNumber _sequenceNumber = new SequenceNumber();

        internal DynamicView()
        {
        }

        internal DynamicView(Model model, string key, string description) : base(null, key, description)
        {
            Model = model;
            Element = null;
        }

        internal DynamicView(SoftwareSystem softwareSystem, string key, string description) : base(softwareSystem, key, description)
        {
            Model = softwareSystem.Model;
            Element = softwareSystem;
        }

        internal DynamicView(Container container, string key, string description) : base(container.SoftwareSystem, key, description)
        {
            Model = container.Model;
            Element = container;
        }

        protected override void CheckElementCanBeAdded(Element elementToBeAdded)
        {
            if (!(elementToBeAdded is StaticStructureElement))
            {
                throw new ElementNotPermittedInViewException(
                    "Only people, software systems, containers and components can be added to dynamic views.");
            }

            StaticStructureElement staticStructureElementToBeAdded = (StaticStructureElement) elementToBeAdded;

            // people can always be added
            if (staticStructureElementToBeAdded is Person)
            {
                return;
            }

            // if the scope of this dynamic view is a software system, we only want:
            //  - containers
            //  - other software systems
            if (Element is SoftwareSystem)
            {
                if (staticStructureElementToBeAdded.Equals(Element))
                {
                    throw new ElementNotPermittedInViewException(
                        staticStructureElementToBeAdded.Name +
                        " is already the scope of this view and cannot be added to it.");
                }

                if (staticStructureElementToBeAdded is SoftwareSystem ||
                    staticStructureElementToBeAdded is Container) {
                    checkParentAndChildrenHaveNotAlreadyBeenAdded(staticStructureElementToBeAdded);
                } else if (staticStructureElementToBeAdded is Component) {
                    throw new ElementNotPermittedInViewException(
                        "Components can't be added to a dynamic view when the scope is a software system.");
                }
            }

            // dynamic view with container scope:
            //  - other containers
            //  - components
            if (Element is Container) {
                if (staticStructureElementToBeAdded.Equals(Element) ||
                    staticStructureElementToBeAdded.Equals(Element.Parent))
                {
                    throw new ElementNotPermittedInViewException(
                        staticStructureElementToBeAdded.Name +
                        " is already the scope of this view and cannot be added to it.");
                }

                checkParentAndChildrenHaveNotAlreadyBeenAdded(staticStructureElementToBeAdded);
            }

            // dynamic view with no scope
            //  - software systems
            if (Element == null)
            {
                if (!(staticStructureElementToBeAdded is SoftwareSystem)) {
                    throw new ElementNotPermittedInViewException(
                        "Only people and software systems can be added to this dynamic view.");
                }
            }
        }

        private void checkParentAndChildrenHaveNotAlreadyBeenAdded(StaticStructureElement elementToBeAdded) {
            // check the parent hasn't been added already
            ISet<String> elementIds = new HashSet<string>(Elements.Select(ev => ev.Element.Id));

            if (elementToBeAdded.Parent != null) {
                if (elementIds.Contains(elementToBeAdded.Parent.Id)) {
                    throw new ElementNotPermittedInViewException("The parent of " + elementToBeAdded.Name + " is already in this view.");
                }
            }

            // and now check a child hasn't been added already
            ISet<String> elementParentIds = new HashSet<string>(Elements.Where(ev => ev.Element.Parent != null).Select(ev => ev.Element.Parent.Id));

            if (elementParentIds.Contains(elementToBeAdded.Id)) {
                throw new ElementNotPermittedInViewException("The child of " + elementToBeAdded.Name + " is already in this view.");
            }
        }

        public RelationshipView Add(StaticStructureElement source, StaticStructureElement destination)
        {
            return Add(source, "", destination);
        }

        public RelationshipView Add(StaticStructureElement source, string description, StaticStructureElement destination)
        {
            if (source == null) {
                throw new ArgumentException("A source element must be specified.");
            }

            if (destination == null) {
                throw new ArgumentException("A destination element must be specified.");
            }

            CheckElementCanBeAdded(source);
            CheckElementCanBeAdded(destination);

            // check that the relationship is in the model before adding it
            Relationship relationship = source.GetEfferentRelationshipWith(destination);
            
            if (relationship != null)
            {
                AddElement(source, false);
                AddElement(destination, false);

                return AddRelationship(relationship, description, _sequenceNumber.GetNext(), false);
            }
            else
            {
                // perhaps model this as a return/reply/response message instead, if the reverse relationship exists
                relationship = destination.GetEfferentRelationshipWith(source);

                if (relationship != null)
                {
                    AddElement(source, false);
                    AddElement(destination, false);

                    return AddRelationship(relationship, description, _sequenceNumber.GetNext(), true);
                }
                else
                { 
                    throw new ArgumentException("A relationship between " + source.Name + " and " + destination.Name + " does not exist in model.");
                }
            }
        }

        public override RelationshipView Add(Relationship relationship)
        {
            // when adding a relationship to a DynamicView we suppose the user really wants to also see both elements
            AddElement(relationship.Source, false);
            AddElement(relationship.Destination, false);

            return base.Add(relationship);
        }

        public void StartParallelSequence()
        {
            _sequenceNumber.StartParallelSequence();
        }

        public void EndParallelSequence()
        {
            EndParallelSequence(false);
        }

        public void EndParallelSequence(bool endAllParallelSequencesAndContinueNumbering)
        {
            _sequenceNumber.EndParallelSequence(endAllParallelSequencesAndContinueNumbering);
        }

    }
}