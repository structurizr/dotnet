using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Structurizr
{
    
    /// <summary>
    /// Represents a deployment node, which is something like:
    ///  - Physical infrastructure (e.g. a physical server or device)
    ///  - Virtualised infrastructure (e.g. IaaS, PaaS, a virtual machine)
    ///  - Containerised infrastructure (e.g. a Docker container)
    ///  - Database server
    ///  - Java EE web/application server
    ///  - Microsoft IIS
    ///  - etc
    /// </summary>
    [DataContract]
    public sealed class DeploymentNode : DeploymentElement
    {

        private DeploymentNode _parent;

        /// <summary>
        /// The parent DeploymentNode, or null if there is no parent.
        /// </summary>
        public override Element Parent
        {
            get { return _parent; }
            set { _parent = value as DeploymentNode; }
        }
            
        [DataMember(Name = "technology", EmitDefaultValue = false)]
        public string Technology { get; set; }

        private int _instances = 1;

        [DataMember(Name = "instances", EmitDefaultValue = false)]
        public int Instances
        {
            get
            {
                return _instances;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Number of instances must be a positive integer.");
                }

                _instances = value;
            }
        }

        private HashSet<DeploymentNode> _children;

        /// <summary>
        /// The set of child deployment nodes.
        /// </summary>
        [DataMember(Name = "children", EmitDefaultValue = false)]
        public ISet<DeploymentNode> Children
        {
            get
            {
                return new HashSet<DeploymentNode>(_children);
            }

            internal set
            {
                _children = new HashSet<DeploymentNode>(value);
            }
        }

        private HashSet<InfrastructureNode> _infrastructureNodes;

        /// <summary>
        /// The set of infrastructure nodes associated with this deployment node.
        /// </summary>
        [DataMember(Name = "infrastructureNodes", EmitDefaultValue = false)]
        public ISet<InfrastructureNode> InfrastructureNodes
        {
            get
            {
                return new HashSet<InfrastructureNode>(_infrastructureNodes);
            }

            internal set
            {
                _infrastructureNodes = new HashSet<InfrastructureNode>(value);
            }
        }
        
        private HashSet<SoftwareSystemInstance> _softwareSystemInstances;
        
        /// <summary>
        /// The set of software system instances associated with this deployment node.
        /// </summary>
        [DataMember(Name = "softwareSystemInstances", EmitDefaultValue = false)]
        public ISet<SoftwareSystemInstance> SoftwareSystemInstances
        {
            get
            {
                return new HashSet<SoftwareSystemInstance>(_softwareSystemInstances);
            }

            internal set
            {
                _softwareSystemInstances = new HashSet<SoftwareSystemInstance>(value);
            }
        }

        private HashSet<ContainerInstance> _containerInstances;
        
        /// <summary>
        /// The set of container instances associated with this deployment node.
        /// </summary>
        [DataMember(Name = "containerInstances", EmitDefaultValue = false)]
        public ISet<ContainerInstance> ContainerInstances
        {
            get
            {
                return new HashSet<ContainerInstance>(_containerInstances);
            }

            internal set
            {
                _containerInstances = new HashSet<ContainerInstance>(value);
            }
        }

        internal DeploymentNode()
        {
            Instances = 1;
            _children = new HashSet<DeploymentNode>();
            _infrastructureNodes = new HashSet<InfrastructureNode>();
            _softwareSystemInstances = new HashSet<SoftwareSystemInstance>();
            _containerInstances = new HashSet<ContainerInstance>();
            Environment = DefaultDeploymentEnvironment;
        }

        public override List<string> GetRequiredTags()
        {
            return new List<string>
            {
                Structurizr.Tags.Element,
                Structurizr.Tags.DeploymentNode
            };
        }

        public override string CanonicalName
        {
            get
            {
                return new CanonicalNameGenerator().Generate(this);
            }
        }

        /// <summary>
        /// Adds a software system instance to this deployment node, replicating relationships.
        /// </summary>
        /// <param name="softwareSystem">the SoftwareSystem to add an instance of</param>
        /// <returns>a SoftwareSystemInstance object</returns>
        public SoftwareSystemInstance Add(SoftwareSystem softwareSystem)
        {
            return Add(softwareSystem, true);
        }

        /// <summary>
        /// Adds a software system instance to this deployment node, optionally replicating relationships.
        /// </summary>
        /// <param name="softwareSystem">the SoftwareSystem to add an instance of</param>
        /// <param name="replicateRelationships">true if relationships should be replicated between the element instances in the same deployment environment, false otherwise</param>
        /// <returns>a SoftwareSystemInstance object</returns>
        public SoftwareSystemInstance Add(SoftwareSystem softwareSystem, bool replicateRelationships)
        {
            if (softwareSystem == null)
            {
                throw new ArgumentException("A software system must be specified.");
            }

            SoftwareSystemInstance softwareSystemInstance = Model.AddSoftwareSystemInstance(this, softwareSystem, replicateRelationships);
            _softwareSystemInstances.Add(softwareSystemInstance);
    
            return softwareSystemInstance;
        }

        /// <summary>
        /// Adds a container instance to this deployment node, replicating relationships.
        /// </summary>
        /// <param name="container">the Container to add an instance of</param>
        /// <returns>a ContainerInstance object</returns>
        public ContainerInstance Add(Container container)
        {
            return Add(container, true);
        }

        /// <summary>
        /// Adds a container instance to this deployment node.
        /// </summary>
        /// <param name="container">the Container to add an instance of</param>
        /// <returns>a ContainerInstance object</returns>
        public ContainerInstance Add(Container container, bool replicateRelationships)
        {
            if (container == null)
            {
                throw new ArgumentException("A container must be specified.");
            }

            ContainerInstance containerInstance = Model.AddContainerInstance(this, container, replicateRelationships);
            _containerInstances.Add(containerInstance);
    
            return containerInstance;
        }

        /// <summary>
        /// Adds a child deployment node.
        /// </summary>
        /// <param name="name">the name of the deployment node</param>
        /// <returns></returns>
        public DeploymentNode AddDeploymentNode(string name) {
            return AddDeploymentNode(name, null, null);
        }
    
        /// <summary>
        /// Adds a child deployment node.
        /// </summary>
        /// <param name="name">the name of the deployment node</param>
        /// <param name="description">a short description</param>
        /// <param name="technology">the technology</param>
        /// <returns></returns>
        public DeploymentNode AddDeploymentNode(string name, string description, string technology) {
            return AddDeploymentNode(name, description, technology, 1);
        }
    
        /// <summary>
        /// Adds a child deployment node.
        /// </summary>
        /// <param name="name">the name of the deployment node</param>
        /// <param name="description">a short description</param>
        /// <param name="technology">the technology</param>
        /// <param name="instances">the number of  instances</param>
        /// <returns></returns>
        public DeploymentNode AddDeploymentNode(string name, string description, string technology, int instances) {
            return AddDeploymentNode(name, description, technology, instances, null);
        }
    
        /// <summary>
        /// Adds a child deployment node.
        /// </summary>
        /// <param name="name">the name of the deployment node</param>
        /// <param name="description">a short description</param>
        /// <param name="technology">the technology</param>
        /// <param name="instances">the number of  instances</param>
        /// <param name="properties">a Dictionary (string,string) describing name=value properties</param>
        /// <returns></returns>
        public DeploymentNode AddDeploymentNode(string name, string description, string technology, int instances, Dictionary<string,string> properties) {
            DeploymentNode deploymentNode = Model.AddDeploymentNode(this, this.Environment, name, description, technology, instances, properties);
            if (deploymentNode != null) {
                _children.Add(deploymentNode);
            }
            
            return deploymentNode;
        }

        /// <summary>
        /// Gets the DeploymentNode with the specified name.
        /// </summary>
        /// <param name="name">the name of the deployment node</param>
        /// <returns>the DeploymentNode instance with the specified name (or null if it doesn't exist)</returns>
        public DeploymentNode GetDeploymentNodeWithName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A name must be specified.");
            }

            return _children.FirstOrDefault(dn => dn.Name.Equals(name));
        }

        /// <summary>
        /// Gets the InfrastructureNode with the specified name.
        /// </summary>
        /// <param name="name">the name of the infrastructure node</param>
        /// <returns>the InfrastructureNode instance with the specified name (or null if it doesn't exist)</returns>
        public InfrastructureNode GetInfrastructureNodeWithName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A name must be specified.");
            }

            return _infrastructureNodes.FirstOrDefault(dn => dn.Name.Equals(name));
        }

        /// <summary>
        /// Adds a child infrastructure node.
        /// </summary>
        /// <param name="name">the name of the infrastructure node</param>
        /// <returns>an InfrastructureNode object</returns>
        public InfrastructureNode AddInfrastructureNode(string name)
        {
            return AddInfrastructureNode(name, null, null);
        }

        /// <summary>
        /// Adds a child infrastructure node.
        /// </summary>
        /// <param name="name">the name of the infrastructure node</param>
        /// <param name="description">a short description</param>
        /// <param name="technology">the technology</param>
        /// <returns>an InfrastructureNode object</returns>
        public InfrastructureNode AddInfrastructureNode(string name, string description, string technology)
        {
            return AddInfrastructureNode(name, description, technology, null);
        }

        /// <summary>
        /// Adds a child infrastructure node.
        /// </summary>
        /// <param name="name">the name of the infrastructure node</param>
        /// <param name="description">a short description</param>
        /// <param name="technology">the technology</param>
        /// <param name="properties">a Dictionary (string,string) describing name=value properties</param>
        /// <returns>an InfrastructureNode object</returns>
        public InfrastructureNode AddInfrastructureNode(string name, string description, string technology, Dictionary<string, string> properties)
        {
            InfrastructureNode infrastructureNode = Model.AddInfrastructureNode(this, name, description, technology, properties);
            if (infrastructureNode != null) {
                _infrastructureNodes.Add(infrastructureNode);
            }
            return infrastructureNode;
        }
        
        /// <summary>
        /// Adds a relationship between this and another deployment node.
        /// </summary>
        /// <param name="destination">the destination DeploymentNode</param>
        /// <param name="description">a short description of the relationship</param>
        /// <param name="technology">the technology</param>
        /// <returns>a Relationship object</returns>
        public Relationship Uses(DeploymentNode destination, string description, string technology)
        {
            return Model.AddRelationship(this, destination, description, technology);
        }

    }
    
}