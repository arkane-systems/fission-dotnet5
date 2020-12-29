#region using

using System;
using System.Reflection;

using Fission.Functions;

#endregion

namespace Fission.DotNet
{
    public class FunctionRef
    {
        private readonly Assembly assembly;
        private readonly Type     type;

        public FunctionRef (Assembly assembly, Type type)
        {
            this.assembly = assembly;
            this.type     = type;
        }

        public object Invoke (FissionContext context)
            => ((IFissionFunction) this.assembly.CreateInstance (typeName: this.type.FullName!)!)!.Execute (context: context);
    }
}
