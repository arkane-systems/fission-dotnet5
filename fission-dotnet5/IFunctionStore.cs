#region header

// /*
//  * fission-dotnet5 - IFunctionStore.cs
//  *
//  * Created by: avatar at 2020/12/29 10:39 AM.
//  *
//  */

#endregion

namespace Fission.DotNet
{
    public interface IFunctionStore
    {
        public FunctionRef? Func { get; }

        public void SetFunctionRef (FunctionRef func);
    }
}
