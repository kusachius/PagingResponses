# Kusachius.PagingResponses

`Kusachius.PagingResponses` provides paging, sorting, and filtering helpers for
ASP.NET Core and Entity Framework Core APIs, plus a paged JSON response
envelope.

## Installation

Version 1.10.0 targets .NET 10 and Entity Framework Core 10. Building and testing
requires the .NET 10 SDK. Consumers require the ASP.NET Core 10 shared framework.

This release uses AutoMapper 15.1.1 to address
[CVE-2026-32933](https://github.com/advisories/GHSA-rvv3-g6hj-g44x).
Consumers upgrading from AutoMapper 10 must review the
[AutoMapper 15 upgrade guide](https://docs.automapper.io/en/stable/15.0-Upgrade-Guide.html)
and its licensing requirements. Configure the license in the consuming application;
manual `MapperConfiguration` construction now also requires an `ILoggerFactory`.

```shell
dotnet add package Kusachius.PagingResponses
```

## Usage

Implement `ICriteriaMapperFactory` to translate request column names to
database columns, then create criteria from the request query string:

```csharp
using Kusachius.PagingResponses;

var factory = new CriteriaFactory(mapperFactory, new LtStringCleaner());
Criteria<Client> criteria = factory.CreateFromRequest<Client>(Request.Query);

KEnumerable<Client> page = await KEnumerable<Client>.CreateResultAsync(
    dbContext.Clients, criteria);

// Returns { data: [...], meta: { pagination: { ... } } } as a 200 response.
return new PagedActionResult<ClientDto, Client>(page, mapper);
```

- `Criteria<T>` applies conditions, sorting, and paging to an `IQueryable<T>`.
- `KEnumerable<T>` holds the page data with the current page, page size, total
  items, total pages, and optional column totals.
- `PagedActionResult<T, K>` maps the page with AutoMapper and writes it as the
  response envelope.
- `LtStringCleaner` normalizes Lithuanian characters for search conditions.

## Publishing

The GitHub Actions workflow in [`.github/workflows/publish.yml`](.github/workflows/publish.yml)
builds, tests, packs, and publishes the package to NuGet.org when a `v*` tag is
pushed (for example `v1.10.0`), or when it is run manually with a package
version.

Before publishing, configure NuGet.org trusted publishing:

1. On NuGet.org, open **Trusted Publishing** and add a trusted publisher for
   the `kusachius/PagingResponses` GitHub repository and the `publish.yml`
   workflow file.
2. Add a GitHub Actions secret named `NUGET_USER` containing the NuGet.org user
   name (not an email address).
