using System.IO;
using Structurizr.Api;
using Structurizr.Documentation;

namespace Structurizr.Examples
{
    
    /// <summary>
    /// An empty software architecture document using the arc42 template.
    /// 
    /// See https://structurizr.com/share/27791/documentation for the live version.
    /// </summary>
    public class Arc42DocumentationExample
    {
        
        private const long WorkspaceId = 27791;
        private const string ApiKey = "key";
        private const string ApiSecret = "secret";

        static void Main()
        {
            Workspace workspace = new Workspace("Documentation - arc42", "An empty software architecture document using the arc42 template.");
            Model model = workspace.Model;
            ViewSet views = workspace.Views;

            Person user = model.AddPerson("User", "A user of my software system.");
            SoftwareSystem softwareSystem = model.AddSoftwareSystem("Software System", "My software system.");
            user.Uses(softwareSystem, "Uses");

            SystemContextView contextView = views.CreateSystemContextView(softwareSystem, "SystemContext", "An example of a System Context diagram.");
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            Styles styles = views.Configuration.Styles;
            styles.Add(new ElementStyle(Tags.Person) { Shape = Shape.Person });

            Arc42DocumentationTemplate template = new Arc42DocumentationTemplate(workspace);

            // this is the Markdown version
            DirectoryInfo documentationRoot = new DirectoryInfo("Documentation" + Path.DirectorySeparatorChar + "arc42" + Path.DirectorySeparatorChar + "markdown");
            template.AddIntroductionAndGoalsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction-and-goals.md")));
            template.AddConstraintsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "02-architecture-constraints.md")));
            template.AddContextAndScopeSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "03-system-scope-and-context.md")));
            template.AddSolutionStrategySection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "04-solution-strategy.md")));
            template.AddBuildingBlockViewSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "05-building-block-view.md")));
            template.AddRuntimeViewSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "06-runtime-view.md")));
            template.AddDeploymentViewSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "07-deployment-view.md")));
            template.AddCrosscuttingConceptsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "08-crosscutting-concepts.md")));
            template.AddArchitecturalDecisionsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "09-architecture-decisions.md")));
            template.AddQualityRequirementsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "10-quality-requirements.md")));
            template.AddRisksAndTechnicalDebtSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "11-risks-and-technical-debt.md")));
            template.AddGlossarySection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "12-glossary.md")));

            // this is the AsciiDoc version
//            DirectoryInfo documentationRoot = new DirectoryInfo("Documentation" + Path.DirectorySeparatorChar + "arc42" + Path.DirectorySeparatorChar + "asciidoc");
//            template.AddIntroductionAndGoalsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "01-introduction-and-goals.adoc")));
//            template.AddConstraintsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "02-architecture-constraints.adoc")));
//            template.AddContextAndScopeSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "03-system-scope-and-context.adoc")));
//            template.AddSolutionStrategySection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "04-solution-strategy.adoc")));
//            template.AddBuildingBlockViewSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "05-building-block-view.adoc")));
//            template.AddRuntimeViewSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "06-runtime-view.adoc")));
//            template.AddDeploymentViewSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "07-deployment-view.adoc")));
//            template.AddCrosscuttingConceptsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "08-crosscutting-concepts.adoc")));
//            template.AddArchitecturalDecisionsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "09-architecture-decisions.adoc")));
//            template.AddQualityRequirementsSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "10-quality-requirements.adoc")));
//            template.AddRisksAndTechnicalDebtSection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "11-risks-and-technical-debt.adoc")));
//            template.AddGlossarySection(softwareSystem, new FileInfo(Path.Combine(documentationRoot.FullName, "12-glossary.adoc")));

            StructurizrClient structurizrClient = new StructurizrClient(ApiKey, ApiSecret);
            structurizrClient.PutWorkspace(WorkspaceId, workspace);
        }
        
    }
    
}