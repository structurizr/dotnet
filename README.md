# Structurizr for .NET

This repo is a port of the [Structurizr for Java library](https://github.com/structurizr/java) but has drifted out of sync, and some newer features are missing. I will continue to support the Java-based Structurizr tooling but, due to time constraints, I will no longer be making updates to this codebase or releases via NuGet. The code remains open source, so you are welcome to fork the repo and make your own releases.

Unless you are planning to use .NET code to generate parts of your software architecture model, the [Structurizr DSL](https://docs.structurizr.com/dsl) is the recommended tooling for authoring Structurizr workspaces.

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
