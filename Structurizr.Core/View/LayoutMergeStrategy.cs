namespace Structurizr
{
    
    /// <summary>
    /// A pluggable strategy that can be used to copy layout information from one version of a view to another.
    /// </summary>
    public interface LayoutMergeStrategy
    {
    
        /// <summary>
        /// Attempts to copy the visual layout information (e.g. x,y coordinates) of elements and relationships
        /// from the specified source view into the specified destination view. 
        /// </summary>
        /// <param name="viewWithLayoutInformation">the source view (e.g. the version stored by the Structurizr service)</param>
        /// <param name="viewWithoutLayoutInformation">the destination View (e.g. the new version, created locally with code)</param>
        void CopyLayoutInformation(View viewWithLayoutInformation, View viewWithoutLayoutInformation);
        
    }
    
}