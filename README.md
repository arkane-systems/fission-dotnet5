# fission-dotnet5
A .NET 5 function environment for [Fission](https://fission.io/).

The environment Docker image (_fission-dotnet5_) contains the .NET 5.0 runtime and uses an ASP.NET Core web api application to make the relevant endpoints available to Fission. At present, this image supports compiling and running single-file functions (although this file may contain an arbitrary number of classes, etc.) using the types available in the core .NET 5 assemblies.

(Support for packaged types is expected to arrive with an upcoming release including multi-file function and builder functionality.)

The environment works via the IFissionFunction interface (provided by the _Fission.Functions_ assembly.) A function for _fission-dotnet5_ is presented as a class which implements the _IFissionFunction_ interface, thus:

```
using System;
using Fission.Functions;

public class HelloWorld : IFissionFunction
{
    public object Execute(FissionContext context)
    {
        return "hello, world!";
    }
}

```

Logging and access to function call parameters are accessible through the _context_ parameter. Please see the inline documentation for _FissionContext.cs_ and the examples for further details.

## Rebuilding the image

The Dockerfile requires Docker 17.05+ to build, for the multi-stage builds feature, and to rebuild the images as they are provided also requires `docker buildx` to build the multi-arch manifests.

To rebuild the containers, in the top-level directory, execute the following:

```
docker build . --platform=linux/amd64,linux/arm64,linux/arm -t repository/fission-dotnet5:dev --push
```

## Setting up the Fission environment

To set up the .NET 5 Fission environment, execute the command:

```
fission env create --name dotnet5 --image repository/fission-dotnet5:dev --version 1
```

## Configuring and testing a function

If the above function is contained within the file `HelloWorld.cs` (see the examples directory), and the environment has been set up as above, then it can be installed as follows:

```
fission fn create --name hello-dotnet --env dotnet5 --code HelloWorld.cs
```

And tested thus:

```
fission fn test --name hello-dotnet
```

