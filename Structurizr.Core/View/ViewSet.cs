using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Structurizr
{

    /// <summary>
    /// The set of views onto a software architecture model.
    /// </summary>
    [DataContract]
    public class ViewSet
    {

        public Model Model { get; set; }

        /// <summary>
        /// The set of enterprise context views.
        /// </summary>
        [DataMember(Name = "enterpriseContextViews", EmitDefaultValue = false)]
        public List<EnterpriseContextView> EnterpriseContextViews { get; set; }

        /// <summary>
        /// The set of system context views.
        /// </summary>
        [DataMember(Name = "systemContextViews", EmitDefaultValue = false)]
        public List<SystemContextView> SystemContextViews { get; set; }

        /// <summary>
        /// The set of container views.
        /// </summary>
        [DataMember(Name = "containerViews", EmitDefaultValue = false)]
        public List<ContainerView> ContainerViews { get; set; }

        /// <summary>
        /// The set of component views.
        /// </summary>
        [DataMember(Name = "componentViews", EmitDefaultValue = false)]
        public List<ComponentView> ComponentViews { get; set; }

        /// <summary>
        /// The set of dynamic views.
        /// </summary>
        [DataMember(Name = "dynamicViews", EmitDefaultValue = false)]
        public List<DynamicView> DynamicViews { get; set; }

        /// <summary>
        /// The configuration object associated with this set of views.
        /// </summary>
        [DataMember(Name = "configuration", EmitDefaultValue = false)]
        public Configuration Configuration { get; set; }

        internal ViewSet()
        {
            this.EnterpriseContextViews = new List<EnterpriseContextView>();
            this.SystemContextViews = new List<SystemContextView>();
            this.ContainerViews = new List<ContainerView>();
            this.ComponentViews = new List<ComponentView>();
            this.DynamicViews = new List<DynamicView>();

            this.Configuration = new Configuration();
        }

        internal ViewSet(Model model) : this()
        {
            this.Model = model;
        }

        public EnterpriseContextView CreateEnterpriseContextView(string key, string description)
        {
            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                EnterpriseContextView view = new EnterpriseContextView(Model, key, description);
                EnterpriseContextViews.Add(view);
                return view;
            }
        }

        public SystemContextView CreateSystemContextView(SoftwareSystem softwareSystem, string key, string description)
        {
            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                SystemContextView view = new SystemContextView(softwareSystem, key, description);
                this.SystemContextViews.Add(view);

                return view;
            }
        }

        public ContainerView CreateContainerView(SoftwareSystem softwareSystem, string key, string description)
        {
            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                ContainerView view = new ContainerView(softwareSystem, key, description);
                ContainerViews.Add(view);

                return view;
            }
        }

        public ComponentView CreateComponentView(Container container, string key, string description)
        {
            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                ComponentView view = new ComponentView(container, key, description);
                ComponentViews.Add(view);

                return view;
            }
        }

        public DynamicView CreateDynamicView(string key, string description)
        {
            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                DynamicView view = new DynamicView(Model, key, description);
                DynamicViews.Add(view);
                return view;
            }
        }

        public DynamicView CreateDynamicView(SoftwareSystem softwareSystem, string key, string description)
        {
            if (softwareSystem == null)
            {
                throw new ArgumentException("Software system must not be null.");
            }

            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                DynamicView view = new DynamicView(softwareSystem, key, description);
                DynamicViews.Add(view);
                return view;
            }
        }

        public DynamicView CreateDynamicView(Container container, string key, string description)
        {
            if (container == null)
            {
                throw new ArgumentException("Container must not be null.");
            }

            if (GetViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
            else
            {
                DynamicView view = new DynamicView(container, key, description);
                DynamicViews.Add(view);
                return view;
            }
        }

        public void Hydrate()
        {
            foreach (EnterpriseContextView view in EnterpriseContextViews)
            {
                view.Model = Model;
                HydrateView(view);
            }

            foreach (SystemContextView view in SystemContextViews)
            {
                view.SoftwareSystem = Model.GetSoftwareSystemWithId(view.SoftwareSystemId);
                HydrateView(view);
            }

            foreach (ContainerView view in ContainerViews)
            {
                HydrateView(view);
            }

            foreach (ComponentView view in ComponentViews)
            {
                view.SoftwareSystem = Model.GetSoftwareSystemWithId(view.SoftwareSystemId);
                view.Container = view.SoftwareSystem.GetContainerWithId(view.ContainerId);
                HydrateView(view);
            }

            foreach (DynamicView view in DynamicViews)
            {
                view.Model = Model;
                HydrateView(view);
            }
        }

        private void HydrateView(View view)
        {
            foreach (ElementView elementView in view.Elements)
            {
                elementView.Element = Model.GetElement(elementView.Id);
            }
            foreach (RelationshipView relationshipView in view.Relationships)
            {
                relationshipView.Relationship = Model.GetRelationship(relationshipView.Id);
            }
        }

        public void CopyLayoutInformationFrom(ViewSet source)
        {
            foreach (EnterpriseContextView sourceView in source.EnterpriseContextViews)
            {
                EnterpriseContextView destinationView = FindEnterpriseContextView(sourceView);
                if (destinationView != null)
                {
                    destinationView.CopyLayoutInformationFrom(sourceView);
                }
            }

            foreach (SystemContextView sourceView in source.SystemContextViews)
            {
                SystemContextView destinationView = FindSystemContextView(sourceView);
                if (destinationView != null)
                {
                    destinationView.CopyLayoutInformationFrom(sourceView);
                }
            }

            foreach (ContainerView sourceView in source.ContainerViews)
            {
                ContainerView destinationView = FindContainerView(sourceView);
                if (destinationView != null)
                {
                    destinationView.CopyLayoutInformationFrom(sourceView);
                }
            }

            foreach (ComponentView sourceView in source.ComponentViews)
            {
                ComponentView destinationView = FindComponentView(sourceView);
                if (destinationView != null)
                {
                    destinationView.CopyLayoutInformationFrom(sourceView);
                }
            }

            foreach (DynamicView sourceView in source.DynamicViews)
            {
                DynamicView destinationView = FindDynamicView(sourceView);
                if (destinationView != null)
                {
                    destinationView.CopyLayoutInformationFrom(sourceView);
                }
            }
        }

        private EnterpriseContextView FindEnterpriseContextView(EnterpriseContextView enterpriseContextView)
        {
            return EnterpriseContextViews.FirstOrDefault(view => view.Key == enterpriseContextView.Key);
        }

        private SystemContextView FindSystemContextView(SystemContextView systemContextView)
        {
            return SystemContextViews.FirstOrDefault(view => view.Key == systemContextView.Key);
        }

        private ContainerView FindContainerView(ContainerView containerView)
        {
            return ContainerViews.FirstOrDefault(view => view.Key == containerView.Key);
        }

        private ComponentView FindComponentView(ComponentView componentView)
        {
            return ComponentViews.FirstOrDefault(view => view.Key == componentView.Key);
        }

        private DynamicView FindDynamicView(DynamicView dynamicView)
        {
            return DynamicViews.FirstOrDefault(view => view.Key == dynamicView.Key);
        }

        /// <summary>
        /// Finds the view with the specified key, or null if the view does not exist.
        /// </summary>
        public View GetViewWithKey(string key)
        {
            foreach (EnterpriseContextView view in EnterpriseContextViews)
            {
                if (view.Key.Equals(key))
                {
                    return view;
                }
            }

            foreach (SystemContextView view in SystemContextViews)
            {
                if (view.Key.Equals(key))
                {
                    return view;
                }
            }

            foreach (ContainerView view in ContainerViews)
            {
                if (view.Key.Equals(key))
                {
                    return view;
                }
            }

            foreach (ComponentView view in ComponentViews)
            {
                if (view.Key.Equals(key))
                {
                    return view;
                }
            }

            foreach (DynamicView view in DynamicViews)
            {
                if (view.Key.Equals(key))
                {
                    return view;
                }
            }

            return null;
        }


    }
}
