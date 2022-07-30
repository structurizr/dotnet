using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// A deployment view, used to show the mapping of container instances to deployment nodes.
    /// </summary>
    public sealed class DeploymentView : View
    {

        public override Model Model { get; set; }

        private IList<Animation> _animations = new List<Animation>();

        [DataMember(Name = "animations", EmitDefaultValue = false)]
        public IList<Animation> Animations
        {
            get { return new List<Animation>(_animations); }

            internal set { _animations = new List<Animation>(value); }
        }

        /// <summary>
        /// The name of the environment that this deployment view is for (e.g. "Development", "Live", etc).
        /// </summary>
        [DataMember(Name = "environment", EmitDefaultValue = false)]
        public string Environment { get; set; }

        DeploymentView()
        {
            Environment = DeploymentElement.DefaultDeploymentEnvironment;
        }

        internal DeploymentView(Model model, string key, string description) : base(null, key, description)
        {
            Model = model;
            Environment = DeploymentElement.DefaultDeploymentEnvironment;
        }

        internal DeploymentView(SoftwareSystem softwareSystem, string key, string description) : base(softwareSystem,
            key, description)
        {
            Model = softwareSystem.Model;
            Environment = DeploymentElement.DefaultDeploymentEnvironment;
        }

        protected override void CheckElementCanBeAdded(Element elementToBeAdded)
        {
            if (!(elementToBeAdded is DeploymentElement))
            {
                throw new ElementNotPermittedInViewException("Only deployment nodes, infrastructure nodes, software system instances, and container instances can be added to deployment views.");
            }

            DeploymentElement deploymentElementToBeAdded = (DeploymentElement) elementToBeAdded;
            if (!deploymentElementToBeAdded.Environment.Equals(this.Environment)) {
                throw new ElementNotPermittedInViewException("Only elements in the " + this.Environment + " deployment environment can be added to this view.");
            }

            if (this.SoftwareSystem != null && elementToBeAdded is SoftwareSystemInstance) {
                SoftwareSystemInstance ssi = (SoftwareSystemInstance) elementToBeAdded;

                if (ssi.SoftwareSystem.Equals(this.SoftwareSystem)) {
                    // adding an instance of the scoped software system isn't permitted
                    throw new ElementNotPermittedInViewException("The software system in scope cannot be added to a deployment view.");
                }
            }

            if (elementToBeAdded is SoftwareSystemInstance)
            {
                // check that a child container instance hasn't been added already
                SoftwareSystemInstance softwareSystemInstanceToBeAdded = (SoftwareSystemInstance) elementToBeAdded;
                IList<string> softwareSystemIds = Elements.Select(ev => ev.Element).OfType<ContainerInstance>().Select(ci => ci.Container.SoftwareSystem.Id).ToList();

                if (softwareSystemIds.Contains(softwareSystemInstanceToBeAdded.SoftwareSystemId))
                {
                    throw new ElementNotPermittedInViewException("A child of " + elementToBeAdded.Name + " is already in this view.");
                }
            }

            if (elementToBeAdded is ContainerInstance)
            {
                // check that the parent software system instance hasn't been added already
                ContainerInstance containerInstanceToBeAdded = (ContainerInstance)elementToBeAdded;
                IList<String> softwareSystemIds = Elements.Select(ev => ev.Element).OfType<SoftwareSystemInstance>().Select(ssi => ssi.SoftwareSystemId).ToList();

                if (softwareSystemIds.Contains(containerInstanceToBeAdded.Container.SoftwareSystem.Id))
                {
                    throw new ElementNotPermittedInViewException("The parent of " + elementToBeAdded.Name + " is already in this view.");
                }
            }
        }

        /// <summary>
        /// Adds the default set of elements to this view. 
        /// </summary>
        public void AddDefaultElements()
        {
            AddAllDeploymentNodes();
        }

        /// <summary>
        /// Adds all of the top-level deployment nodes to this view, for the same deployment environment (if set). 
        /// </summary>
        public void AddAllDeploymentNodes()
        {
            foreach (DeploymentNode deploymentNode in Model.DeploymentNodes)
            {
                if (deploymentNode.Parent == null)
                {
                    if (this.Environment == null || this.Environment.Equals(deploymentNode.Environment))
                    {
                        Add(deploymentNode);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a deployment node to this view.
        /// </summary>
        /// <param name="deploymentNode">the DeploymentNode to add</param>
        public void Add(DeploymentNode deploymentNode)
        {
            Add(deploymentNode, true);
        }

        /// <summary>
        /// Adds a deployment node to this view.
        /// </summary>
        /// <param name="deploymentNode">the DeploymentNode to add</param>
        public void Add(DeploymentNode deploymentNode, bool addRelationships)
        {
            if (deploymentNode == null)
            {
                throw new ArgumentException("A deployment node must be specified.");
            }

            if (AddElementInstancesAndDeploymentNodesAndInfrastructureNodes(deploymentNode, addRelationships))
            {
                Element parent = deploymentNode.Parent;
                while (parent != null)
                {
                    AddElement(parent, addRelationships);
                    parent = parent.Parent;
                }
            }
        }
        
        /// <summary>
        /// Adds an infrastructure node (and its parent deployment nodes) to this view.
        /// </summary>
        /// <param name="infrastructureNode">the InfrastructureNode to add</param>
        public void Add(InfrastructureNode infrastructureNode) {
            AddElement(infrastructureNode, true);
            DeploymentNode parent = (DeploymentNode)infrastructureNode.Parent;
            while (parent != null)
            {
                AddElement(parent, true);
                parent = (DeploymentNode)parent.Parent;
            }
        }

        /// <summary>
        /// Adds a software system instance (and its parent deployment nodes) to this view.
        /// </summary>
        /// <param name="softwareSystemInstance">the SoftwareSystemInstance to add</param>
        public void Add(SoftwareSystemInstance softwareSystemInstance)
        {
            AddElement(softwareSystemInstance, true);
            DeploymentNode parent = (DeploymentNode)softwareSystemInstance.Parent;
            while (parent != null)
            {
                AddElement(parent, true);
                parent = (DeploymentNode)parent.Parent;
            }
        }

        /// <summary>
        /// Adds a container instance (and its parent deployment nodes) to this view.
        /// </summary>
        /// <param name="containerInstance">the ContainerInstance to add</param>
        public void Add(ContainerInstance containerInstance)
        {
            AddElement(containerInstance, true);
            DeploymentNode parent = (DeploymentNode)containerInstance.Parent;
            while (parent != null)
            {
                AddElement(parent, true);
                parent = (DeploymentNode)parent.Parent;
            }
        }

        private bool AddElementInstancesAndDeploymentNodesAndInfrastructureNodes(DeploymentNode deploymentNode, bool addRelationships)
        {
            bool hasElementsInstancesOrInfrastructureNodes = false;
            
            foreach (SoftwareSystemInstance softwareSystemInstance in deploymentNode.SoftwareSystemInstances)
            {
                try {
                    AddElement(softwareSystemInstance, addRelationships);
                    hasElementsInstancesOrInfrastructureNodes = true;
                } catch (ElementNotPermittedInViewException e) {
                    // the element can't be added, so ignore it
                }
            }

            foreach (ContainerInstance containerInstance in deploymentNode.ContainerInstances)
            {
                Container container = containerInstance.Container;
                if (SoftwareSystem == null || container.Parent.Equals(SoftwareSystem))
                {
                    try
                    {
                        AddElement(containerInstance, addRelationships);
                        hasElementsInstancesOrInfrastructureNodes = true;
                    } catch (ElementNotPermittedInViewException e) {
                        // the element can't be added, so ignore it
                    }
                }
            }

            foreach (InfrastructureNode infrastructureNode in deploymentNode.InfrastructureNodes)
            {
                AddElement(infrastructureNode, addRelationships);
                hasElementsInstancesOrInfrastructureNodes = true;
            }

            foreach (DeploymentNode child in deploymentNode.Children)
            {
                hasElementsInstancesOrInfrastructureNodes = hasElementsInstancesOrInfrastructureNodes | AddElementInstancesAndDeploymentNodesAndInfrastructureNodes(child, addRelationships);
            }

            if (hasElementsInstancesOrInfrastructureNodes)
            {
                AddElement(deploymentNode, addRelationships);
            }

            return hasElementsInstancesOrInfrastructureNodes;
        }

        /// <summary>
        /// Removes a deployment node from this view.
        /// </summary>
        /// <param name="deploymentNode">the DeploymentNode to remove</param>
        public void Remove(DeploymentNode deploymentNode)
        {
            foreach (SoftwareSystemInstance softwareSystemInstance in deploymentNode.SoftwareSystemInstances)
            {
                Remove(softwareSystemInstance);
            }

            foreach (ContainerInstance containerInstance in deploymentNode.ContainerInstances)
            {
                Remove(containerInstance);
            }

            foreach (InfrastructureNode infrastructureNode in deploymentNode.InfrastructureNodes)
            {
                Remove(infrastructureNode);
            }

            foreach (DeploymentNode child in deploymentNode.Children)
            {
                Remove(child);
            }

            RemoveElement(deploymentNode);
        }

        /// <summary>
        /// Removes an infrastructure node from this view.
        /// </summary>
        /// <param name="infrastructureNode">the InfrastructureNode to remove</param>

        public void Remove(InfrastructureNode infrastructureNode)
        {
            RemoveElement(infrastructureNode);
        }

        /// <summary>
        /// Removes a software system instance from this view.
        /// </summary>
        /// <param name="softwareSystemInstance">the SoftwareSystemInstance to remove</param>
        public void Remove(SoftwareSystemInstance softwareSystemInstance)
        {
            RemoveElement(softwareSystemInstance);
        }

        /// <summary>
        /// Removes a container instance from this view.
        /// </summary>
        /// <param name="containerInstance">the ContainerInstance to remove</param>
        public void Remove(ContainerInstance containerInstance)
        {
            RemoveElement(containerInstance);
        }

        /// <summary>
        /// Adds a specific relationship to this view.
        /// </summary>
        /// <param name="relationship">the Relationship to be added</param>
        /// <returns>a RelationshipView object representing the relationship added</returns>
        public RelationshipView Add(Relationship relationship)
        {
            return AddRelationship(relationship);
        }
        
        /// <summary>
        /// Adds an animation step, with the specified container instances and infrastructure nodes.
        /// </summary>
        /// <param name="elementInstances">the software system/container instances that should be shown in the animation step</param>
        /// <param name="infrastructureNodes">the infrastructure nodes that should be shown in the animation step</param>
        public void AddAnimation(StaticStructureElementInstance[] elementInstances, InfrastructureNode[] infrastructureNodes)
        {
            if ((elementInstances == null || elementInstances.Length == 0) && (infrastructureNodes == null || infrastructureNodes.Length == 0))
            {
                throw new ArgumentException("One or more software system/container instances and/or infrastructure nodes must be specified.");
            }

            List<Element> elements = new List<Element>();
            if (elementInstances != null)
            {
                elements.AddRange(elementInstances);
            }
            if (infrastructureNodes != null)
            {
                elements.AddRange(infrastructureNodes);
            }

            addAnimationStep(elements.ToArray());
        }

        /// <summary>
        /// Adds an animation step, with the specified infrastructure nodes.
        /// </summary>
        /// <param name="infrastructureNodes">the infrastructure nodes that should be shown in the animation step</param>
        public void AddAnimation(params InfrastructureNode[] infrastructureNodes)
        {
            if (infrastructureNodes == null || infrastructureNodes.Length == 0)
            {
                throw new ArgumentException("One or more infrastructure nodes must be specified.");
            }

            AddAnimation(new ContainerInstance[0], infrastructureNodes);
        }

        /// <summary>
        /// Adds an animation step, with the specified container instances.
        /// </summary>
        /// <param name="elementInstances">the software system/container instances that should be shown in the animation step</param>
        public void AddAnimation(params StaticStructureElementInstance[] elementInstances)
        {
            if (elementInstances == null || elementInstances.Length == 0)
            {
                throw new ArgumentException("One or more software system/container instances must be specified.");
            }

            AddAnimation(elementInstances, new InfrastructureNode[0]);
        }

        private void addAnimationStep(params Element[] elements)
        {
            ISet<string> elementIdsInPreviousAnimationSteps = new HashSet<string>();
            foreach (Animation animationStep in Animations) {
                foreach (string element in animationStep.Elements)
                {
                    elementIdsInPreviousAnimationSteps.Add(element);
                }
            }

            ISet<Element> elementsInThisAnimationStep = new HashSet<Element>();
            ISet<Relationship> relationshipsInThisAnimationStep = new HashSet<Relationship>();

            foreach (Element element in elements)
            {
                if (IsElementInView(element) && !elementIdsInPreviousAnimationSteps.Contains(element.Id))
                {
                    elementIdsInPreviousAnimationSteps.Add(element.Id);
                    elementsInThisAnimationStep.Add(element);

                    Element deploymentNode = findDeploymentNode(element);
                    while (deploymentNode != null)
                    {
                        if (!elementIdsInPreviousAnimationSteps.Contains(deploymentNode.Id))
                        {
                            elementIdsInPreviousAnimationSteps.Add(deploymentNode.Id);
                            elementsInThisAnimationStep.Add(deploymentNode);
                        }

                        deploymentNode = deploymentNode.Parent;
                    }
                }
            }

            if (elementsInThisAnimationStep.Count == 0)
            {
                throw new ArgumentException("None of the specified container instances exist in this view.");
            }

            foreach (RelationshipView relationshipView in Relationships)
            {
                if (
                        (elementsInThisAnimationStep.Contains(relationshipView.Relationship.Source) && elementIdsInPreviousAnimationSteps.Contains(relationshipView.Relationship.Destination.Id)) ||
                        (elementIdsInPreviousAnimationSteps.Contains(relationshipView.Relationship.Source.Id) && elementsInThisAnimationStep.Contains(relationshipView.Relationship.Destination))
                )
                {
                    relationshipsInThisAnimationStep.Add(relationshipView.Relationship);
                }
            }

            _animations.Add(new Animation(Animations.Count + 1, elementsInThisAnimationStep, relationshipsInThisAnimationStep));
        }

        
        private DeploymentNode findDeploymentNode(Element e)
        {
            foreach (Element element in Model.GetElements())
            {
                if (element is DeploymentNode)
                {
                    DeploymentNode deploymentNode = (DeploymentNode) element;

                    if (e is ContainerInstance)
                    {
                        if (deploymentNode.ContainerInstances.Contains(e))
                        {
                            return deploymentNode;
                        }
                    }

                    if (e is InfrastructureNode)
                    {
                        if (deploymentNode.InfrastructureNodes.Contains(e))
                        {
                            return deploymentNode;
                        }
                    }
                }
            }

            return null;
        }

        public override string Name
        {
            get
            {
                string name;

                if (SoftwareSystem != null)
                {
                    name = SoftwareSystem.Name + " - Deployment";
                }
                else
                {
                    name = "Deployment";
                }

                if (!String.IsNullOrEmpty(Environment))
                {
                    name = name + " - " + Environment;
                }

                return name;

            }
        }
        
    }
}