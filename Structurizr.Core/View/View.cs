using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Structurizr
{

    [DataContract]
    public abstract class View
    {

        /// <summary>
        /// An identifier for this view.
        /// </summary>
        [DataMember(Name = "key", EmitDefaultValue = false)]
        public string Key { get; set; }

        public SoftwareSystem SoftwareSystem { get; set; }

        private string softwareSystemId;

        /// <summary>
        /// The ID of the software system this view is associated with.
        /// </summary>
        [DataMember(Name = "softwareSystemId", EmitDefaultValue = false)]
        public string SoftwareSystemId {
            get
            {
                if (this.SoftwareSystem != null)
                {
                    return this.SoftwareSystem.Id;
                } else
                {
                    return this.softwareSystemId;
                }
            }
            set
            {
                this.softwareSystemId = value;
            }
        }

        [DataMember(Name = "description", EmitDefaultValue = false)]
        public string Description { get; set; }

        public abstract string Name { get; }

        /// <summary>
        /// The title for this view.
        /// </summary>
        [DataMember(Name = "title", EmitDefaultValue = false)]
        public string Title { get; set; }

        public virtual Model Model
        {
            get
            {
                return this.SoftwareSystem.Model;
            }

            set
            {
                // do nothing
            }
        }

        /// <summary>
        /// The paper size that should be used to render this view.
        /// </summary>
        [DataMember(Name = "paperSize", EmitDefaultValue = false)]
        public PaperSize PaperSize { get; set; }

        private HashSet<ElementView> _elements;

        /// <summary>
        /// The set of elements in this view.
        /// </summary>
        [DataMember(Name = "elements", EmitDefaultValue = false)]
        public ISet<ElementView> Elements
        {
            get
            {
                return new HashSet<ElementView>(_elements);
            }

            internal set
            {
                _elements = new HashSet<ElementView>(value);
            }
        }

        private HashSet<RelationshipView> _relationships;

        /// <summary>
        /// The set of relationships in this view.
        /// </summary>
        [DataMember(Name = "relationships", EmitDefaultValue = false)]
        public virtual ISet<RelationshipView> Relationships
        {
            get
            {
                return new HashSet<RelationshipView>(_relationships);
            }

            internal set
            {
                _relationships = new HashSet<RelationshipView>(value);
            }
        }

        internal View()
        {
            _elements = new HashSet<ElementView>();
            _relationships = new HashSet<RelationshipView>();
        }

        internal View(SoftwareSystem softwareSystem, string key, string description) : this()
        {
            this.SoftwareSystem = softwareSystem;
            if (key != null && key.Trim().Length > 0)
            {
                this.Key = key;
            }
            else
            {
                throw new ArgumentException("A key must be specified.");
            }
            this.Description = description;

            _elements = new HashSet<ElementView>();
            _relationships = new HashSet<RelationshipView>();
        }

        protected abstract void CheckElementCanBeAdded(Element element);

        protected void AddElement(Element element, bool addRelationships)
        {
            if (element == null)
            {
                throw new ArgumentException("An element must be specified.");
            }

            if (Model.Contains(element))
            {
                CheckElementCanBeAdded(element);
                _elements.Add(new ElementView(element));

                if (addRelationships)
                {
                    AddRelationships(element);
                }
            }
            else
            {
                throw new ArgumentException("The element named " + element.Name + " does not exist in the model associated with this view.");
            }
        }

        protected void RemoveElement(Element element)
        {
            if (element != null)
            {
                ElementView elementView = new ElementView(element);
                _elements.Remove(elementView);

                _relationships.RemoveWhere(r =>
                            r.Relationship.Source.Equals(element) ||
                            r.Relationship.Destination.Equals(element));
            }
        }

        public virtual RelationshipView Add(Relationship relationship)
        {
            if (relationship == null)
            {
                throw new ArgumentException("A relationship must be specified.");
            }

            if (IsElementInView(relationship.Source) && IsElementInView(relationship.Destination))
            {
                RelationshipView relationshipView = new RelationshipView(relationship);
                _relationships.Add(relationshipView);

                return relationshipView;
            }

            return null;
        }

        internal RelationshipView AddRelationship(Relationship relationship, string description, string order)
        {
            RelationshipView relationshipView = Add(relationship);
            if (relationshipView != null)
            {
                relationshipView.Description = description;
                relationshipView.Order = order;
            }

            return relationshipView;
        }

        internal bool IsElementInView(Element element)
        {
            return _elements.Count(ev => ev.Element.Equals(element)) > 0;
        }

        private void AddRelationships(Element element)
        {
            List<Element> elements = new List<Element>();
            foreach (ElementView elementView in this.Elements)
            {
                elements.Add(elementView.Element);
            }

            // add relationships where the destination exists in the view already
            foreach (Relationship relationship in element.Relationships)
            {
                if (elements.Contains(relationship.Destination))
                {
                    this._relationships.Add(new RelationshipView(relationship));
                }
            }

            // add relationships where the source exists in the view already
            foreach (Element e in elements)
            {
                foreach (Relationship relationship in e.Relationships)
                {
                    if (relationship.Destination.Equals(element))
                    {
                        _relationships.Add(new RelationshipView(relationship));
                    }
                }
            }
        }

        public void Remove(Relationship relationship)
        {
            if (relationship != null)
            {
                RelationshipView relationshipView = new RelationshipView(relationship);
                _relationships.Remove(relationshipView);
            }
        }

        public void CopyLayoutInformationFrom(View source)
        {
            if (PaperSize == null)
            {
                PaperSize = source.PaperSize;
            }

            foreach (ElementView sourceElementView in source.Elements)
            {
                ElementView destinationElementView = FindElementView(sourceElementView);
                if (destinationElementView != null)
                {
                    destinationElementView.CopyLayoutInformationFrom(sourceElementView);
                }
            }

            foreach (RelationshipView sourceRelationshipView in source.Relationships)
            {
                RelationshipView destinationRelationshipView = FindRelationshipView(sourceRelationshipView);
                if (destinationRelationshipView != null)
                {
                    destinationRelationshipView.CopyLayoutInformationFrom(sourceRelationshipView);
                }
            }
        }

        private ElementView FindElementView(ElementView sourceElementView)
        {
            foreach (ElementView elementView in Elements)
            {
                if (elementView.Element.Equals(sourceElementView.Element))
                {
                    return elementView;
                }
            }

            return null;
        }

        internal virtual RelationshipView FindRelationshipView(RelationshipView sourceRelationshipView)
        {
            foreach (RelationshipView relationshipView in Relationships)
            {
                if (relationshipView.Relationship.Equals(sourceRelationshipView.Relationship))
                {
                    return relationshipView;
                }
            }

            return null;
        }

        [DataMember(Name = "automaticLayout", EmitDefaultValue = false)]
        public AutomaticLayout AutomaticLayout { get; internal set; }

        /// <summary>
        /// Enables automatic layout for this view, with some default settings.
        /// </summary>
        public void EnableAutomaticLayout()
        {
            EnableAutomaticLayout(RankDirection.TopBottom, 300, 600, 200, false);
        }

        /// <summary>
        /// Enables the automatic layout for this view, with the specified settings.
        /// </summary>
        /// <param name="rankDirection">the rank direction</param>
        /// <param name="rankSeparation">the separation between ranks (in pixels, a positive integer)</param>
        /// <param name="nodeSeparation">the separation between nodes within the same rank (in pixels, a positive integer)</param>
        /// <param name="edgeSeparation">the separation between edges (in pixels, a positive integer)</param>
        /// <param name="vertices">whether vertices should be created during automatic layout</param>
        public void EnableAutomaticLayout(RankDirection rankDirection, int rankSeparation, int nodeSeparation, int edgeSeparation, bool vertices)
        {
            AutomaticLayout = new AutomaticLayout(rankDirection, rankSeparation, nodeSeparation, edgeSeparation, vertices);
        }

        /// <summary>
        /// Disables automatic layout for this view.
        /// </summary>
        public void DisableAutomaticLayout()
        {
            AutomaticLayout = null;
        }
        
    }
}