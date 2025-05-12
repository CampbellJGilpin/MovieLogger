# Use FluentValidation for input validation

Date: 12-05-2025

## Status

Accepted

## Context and Problem Statement

The MovieLogger apploications requires consistent and reusable validation logic for incoming models.

## Decision Drivers

- Keep validation logic clean
- Maintain testability and seperation of concerns

## Considered Options

- Use FluentValidation
- Manually validate in controller or service methods

## Decision Outcome

We will use FluentValidation for validating input models across the application.

### Positive Consequences

- Easy to unit test
- Validation logic remains cleanly separated from models
- Fluent and strongly typed rule definitions

### Negative Consequences

- Additional dependency in the project

## Links
- [FluentValidation GitHub](https://github.com/FluentValidation/FluentValidation)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)