using System.Text;

namespace Structurizr
{
    internal class CanonicalNameGenerator
    {

        private const string CustomElementType = "Custom://";
        private const string PersonType = "Person://";
        private const string SoftwareSystemType = "SoftwareSystem://";
        private const string ContainerType = "Container://";
        private const string ComponentType = "Component://";

        private const string DeploymentNodeType = "DeploymentNode://";
        private const string InfrastructureNodeType = "InfrastructureNode://";
        private const string ContainerInstanceType = "ContainerInstance://";
        private const string SoftwareSystemInstanceType = "SoftwareSystemInstance://";

        private const string StaticCanonicalNameSeperator = ".";
        private const string DeploymentCanonicalNameSeperator = "/";

        private string formatName(Element element)
        {
            return formatName(element.Name);
        }

        private string formatName(string name)
        {
            return name
                .Replace(StaticCanonicalNameSeperator, "")
                .Replace(DeploymentCanonicalNameSeperator, "");
        }

        internal string Generate(Person person)
        {
            return PersonType + formatName(person);
        }

        internal string Generate(SoftwareSystem softwareSystem)
        {
            return SoftwareSystemType + formatName(softwareSystem);
        }

        internal string Generate(Container container)
        {
            return ContainerType + formatName(container.SoftwareSystem) + StaticCanonicalNameSeperator + formatName(container);
        }

        internal string Generate(Component component)
        {
            return ComponentType + formatName(component.Container.SoftwareSystem) + StaticCanonicalNameSeperator + formatName(component.Container) + StaticCanonicalNameSeperator + formatName(component);
        }

        internal string Generate(DeploymentNode deploymentNode)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(DeploymentNodeType);

            buf.Append(formatName(deploymentNode.Environment));
            buf.Append(DeploymentCanonicalNameSeperator);

            DeploymentNode parent = (DeploymentNode)deploymentNode.Parent;
            while (parent != null)
            {
                buf.Append(formatName(parent));
                buf.Append(DeploymentCanonicalNameSeperator);
                parent = (DeploymentNode)parent.Parent;
            }

            buf.Append(formatName(deploymentNode));

            return buf.ToString();
        }

        internal string Generate(InfrastructureNode infrastructureNode)
        {
            string deploymentNodeCanonicalName = Generate((DeploymentNode)infrastructureNode.Parent).Substring(DeploymentNodeType.Length);

            return InfrastructureNodeType + deploymentNodeCanonicalName + DeploymentCanonicalNameSeperator + formatName(infrastructureNode);
        }

        internal string Generate(SoftwareSystemInstance softwareSystemInstance)
        {
            string deploymentNodeCanonicalName = Generate((DeploymentNode)softwareSystemInstance.Parent).Substring(DeploymentNodeType.Length);

            return SoftwareSystemInstanceType + deploymentNodeCanonicalName + DeploymentCanonicalNameSeperator + formatName(softwareSystemInstance.SoftwareSystem) + "[" + softwareSystemInstance.InstanceId + "]";
        }

        internal string Generate(ContainerInstance containerInstance)
        {
            string deploymentNodeCanonicalName = Generate((DeploymentNode)containerInstance.Parent).Substring(DeploymentNodeType.Length);

            return ContainerInstanceType + deploymentNodeCanonicalName + DeploymentCanonicalNameSeperator + Generate(containerInstance.Container).Substring(ContainerType.Length) + "[" + containerInstance.InstanceId + "]";
        }

    }
}
