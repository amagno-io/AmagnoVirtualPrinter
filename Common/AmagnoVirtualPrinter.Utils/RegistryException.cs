using System;

namespace AmagnoVirtualPrinter.Utils
{
    public class RegistryException : Exception
    {
        public RegistryException()
        {
        }

        public RegistryException(string message) : base(message)
        {
        }

        public RegistryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}