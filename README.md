# Net.Utils.GraphQL

Utilities for working with GraphQL responses.

## Usage

Install via NuGet:

```shell
dotnet add package Net.Utils.GraphQL
```

```csharp
using GraphQL.Client.Abstractions;
using Net.Utils.GraphQL;

GraphQLResponse<MyData> response = await client.SendQueryAsync<MyData>(request);

// Filters out 'indexing_error' messages by default
var data = response.EnsureNoErrors();

// Disable filtering:
var dataWithoutFilter = response.EnsureNoErrors(filterIndexingErrors: false);
```