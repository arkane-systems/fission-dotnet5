#region header

// fission-dotnet5 - IFunctionStore.cs
// 
// Created by: Alistair J R Young (avatar) at 2020/12/29 10:39 AM.

#endregion

namespace Fission.DotNet
{
    /// <summary>
    ///     Interface for the function store service, which caches the post-specialization function for repeated use by the
    ///     <see cref="Controllers.FunctionController" />.
    /// </summary>
    public interface IFunctionStore
    {
        /// <summary>
        ///     Returns a reference to the post-specialization function, or null if container has not yet been specialized.
        /// </summary>
        public FunctionRef? Func { get; }

        /// <summary>
        ///     Stores a reference to the post-specialization function for later use.
        /// </summary>
        /// <param name="func"></param>
        public void SetFunctionRef (FunctionRef func);
    }
}
