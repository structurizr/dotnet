using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Structurizr
{
    
    /// <summary>
    /// Represents a deployment instance of a Software System, which can be added to a DeploymentNode.
    /// </summary>
    [DataContract]
    public sealed class SoftwareSystemInstance : StaticStructureElementInstance
    {

        public SoftwareSystem SoftwareSystem { get; internal set; }

        private string _softwareSystemId;

        [DataMember(Name = "softwareSystemId", EmitDefaultValue = false)]
        public string SoftwareSystemId
        {
            get
            {
                if (SoftwareSystem != null)
                {
                    return SoftwareSystem.Id;
                }
                else
                {
                    return _softwareSystemId;
                }
            }
            set { _softwareSystemId = value; }
        }

        internal SoftwareSystemInstance() {
        }

        internal SoftwareSystemInstance(SoftwareSystem softwareSystem, int instanceId, string environment)
        {
            SoftwareSystem = softwareSystem;
            AddTags(Structurizr.Tags.SoftwareSystemInstance);
            InstanceId = instanceId;
            Environment = environment;
        }

        public override StaticStructureElement getElement()
        {
            return SoftwareSystem;
        }

        public override string CanonicalName
        {
            get { return new CanonicalNameGenerator().Generate(this); }
        }

    }

}