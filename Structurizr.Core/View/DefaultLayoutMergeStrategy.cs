using System;
using System.Collections.Generic;
using System.Linq;

namespace Structurizr
{
    
    /// <summary>
    /// A default implementation of a LayoutMergeStrategy that:
    ///
    /// - Sets the paper size (if not set).
    /// - Copies element x,y positions.
    /// - Copies relationship vertices.
    ///
    /// Elements are matched using the following properties, in order:
    /// - the element's full canonical name
    /// - the element's name
    /// - the element's description
    /// </summary>
    public class DefaultLayoutMergeStrategy : LayoutMergeStrategy
    {
        
        /// <summary>
        /// Attempts to copy the visual layout information (e.g. x,y coordinates) of elements and relationships
        /// from the specified source view into the specified destination view. 
        /// </summary>
        /// <param name="viewWithLayoutInformation">the source view (e.g. the version stored by the Structurizr service)</param>
        /// <param name="viewWithoutLayoutInformation">the destination View (e.g. the new version, created locally with code)</param>
        public void CopyLayoutInformation(View viewWithLayoutInformation, View viewWithoutLayoutInformation)
        {
            setPaperSizeIfNotSpecified(viewWithLayoutInformation, viewWithoutLayoutInformation);

            Dictionary<ElementView, ElementView> elementViewMap = new Dictionary<ElementView, ElementView>();
            Dictionary<Element, Element> elementMap = new Dictionary<Element, Element>();

            foreach (ElementView elementViewWithoutLayoutInformation in viewWithoutLayoutInformation.Elements)
            {
                ElementView elementViewWithLayoutInformation = findElementView(viewWithLayoutInformation, elementViewWithoutLayoutInformation.Element);
                if (elementViewWithLayoutInformation != null)
                {
                    elementViewMap.Add(elementViewWithoutLayoutInformation, elementViewWithLayoutInformation);
                    elementMap.Add(elementViewWithoutLayoutInformation.Element, elementViewWithLayoutInformation.Element);
                }
                else
                {
                    Console.WriteLine("There is no layout information for the element named " + elementViewWithoutLayoutInformation.Element.Name + " on view " + viewWithLayoutInformation.Key);
                }
            }

            foreach (ElementView elementViewWithoutLayoutInformation in elementViewMap.Keys)
            {
                ElementView elementViewWithLayoutInformation = elementViewMap[elementViewWithoutLayoutInformation];
                elementViewWithoutLayoutInformation.CopyLayoutInformationFrom(elementViewWithLayoutInformation);
            }

            foreach (RelationshipView relationshipViewWithoutLayoutInformation in viewWithoutLayoutInformation.Relationships)
            {
                RelationshipView relationshipViewWithLayoutInformation;
                if (viewWithoutLayoutInformation is DynamicView)
                {
                    relationshipViewWithLayoutInformation = findRelationshipView(viewWithLayoutInformation, relationshipViewWithoutLayoutInformation, elementMap);
                }
                else
                {
                    relationshipViewWithLayoutInformation = findRelationshipView(viewWithLayoutInformation, relationshipViewWithoutLayoutInformation.Relationship, elementMap);
                }

                if (relationshipViewWithLayoutInformation != null)
                {
                    relationshipViewWithoutLayoutInformation.CopyLayoutInformationFrom(relationshipViewWithLayoutInformation);
                }
            }
        }

        private void setPaperSizeIfNotSpecified(View remoteView, View localView)
        {
            if (localView.PaperSize == null)
            {
                localView.PaperSize = remoteView.PaperSize;
            }
        }

    /**
     * Finds an element. Override this to change the behaviour.
     *
     * @param viewWithLayoutInformation             the view to search
     * @param elementWithoutLayoutInformation       the Element to find
     * @return  an ElementView
     */
        protected ElementView findElementView(View viewWithLayoutInformation, Element elementWithoutLayoutInformation)
        {
            // see if we can find an element with the same canonical name in the source view
            ElementView elementView = viewWithLayoutInformation.Elements.FirstOrDefault(ev => ev.Element.CanonicalName.Equals(elementWithoutLayoutInformation.CanonicalName));

            if (elementView == null)
            {
                // no element was found, so try finding an element of the same type with the same name (in this situation, the parent element may have been renamed)
                elementView = viewWithLayoutInformation.Elements.FirstOrDefault(ev => ev.Element.Name.Equals(elementWithoutLayoutInformation.Name) && ev.Element.GetType().Equals(elementWithoutLayoutInformation.GetType()));
            }

            if (elementView == null)
            {
                // no element was found, so try finding an element of the same type with the same description if set (in this situation, the element itself may have been renamed)
                if (!String.IsNullOrEmpty(elementWithoutLayoutInformation.Description))
                {
                    elementView = viewWithLayoutInformation.Elements.FirstOrDefault(ev => elementWithoutLayoutInformation.Description.Equals(ev.Element.Description) && ev.Element.GetType().Equals(elementWithoutLayoutInformation.GetType()));
                }
            }

            if (elementView == null)
            {
                // no element was found, so try finding an element of the same type with the same ID (in this situation, the name and description may have changed)
                elementView = viewWithLayoutInformation.Elements.FirstOrDefault(ev => ev.Element.Id.Equals(elementWithoutLayoutInformation.Id) && ev.Element.GetType().Equals(elementWithoutLayoutInformation.GetType()));
            }

            return elementView;
        }

        private RelationshipView findRelationshipView(View viewWithLayoutInformation, Relationship relationshipWithoutLayoutInformation, Dictionary<Element,Element> elementMap)
        {
            if (!elementMap.ContainsKey(relationshipWithoutLayoutInformation.Source) || !elementMap.ContainsKey(relationshipWithoutLayoutInformation.Destination))
            {
                return null;
            }
            
            Element sourceElementWithLayoutInformation = elementMap[relationshipWithoutLayoutInformation.Source];
            Element destinationElementWithLayoutInformation = elementMap[relationshipWithoutLayoutInformation.Destination];

            foreach (RelationshipView rv in viewWithLayoutInformation.Relationships)
            {
                if (
                    rv.Relationship.Source.Equals(sourceElementWithLayoutInformation) &&
                    rv.Relationship.Destination.Equals(destinationElementWithLayoutInformation) &&
                    rv.Relationship.Description.Equals(relationshipWithoutLayoutInformation.Description)
                )
                {
                    return rv;
                }
            }

            return null;
        }

        private RelationshipView findRelationshipView(View view, RelationshipView relationshipWithoutLayoutInformation, Dictionary<Element,Element> elementMap)
        {
            if (!elementMap.ContainsKey(relationshipWithoutLayoutInformation.Relationship.Source) || !elementMap.ContainsKey(relationshipWithoutLayoutInformation.Relationship.Destination))
            {
                return null;
            }
            
            Element sourceElementWithLayoutInformation = elementMap[relationshipWithoutLayoutInformation.Relationship.Source];
            Element destinationElementWithLayoutInformation = elementMap[relationshipWithoutLayoutInformation.Relationship.Destination];

            foreach (RelationshipView rv in view.Relationships)
            {
                if (
                    rv.Relationship.Source.Equals(sourceElementWithLayoutInformation) &&
                    rv.Relationship.Destination.Equals(destinationElementWithLayoutInformation) &&
                    rv.Description.Equals(relationshipWithoutLayoutInformation.Description) &&
                    rv.Order.Equals(relationshipWithoutLayoutInformation.Order)) {
                        
                    return rv;
                }
            }

            return null;
        }        
    }
    
}