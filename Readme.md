# Test

SearchDatastore_TaskAsync against VSAN in sddc returns a Task that produces a stack overflow in this code.

The dependency NetVimClient is a net wrapper of the vsphere web service api wsdl.

Run with .Net Core 3.1 SDK.

Lots of ways to do this.  Here is one:

```bash
$ git clone https://github.com/sei-jmattson/sddc-hostbrowser.git
$ cd sddc-hostbrowser

## edit connection properties in Program.cs

$ docker run --rm -it -v `pwd`:/app mcr.microsoft.com/dotnet/core/sdk:3.1
/# cd /app
/# dotnet run
```

To debug, load folder in VSCode.

Edit connection properties in Program.cs
