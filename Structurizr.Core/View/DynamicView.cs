using System;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A dynamic view, used to describe behaviour between static elements at runtime.
    /// </summary>
    [DataContract]
    public class DynamicView : View
    {

        public sealed override Model Model { get; set; }

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

        public RelationshipView Add(Element source, Element destination)
        {
            return Add(source, "", destination);
        }

        public RelationshipView Add(Element source, string description, Element destination)
        {
            if (source != null && destination != null)
            {
                CheckElement(source);
                CheckElement(destination);

                // check that the relationship is in the model before adding it
                Relationship relationship = source.GetEfferentRelationshipWith(destination);
                if (relationship != null)
                {
                    AddElement(source, false);
                    AddElement(destination, false);
                    RelationshipView relationshipView = AddRelationship(relationship, description, _sequenceNumber.GetNext());
                    return relationshipView;
                }
                else
                {
                    throw new ArgumentException("Relationship does not exist in model");
                }
            }
            else
            {
                throw new ArgumentException("Source and destination must not be null");
            }
        }

        private void CheckElement(Element e)
        {
            // people can always be added
            if (e is Person) {
                return;
            }

            // if the scope of this dynamic is a software system, we only want:
            // - containers inside that software system
            // - other software systems
            if (Element is SoftwareSystem) {
                if (e.Equals(Element))
                {
                    throw new ArgumentException(e.Name + " is already the scope of this view and cannot be added to it.");
                }
                if (e is Container && !e.Parent.Equals(Element))
                {
                    throw new ArgumentException("Only containers that reside inside " + Element.Name + " can be added to this view.");
                }
                if (e is Component) {
                    throw new ArgumentException("Components can't be added to a dynamic view when the scope is a software system.");
                }
            }

            // if the scope of this dynamic view is a container, we only want other containers inside the same software system
            // and other components inside the container
            if (Element is Container) {
                if (e.Equals(Element) || e.Equals(Element.Parent))
                {
                    throw new ArgumentException(e.Name + " is already the scope of this view and cannot be added to it.");
                }
                if (e is Container && !e.Parent.Equals(Element.Parent)) {
                    throw new ArgumentException("Only containers that reside inside " + Element.Parent.Name + " can be added to this view.");
                }

                if (e is Component && !e.Parent.Equals(Element)) {
                    throw new ArgumentException("Only components that reside inside " + Element.Name + " can be added to this view.");
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

        internal override RelationshipView FindRelationshipView(RelationshipView sourceRelationshipView)
        {
            foreach (RelationshipView relationshipView in Relationships)
            {
                if (relationshipView.Relationship.Equals(sourceRelationshipView.Relationship))
                {
                    if ((relationshipView.Description != null && relationshipView.Description.Equals(sourceRelationshipView.Description)) &&
                            relationshipView.Order.Equals(sourceRelationshipView.Order))
                    {
                        return relationshipView;
                    }
                }
            }

            return null;
        }

        public void StartParallelSequence()
        {
            _sequenceNumber.StartParallelSequence();
        }

        public void EndParallelSequence()
        {
            _sequenceNumber.EndParallelSequence();
        }

    }
}
