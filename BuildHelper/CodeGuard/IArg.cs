using System;
using System.Collections.Generic;
using BuildHelper.CodeGuard.Internals;

namespace BuildHelper.CodeGuard
{
    public interface IArg<T>
    {
        T Value { get; }
        ArgName Name { get; }
        IMessageHandler<T> Message { get; }
        IEnumerable<ErrorInfo> Errors { get; }
    }
}
