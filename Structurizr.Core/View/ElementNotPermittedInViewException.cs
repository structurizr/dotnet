using System;

namespace Structurizr
{
    
    public sealed class ElementNotPermittedInViewException : Exception
    {

        internal ElementNotPermittedInViewException(string message) : base(message)
        {
        }
        
    }
    
}