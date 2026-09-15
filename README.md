# NexStructure

A .NET 10 clean-architecture API template: API, Application, Domain, and Infrastructure.

## Use this GitHub template

1. Open [github.com/Studionex/NexStructure](https://github.com/Studionex/NexStructure)
2. Click **Use this template** → **Create a new repository**
3. Clone your new repo and run it:

```bash
dotnet restore
dotnet run --project NexStructure.Api
```

Rename the solution after copying by replacing `NexStructure` in project names and namespaces, or create from the `dotnet new` template below so that is done for you.

## Install as a `dotnet new` template

From nuget.org (public):

```bash
dotnet new install NexStructure.Template
dotnet new nexstructure -n MyApi
```

From GitHub Packages:

```bash
dotnet nuget add source "https://nuget.pkg.github.com/Studionex/index.json" \
  --name studionex \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text

dotnet new install NexStructure.Template
dotnet new nexstructure -n MyApi
```

## License

MIT
