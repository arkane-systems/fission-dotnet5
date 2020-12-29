using JetBrains.Annotations;

namespace Fission.Functions
{
    [PublicAPI]
    public interface IFissionFunction
    {
        public object Execute (FissionContext context);
    }
}
