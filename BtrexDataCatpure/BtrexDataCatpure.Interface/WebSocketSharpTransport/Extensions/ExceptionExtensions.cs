﻿using System;

namespace BtrexDataCapture.Interface.WebSocketSharpTransport.Extensions
{
    internal static class ExceptionsExtensions
    {
        internal static Exception Unwrap(this Exception ex)
        {
            if (ex == null)
            {
                return null;
            }

            var next = ex.GetBaseException();

            while (next.InnerException != null)
            {
                // On mono GetBaseException() doesn't seem to do anything
                // so just walk the inner exception chain.
                next = next.InnerException;
            }

            return next;
        }
    }
}
