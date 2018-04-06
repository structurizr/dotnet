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
    public sealed class ViewSet
    {

        public Model Model { get; set; }

        /// <summary>
        /// The set of enterprise context views.
        /// </summary>
        [DataMember(Name = "enterpriseContextViews", EmitDefaultValue = false)]
        public ICollection<EnterpriseContextView> EnterpriseContextViews { get; set; }

        /// <summary>
        /// The set of system context views.
        /// </summary>
        [DataMember(Name = "systemContextViews", EmitDefaultValue = false)]
        public ISet<SystemContextView> SystemContextViews { get; set; }

        /// <summary>
        /// The set of container views.
        /// </summary>
        [DataMember(Name = "containerViews", EmitDefaultValue = false)]
        public ISet<ContainerView> ContainerViews { get; set; }

        /// <summary>
        /// The set of component views.
        /// </summary>
        [DataMember(Name = "componentViews", EmitDefaultValue = false)]
        public ISet<ComponentView> ComponentViews { get; set; }

        /// <summary>
        /// The set of dynamic views.
        /// </summary>
        [DataMember(Name = "dynamicViews", EmitDefaultValue = false)]
        public ISet<DynamicView> DynamicViews { get; set; }

        /// <summary>
        /// The set of deployment views.
        /// </summary>
        [DataMember(Name = "deploymentViews", EmitDefaultValue = false)]
        public ISet<DeploymentView> DeploymentViews { get; set; }

        /// <summary>
        /// The set of filtered views.
        /// </summary>
        [DataMember(Name = "filteredViews", EmitDefaultValue = false)]
        public ISet<FilteredView> FilteredViews { get; set; }

        /// <summary>
        /// The configuration object associated with this set of views.
        /// </summary>
        [DataMember(Name = "configuration", EmitDefaultValue = false)]
        public Configuration Configuration { get; set; }

        internal ViewSet()
        {
            EnterpriseContextViews = new HashSet<EnterpriseContextView>();
            SystemContextViews = new HashSet<SystemContextView>();
            ContainerViews = new HashSet<ContainerView>();
            ComponentViews = new HashSet<ComponentView>();
            DynamicViews = new HashSet<DynamicView>();
            DeploymentViews = new HashSet<DeploymentView>();
            FilteredViews = new HashSet<FilteredView>();

            Configuration = new Configuration();
        }

        internal ViewSet(Model model) : this()
        {
            Model = model;
        }

        public EnterpriseContextView CreateEnterpriseContextView(string key, string description)
        {
            AssertThatTheViewKeyIsUnique(key);

            EnterpriseContextView view = new EnterpriseContextView(Model, key, description);
            EnterpriseContextViews.Add(view);
            return view;
        }

        public SystemContextView CreateSystemContextView(SoftwareSystem softwareSystem, string key, string description)
        {
            AssertThatTheViewKeyIsUnique(key);

            SystemContextView view = new SystemContextView(softwareSystem, key, description);
            SystemContextViews.Add(view);

            return view;
        }

        public ContainerView CreateContainerView(SoftwareSystem softwareSystem, string key, string description)
        {
            AssertThatTheViewKeyIsUnique(key);

            ContainerView view = new ContainerView(softwareSystem, key, description);
            ContainerViews.Add(view);

            return view;
        }

        public ComponentView CreateComponentView(Container container, string key, string description)
        {
            AssertThatTheViewKeyIsUnique(key);

            ComponentView view = new ComponentView(container, key, description);
            ComponentViews.Add(view);

            return view;
        }

        public DynamicView CreateDynamicView(string key, string description)
        {
            AssertThatTheViewKeyIsUnique(key);

            DynamicView view = new DynamicView(Model, key, description);
            DynamicViews.Add(view);
            return view;
        }

        public DynamicView CreateDynamicView(SoftwareSystem softwareSystem, string key, string description)
        {
            AssertThatTheSoftwareSystemIsNotNull(softwareSystem);
            AssertThatTheViewKeyIsUnique(key);

            DynamicView view = new DynamicView(softwareSystem, key, description);
            DynamicViews.Add(view);
            return view;
        }

        public DynamicView CreateDynamicView(Container container, string key, string description)
        {
            AssertThatTheContainerIsNotNull(container);
            AssertThatTheViewKeyIsUnique(key);

            DynamicView view = new DynamicView(container, key, description);
            DynamicViews.Add(view);
            return view;
        }
        
        /// <summary>
        /// Creates a deployment view.
        /// </summary>
        /// <returns>a DeploymentView object</returns>
        public DeploymentView CreateDeploymentView(String key, String description) {
            AssertThatTheViewKeyIsUnique(key);

            DeploymentView view = new DeploymentView(Model, key, description);
            DeploymentViews.Add(view);
            return view;
        }

        /// <summary>
        /// Creates a deployment view, where the scope of the view is the specified software system.
        /// </summary>
        /// <param name="softwareSystem">the SoftwareSystem object representing the scope of the view</param>
        /// <param name="key">the key for the deployment view (must be unique)</param>
        /// <param name="description">a description of the  view</param>
        /// <returns>a DeploymentView object</returns>
        public DeploymentView CreateDeploymentView(SoftwareSystem softwareSystem, String key, String description) {
            AssertThatTheSoftwareSystemIsNotNull(softwareSystem);
            AssertThatTheViewKeyIsUnique(key);

            DeploymentView view = new DeploymentView(softwareSystem, key, description);
            DeploymentViews.Add(view);
            return view;
        }


        /// <summary>
        /// Creates a FilteredView on top of an existing static view. 
        /// </summary>
        /// <param name="view">the static view to base the FilteredView upon</param>
        /// <param name="key">the key for the filtered view (must be unique)</param>
        /// <param name="description">a description of the filtered view</param>
        /// <param name="mode">whether to Include or Exclude elements/relationships based upon their tag</param>
        /// <param name="tags">the tags to include or exclude</param>
        /// <returns></returns>
        public FilteredView CreateFilteredView(StaticView view, string key, string description, FilterMode mode, params string[] tags)
        {
            AssertThatTheViewKeyIsUnique(key);

            FilteredView filteredView = new FilteredView(view, key, description, mode, tags);
            FilteredViews.Add(filteredView);
            
            return filteredView;
        }

        private void AssertThatTheViewKeyIsUnique(string key)
        {
            if (GetViewWithKey(key) != null || GetFilteredViewWithKey(key) != null)
            {
                throw new ArgumentException("A view with the key " + key + " already exists.");
            }
        }
        
        private void AssertThatTheSoftwareSystemIsNotNull(SoftwareSystem softwareSystem)
        {
            if (softwareSystem == null)
            {
                throw new ArgumentException("Software system must not be null.");
            }
        }

        private void AssertThatTheContainerIsNotNull(Container container)
        {
            if (container == null)
            {
                throw new ArgumentException("Container must not be null.");
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
            
            foreach (DeploymentView view in DeploymentViews)
            {
                if (!String.IsNullOrEmpty(view.SoftwareSystemId))
                {
                    view.SoftwareSystem = Model.GetSoftwareSystemWithId(view.SoftwareSystemId);
                }
                view.Model = Model;
                HydrateView(view);
            }
            
            foreach (FilteredView filteredView in FilteredViews)
            {
                filteredView.View = GetViewWithKey(filteredView.BaseViewKey);
            }
        }

        private void HydrateView(View view)
        {
            view.SoftwareSystem = Model.GetElement(view.SoftwareSystemId) as SoftwareSystem;

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
            
            foreach (DeploymentView sourceView in source.DeploymentViews)
            {
                DeploymentView destinationView = FindDeploymentView(sourceView);
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

        private DeploymentView FindDeploymentView(DeploymentView deploymentView)
        {
            return DeploymentViews.FirstOrDefault(view => view.Key == deploymentView.Key);
        }

        /// <summary>
        /// Finds the view with the specified key, or null if the view does not exist.
        /// </summary>
        public View GetViewWithKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentException("A key must be specified.");
            }
            
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

            foreach (DeploymentView view in DeploymentViews)
            {
                if (view.Key.Equals(key))
                {
                    return view;
                }
            }

            return null;
        }

        public FilteredView GetFilteredViewWithKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentException("A key must be specified.");
            }

            foreach (FilteredView view in FilteredViews)
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
