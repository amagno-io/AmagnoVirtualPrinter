using System;

namespace VirtualPrinter.Agent.Core
{
    public class PostScriptConversionException : Exception
    {
        public PostScriptConversionException()
        {
        }

        public PostScriptConversionException(string message) : base(message)
        {
        }

        public PostScriptConversionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}