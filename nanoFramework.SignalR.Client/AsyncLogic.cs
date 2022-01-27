// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;

namespace nanoFramework.SignalR.Client
{
    internal class AsyncLogic
    {
        private ushort _asyncId = 0;
        private object _asyncIdLock = new object();
        private Hashtable _asyncBacklog = new Hashtable();
        private object _asyncBacklogLock = new object();

        // call when HubConnection is closed
        internal void CloseAllAsyncResults()
        {
            var keys = _asyncBacklog.Keys;
            foreach (var key in keys)
            {
                SetAsyncResultError("HubConnection was closed", (string)key);
            }
        }

        internal AsyncResult BeginAsyncResult(Type returnType, TimeSpan timeout)
        {
            string invocationId;
            lock (_asyncIdLock)
            {
                invocationId = (_asyncId++).ToString();
            }

            AsyncResult asyncResult = new AsyncResult(invocationId, returnType, timeout, this);
            lock (_asyncBacklogLock)
            {
                _asyncBacklog.Add(asyncResult.InvocationId, asyncResult);
            }

            return asyncResult;
        }

        internal void SetAsyncResultValue(object value, string invocationId)
        {
            var asyncResult = _asyncBacklog[invocationId] as AsyncResult;

            if (asyncResult != null)
            {
                asyncResult.Setvalue(value);
            }

            RemoveAsyncResult(invocationId);
        }

        internal void SetAsyncResultError(string errorMessage, string invocationId)
        {
            var asyncResult = _asyncBacklog[invocationId] as AsyncResult;

            if (asyncResult != null)
            {
                asyncResult.SetError(errorMessage);
            }

            RemoveAsyncResult(invocationId);
        }

        internal void RemoveAsyncResult(string invcationId)
        {
            lock (_asyncBacklogLock)
            {
                _asyncBacklog.Remove(invcationId);
            }
        }
    }
}
