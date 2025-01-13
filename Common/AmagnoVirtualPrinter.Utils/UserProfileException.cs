using System;

namespace AmagnoVirtualPrinter.Utils
{
    public class UserProfileException : Exception
    {
        public UserProfileException()
        {
        }

        public UserProfileException(string message) : base(message)
        {
        }

        public UserProfileException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}