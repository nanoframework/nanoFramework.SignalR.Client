// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Json;
using System;
using System.Threading;

namespace nanoFramework.SignalR.Client
{
    /// <summary>
    /// An asynchronous object that represents the invocation of <see cref="HubConnection.InvokeCoreAsync"/> on the server.
    /// </summary>
    /// <remarks>
    /// Just like an Async <see cref="Completed"/> indicates if the response from server is received.
    /// Calling <see cref="Value"/> before the server response will await the response from the server synchronously. 
    /// </remarks>
    public class AsyncResult
    {
        /// <summary>
        /// The result from the asynchronous invocation of <see cref="HubConnection.InvokeCoreAsync"/> on the server.
        /// </summary>
        /// <remarks>
        /// Calling Value before the server call is <see cref="Completed"/> will await the response of the server synchronously. 
        /// </remarks>
        public object Value => GetValue();

        /// <summary>
        /// Indicates if the invocation of <see cref="HubConnection.InvokeCoreAsync"/> on the server us completed.
        /// </summary>
        public bool Completed { get; private set; } = false;

        /// <summary>
        /// Indicates if the invocation of <see cref="HubConnection.InvokeCoreAsync"/> returned any errors.
        /// </summary>
        /// <remarks>
        /// <see cref="ErrorMessage"/> contains the error details
        /// </remarks>
        public bool Error => ErrorMessage != null;

        /// <summary>
        /// Contains the error message returned by the invocation of <see cref="HubConnection.InvokeCoreAsync"/>.
        /// </summary>
        /// <remarks>
        /// This is null if no errors occurred.
        /// </remarks>
        public string ErrorMessage { get; internal set; } = null;

        /// <summary>
        /// The return Type of the invocation of <see cref="HubConnection.InvokeCoreAsync"/>.
        /// </summary>
        public Type ReturnType { get; private set; }

        internal string InvocationId { get; private set; }

        private object _value;
        private readonly Timer _completionTimeoutTimer;
        internal AutoResetEvent _asyncResult = new AutoResetEvent(false);
        private readonly AsyncLogic _asyncLogic;

        internal AsyncResult(string id, Type returnType, TimeSpan timeout, AsyncLogic asyncLogic)
        {
            ReturnType = returnType;
            InvocationId = id;
            _completionTimeoutTimer = new Timer(completionTimeout, null, (int)timeout.TotalMilliseconds, Timeout.Infinite);
            _asyncLogic = asyncLogic;
        }

        private void completionTimeout(object state)
        {
            SetError("awaiting result timed out");
            _asyncResult.Set();
            _asyncLogic.RemoveAsyncResult(InvocationId);
        }

        private object GetValue()
        {

            if (Completed || Error)
            {
                return _value;
            }

            _asyncResult.WaitOne();
            return _value;
        }

        internal void SetError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Completed = true;
            _asyncResult.Set();
            _asyncLogic.RemoveAsyncResult(InvocationId);
        }

        internal void Setvalue(object contentValue)
        {
            if (contentValue != null)
            {
                _value = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(contentValue), ReturnType);
            }
            else
            {
                _value = null;
            }
            _completionTimeoutTimer.Dispose();
            _asyncResult.Set();
            Completed = true;
            _asyncLogic.RemoveAsyncResult(InvocationId);
        }
    }
}