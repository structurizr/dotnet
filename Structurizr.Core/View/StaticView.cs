using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{

    [DataContract]
    public abstract class StaticView : View
    {

        private IList<Animation> _animations = new List<Animation>();

        [DataMember(Name = "animations", EmitDefaultValue = false)]
        public IList<Animation> Animations
        {
            get { return new List<Animation>(_animations); }

            internal set { _animations = new List<Animation>(value); }
        }

        internal StaticView() : base()
        {
        }

        internal StaticView(SoftwareSystem softwareSystem, string key, string description) : base(softwareSystem, key, description)
        {
        }

        public abstract void AddAllElements();

        /// <summary>
        /// Adds all software systems in the model to this view.
        /// </summary>
        public void AddAllSoftwareSystems()
        {
            foreach (SoftwareSystem softwareSystem in this.Model.SoftwareSystems)
            {
                try
                {
                    Add(softwareSystem);
                }
                catch (ElementNotPermittedInViewException e)
                {
                    // ignore
                }
            }
        }

        /// <summary>
        /// Adds the given SoftwareSystem to this view.
        /// </summary>
        public virtual void Add(SoftwareSystem softwareSystem)
        {
            AddElement(softwareSystem, true);
        }

        /// <summary>
        /// Removes the given SoftwareSystem from this view.
        /// </summary>
        /// <param name="softwareSystem"></param>
        public void Remove(SoftwareSystem softwareSystem)
        {
            RemoveElement(softwareSystem);
        }

        /// <summary>
        /// Adds all people in the model to this view.
        /// </summary>
        public void AddAllPeople()
        {
            foreach (Person person in this.Model.People)
            {
                Add(person);
            }
        }

        /// <summary>
        /// Adds the given Person to this view.
        /// </summary>
        public void Add(Person person)
        {
            AddElement(person, true);
        }

        /// <summary>
        /// Removes the given Person from this view.
        /// </summary>
        /// <param name="person"></param>
        public void Remove(Person person)
        {
            RemoveElement(person);
        }

        /// <summary>
        /// Adds the default set of elements to this view. 
        /// </summary>
        public abstract void AddDefaultElements();

        public abstract void AddNearestNeighbours(Element element);

        protected void AddNearestNeighbours(Element element, Type typeOfElement)
        {
            if (element == null)
            {
                return;
            }

            try
            {
                AddElement(element, true);

                ICollection<Relationship> relationships = Model.Relationships;
                foreach (Relationship relationship in relationships)
                {
                    if (relationship.Source.Equals(element) && relationship.Destination.GetType() == typeOfElement)
                    {
                        try
                        {
                            AddElement(relationship.Destination, true);
                        }
                        catch (ElementNotPermittedInViewException e)
                        {
                            Console.WriteLine(e.Message + " (ignoring " + relationship.Destination.Name + ")");
                        }
                    }

                    if (relationship.Destination.Equals(element) && relationship.Source.GetType() == typeOfElement)
                    {
                        try
                        {
                            AddElement(relationship.Source, true);
                        }
                        catch (ElementNotPermittedInViewException e)
                        {
                            Console.WriteLine(e.Message + " (ignoring " + relationship.Source.Name + ")");
                        }
                    }
                }
            }
            catch (ElementNotPermittedInViewException e)
            {
                Console.WriteLine(e.Message + " (ignoring " + element.Name + ")");
            }
        }
        
        public void AddAnimation(params Element[] elements)
        {
            if (elements == null || elements.Length == 0)
            {
                throw new ArgumentException("One or more elements must be specified.");
            }

            ISet<string> elementIdsInPreviousAnimationSteps = new HashSet<string>();
            ISet<Element> elementsInThisAnimationStep = new HashSet<Element>();
            ISet<Relationship> relationshipsInThisAnimationStep = new HashSet<Relationship>();

            foreach (Element element in elements)
            {
                if (IsElementInView(element))
                {
                    elementIdsInPreviousAnimationSteps.Add(element.Id);
                    elementsInThisAnimationStep.Add(element);
                }
            }

            if (elementsInThisAnimationStep.Count == 0)
            {
                throw new ArgumentException("None of the specified elements exist in this view.");
            }

            foreach (Animation animation in Animations) {
                foreach (string elementId in animation.Elements)
                {
                    elementIdsInPreviousAnimationSteps.Add(elementId);
                }
            }

            foreach (RelationshipView relationshipView in Relationships)
            {
                if (
                    (elementsInThisAnimationStep.Contains(relationshipView.Relationship.Source) && elementIdsInPreviousAnimationSteps.Contains(relationshipView.Relationship.Destination.Id)) ||
                    (elementIdsInPreviousAnimationSteps.Contains(relationshipView.Relationship.Source.Id)) && elementsInThisAnimationStep.Contains(relationshipView.Relationship.Destination)
                   )
                {
                    relationshipsInThisAnimationStep.Add(relationshipView.Relationship);
                }
            }

            _animations.Add(new Animation(Animations.Count + 1, elementsInThisAnimationStep, relationshipsInThisAnimationStep));
        }


    }
}
