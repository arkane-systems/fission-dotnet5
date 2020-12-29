using System;
using Fission.Functions;

public class TestFunc : IFissionFunction
{
    public object Execute(FissionContext context)
    {
        // var x = Convert.ToInt32(context.Arguments["x"]);
        // var y = Convert.ToInt32(context.Arguments["y"]);
        // return (x + y).ToString();
        return "Fluffy!";
    }
}
