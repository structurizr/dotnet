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
        }

        internal DeploymentView(Model model, string key, string description) : base(null, key, description)
        {
            Model = model;
        }

        internal DeploymentView(SoftwareSystem softwareSystem, string key, string description) : base(softwareSystem,
            key, description)
        {
            Model = softwareSystem.Model;
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

            if (AddContainerInstancesAndDeploymentNodesAndInfrastructureNodes(deploymentNode, addRelationships))
            {
                Element parent = deploymentNode.Parent;
                while (parent != null)
                {
                    AddElement(parent, false);
                    parent = parent.Parent;
                }
            }
        }

        private bool AddContainerInstancesAndDeploymentNodesAndInfrastructureNodes(DeploymentNode deploymentNode, bool addRelationships)
        {
            bool hasContainersOrInfrastructureNodes = false;
            foreach (ContainerInstance containerInstance in deploymentNode.ContainerInstances) {
                Container container = containerInstance.Container;
                if (SoftwareSystem == null || container.Parent.Equals(SoftwareSystem))
                {
                    AddElement(containerInstance, addRelationships);
                    hasContainersOrInfrastructureNodes = true;
                }
            }

            foreach (InfrastructureNode infrastructureNode in deploymentNode.InfrastructureNodes) {
                AddElement(infrastructureNode, addRelationships);
                hasContainersOrInfrastructureNodes = true;
            }

            foreach (DeploymentNode child in deploymentNode.Children)
            {
                hasContainersOrInfrastructureNodes = hasContainersOrInfrastructureNodes | AddContainerInstancesAndDeploymentNodesAndInfrastructureNodes(child, addRelationships);
            }

            if (hasContainersOrInfrastructureNodes)
            {
                AddElement(deploymentNode, addRelationships);
            }

            return hasContainersOrInfrastructureNodes;
        }

        /// <summary>
        /// Removes a deployment node from this view.
        /// </summary>
        /// <param name="deploymentNode">the DeploymentNode to remove</param>
        public void Remove(DeploymentNode deploymentNode)
        {
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
        /// Removes an container instance from this view.
        /// </summary>
        /// <param name="containerInstance">the ContainerInstance to remove</param>
        public void Remove(ContainerInstance containerInstance)
        {
            RemoveElement(containerInstance);
        }

        /// <summary>
        /// Adds an animation step, with the specified container instances and infrastructure nodes.
        /// </summary>
        /// <param name="containerInstances">the container instances that should be shown in the animation step</param>
        /// <param name="infrastructureNodes">the infrastructure nodes that should be shown in the animation step</param>
        public void AddAnimation(ContainerInstance[] containerInstances, InfrastructureNode[] infrastructureNodes)
        {
            if ((containerInstances == null || containerInstances.Length == 0) && (infrastructureNodes == null || infrastructureNodes.Length == 0))
            {
                throw new ArgumentException("One or more container instances/infrastructure nodes must be specified.");
            }

            List<Element> elements = new List<Element>();
            if (containerInstances != null)
            {
                elements.AddRange(containerInstances);
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
        /// <param name="containerInstances">the container instances that should be shown in the animation step</param>
        public void AddAnimation(params ContainerInstance[] containerInstances)
        {
            if (containerInstances == null || containerInstances.Length == 0)
            {
                throw new ArgumentException("One or more container instances must be specified.");
            }

            AddAnimation(containerInstances, new InfrastructureNode[0]);
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