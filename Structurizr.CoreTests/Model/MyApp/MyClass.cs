namespace Structurizr.CoreTests.MyApp
{
    public class MyClass : CodeElement
    {
        public MyClass() : base(typeof(MyClass).AssemblyQualifiedName)

        {
            Name = "My class";
            Description =
                "For classes, custom CodeElement subclasses will probably not make much sense. Nevertheless, for consistency overriding should be possible here as well.";
        }
    }
}