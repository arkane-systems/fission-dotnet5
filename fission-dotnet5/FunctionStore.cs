#region using

using System;

#endregion

namespace Fission.DotNet
{
    internal class FunctionStore : IFunctionStore
    {
        private FunctionRef? func;

        /// <inheritdoc />
        FunctionRef? IFunctionStore.Func => this.func;

        /// <inheritdoc />
        void IFunctionStore.SetFunctionRef (FunctionRef func)
        {
            if (this.func != null)
                throw new InvalidOperationException (message: "Cannot overwrite an existing function.");

            this.func = func;
        }
    }
}
