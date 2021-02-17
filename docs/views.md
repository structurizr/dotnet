# Views

Once you've [added elements to a model](model.md), you can create one or more views to visualise parts of the model, which can subsequently be rendered as diagrams by a number of different tools.

Structurizr for .NET supports all of the view types described in the [C4 model](https://c4model.com), and the .NET classes implementing these views can be found in the [Structurizr](https://github.com/structurizr/dotnet/tree/master/Structurizr.Core/View) namespace as follows:

* [SystemContextView](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/SystemContextView.cs)
* [ContainerView](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/ContainerView.cs)
* [ComponentView](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/ComponentView.cs)
* [SystemLandscapeView](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/SystemLandscapeView.cs)
* [DynamicView](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/DynamicView.cs)
* [DeploymentView](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/DeploymentView.cs)

## Creating views

All views are associated with a [ViewSet](https://github.com/structurizr/dotnet/blob/master/Structurizr.Core/View/ViewSet.cs), which is created for you when you create a workspace.

```java
Workspace workspace = new Workspace("Getting Started", "This is a model of my software system.");
ViewSet views = workspace.Views;
```

Use the various ```Create*View``` methods on the ```ViewSet``` class to create views.

