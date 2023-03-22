# Structurizr for .NET

This GitHub repository is a client library for the [Structurizr cloud service and on-premises installation](https://structurizr.com) . This .NET library isn't as well supported as the original Java version, and some newer features are missing.

Unless you are planning to use .NET code to generate parts of your software architecture model, the [Structurizr DSL](https://github.com/structurizr/dsl) is the recommended tooling for authoring Structurizr workspaces, while the [Structurizr CLI](https://github.com/structurizr/cli) can be used to push/pull workspaces and export views to PlantUML, Mermaid, etc.

## A quick example

As an example, the following C# code can be used to create a software architecture model that describes a user using a software system.

```c#
Workspace workspace = new Workspace("Getting Started", "This is a model of my software system.");
Model model = workspace.Model;

Person user = model.AddPerson("User", "A user of my software system.");
SoftwareSystem softwareSystem = model.AddSoftwareSystem("Software System", "My software system.");
user.Uses(softwareSystem, "Uses");

ViewSet viewSet = workspace.Views;
SystemContextView contextView = viewSet.CreateSystemContextView(softwareSystem, "SystemContext", "An example of a System Context diagram.");
contextView.AddAllSoftwareSystems();
contextView.AddAllPeople();

Styles styles = viewSet.Configuration.Styles;
styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
```

The view can then be exported to be visualised using the Structurizr cloud service or an on-premises installation.

## Table of contents

* Introduction
    * [Getting started](docs/getting-started.md)
    * [Binaries](docs/binaries.md)
    * [API Client](docs/api-client.md)
    * [FAQ](docs/faq.md)
 * Model
	* [Implied relationships](docs/implied-relationships.md)
* Views
    * [System Context diagram](docs/system-context-diagram.md)
    * [Container diagram](docs/container-diagram.md)
    * [Component diagram](docs/component-diagram.md)
    * [Dynamic diagram](docs/dynamic-diagram.md)
    * [Deployment diagram](docs/deployment-diagram.md)
    * [System Landscape diagram](docs/system-landscape-diagram.md)
    * [Styling elements](docs/styling-elements.md)
    * [Styling relationships](docs/styling-relationships.md)
    * [Filtered views](docs/filtered-views.md)
* Other
    * [Client-side encryption](docs/client-side-encryption.md)
* [changelog](docs/changelog.md)
