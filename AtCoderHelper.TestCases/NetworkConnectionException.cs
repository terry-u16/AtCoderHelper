using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TerryU16.AtCoderHelper.TestCases;

internal class NetworkConnectionException : Exception
{
    public NetworkConnectionException()
    {
    }

    public NetworkConnectionException(string? message) : base(message)
    {
    }

    public NetworkConnectionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NetworkConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
