# 1. Use N-Tier Architecture Pattern in a Monolith

Date: 2025-05-05

Deciders: Campbell Gilpin

## Status

2025-05-05 proposed

## Context and Problem Statement

The application being built is called Movie Logger, and it is a web application for logging, reviewing and maintaining movies and movie collections.

The application will be built and maintained by one person. 

The priority is to move quickly toward a working MVP, while still building a codebase that is clean, testable, and maintainable. 

## Decision Drivers 
* Speed of development (Necessary to build and demonstrate at a fast pace)

* Simplicity to avoid overengineering a small application

* Maintainability over time

## Considered Options

- **Microservices:** Decided against for being unnecessarily complicated for the scope of the application. There doesn't seem to be any advantage to breaking out the domains into separate services and databases.
- **Event Driven:** Decided against as the positives provided by this approach would be redundant in the context of what the application is set to achieve, and there isn't much need for firing and forgetting for heavy processing. 



## Decision Outcome

The application will be structured using a **Layered N-Tier architecture** inside a **Monolithic deployment**.

The application will be structured following a clear 3 layer structure.

- **UI Layer**
- **Business Logic Layer**
- **Data Access Layer**

### Positive Consequences 

* Easy to write unit tests for the logic layer without touching the DB or UI  

### Negative Consequences

* Could become harder to manage as the codebase grows in size
* Scaling up individual parts of the app in the future would require refactoring 
