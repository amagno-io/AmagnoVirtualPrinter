using System;

namespace AmagnoVirtualPrinter.Agent.Core.Model
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