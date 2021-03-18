using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{
    
    /// <summary>
    /// Represents a deployment instance of a Container, which can be added to a DeploymentNode.
    /// </summary>
    [DataContract]
    public sealed class ContainerInstance : StaticStructureElementInstance
    {

        public Container Container { get; internal set; }

        private string _containerId;

        [DataMember(Name = "containerId", EmitDefaultValue = false)]
        public string ContainerId
        {
            get
            {
                if (Container != null)
                {
                    return Container.Id;
                }
                else
                {
                    return _containerId;
                }
            }
            set { _containerId = value; }
        }

        internal ContainerInstance() {
        }

        internal ContainerInstance(Container container, int instanceId, string environment, string deploymentGroup)
        {
            Container = container;
            AddTags(Structurizr.Tags.ContainerInstance);
            InstanceId = instanceId;
            Environment = environment;
            DeploymentGroup = deploymentGroup;
        }

        public override StaticStructureElement getElement()
        {
            return Container;
        }

        public override string CanonicalName
        {
            get { return new CanonicalNameGenerator().Generate(this); }
        }

    }

}