# Use Automapper for object to object mapping

Date: 12-05-2025

## Status

Accepted

## Context and Problem Statement
The movie logger application requires frequent mapping between domain models and DTOs for controller responses and requests. Manually writing the code necessary to map across would increase development time and risk of errors.

## Decision Drivers

- To remove the need for repetitive mapping code
- To help keep controllers and services clean and focused

## Considered Options

- Use Automapper
- Write manual mapping code

## Decision Outcome

We will use AutoMapping for mapping between domain entities and DTOs.

### Positive Consequences
- Reduces boilerplate and greatly improves readability
- Automapper is mature, well maintained, and widely used in the .NET ecosystem

### Negative Consequences
- If not correctly configured it may obscure mappings when debugging
- small learning curve for configuration and profiles

## Links
- [AutoMapper GitHub](https://github.com/AutoMapper/AutoMapper)
- [AutoMapper documentation](https://docs.automapper.org/)