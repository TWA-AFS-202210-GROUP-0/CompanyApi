using System;

namespace CompanyApi
{
    public class CompanyException : Exception
    {
        public CompanyException(string message) : base(message)
        {
        }
    }
}

