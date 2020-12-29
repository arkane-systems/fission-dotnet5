namespace Fission.DotNet
{
    public interface IFunctionStore
    {
        public FunctionRef? Func { get; }

        public void SetFunctionRef (FunctionRef func);
    }
}
