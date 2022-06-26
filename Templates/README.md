# Templates projects

## Install templates
`$> dotnet new install ./`

## Uninstall templates
`$> dotnet new uninstall ./`

## Existed templates list
`$> dotnet new --list`

Existed template names:
* grpc-service;
* grpc-shared;
* grpc-tests.

## Create new project by template
`$> dotnet new <template-name> --name <project-name> --output <project-folder>`

For example:

`$> dotnet new grpc-service --name Itadakimasu.API.Products --output Itadakimasu.API.Products`

## Notes
For gRPC templates - gRPC service and gRPC tests projects need to fix references between those projects and delete wrong references.
Also they need to fix references in Dockerfile.