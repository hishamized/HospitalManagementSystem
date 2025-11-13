using System;

namespace HMS.Domain.Exceptions
{
    public class TcpPortNumberException : Exception
    {
        public TcpPortNumberException(short tcpPortNumber) : base($"TCPPortNumber cannot be less than 1. Current Value : {tcpPortNumber}")
        {
            
        }
    }
}
