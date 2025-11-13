using System;

namespace HMS.Domain.Exceptions
{
    public class StoredProcedureExecutionException : Exception
    {
        public StoredProcedureExecutionException(string errorMessage) : base($"Error Executing Stored Procedure. Error : {errorMessage}")
        {
            
        }
    }
}
