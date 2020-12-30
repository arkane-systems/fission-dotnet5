#region header

// fission-dotnet5 - FunctionStore.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/29 10:41 AM.

#endregion

#region using

using System;

using Fission.DotNet.Properties;

#endregion

namespace Fission.DotNet
{
    /// <summary>
    ///     Implementation of the function store service, as defined by <see cref="IFunctionStore" />.
    /// </summary>
    internal class FunctionStore : IFunctionStore
    {
        private FunctionRef? func;

        /// <inheritdoc />
        FunctionRef? IFunctionStore.Func => this.func;

        /// <inheritdoc />
        void IFunctionStore.SetFunctionRef (FunctionRef func)
        {
            if (this.func != null)
                throw new InvalidOperationException (message: Resources.FunctionStore_SetFunctionRef_CannotOverwrite);

            this.func = func;
        }
    }
}
