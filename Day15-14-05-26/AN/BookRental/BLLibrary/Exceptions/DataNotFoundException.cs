using System;

namespace BLLibrary.Exceptions
{
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException(string dataName, object identifier)
            : base($"{dataName} with '{identifier}' not found.")
        {
        }
    }
}
