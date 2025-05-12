# Use PostgreSQL as the database

Date: 12-05-2025

## Status

Accepted

## Context and Problem Statement

The application needs a relational database to persist data such as users, movies, and reviews. We need  a database technology that is robust, open source, and well-supported across development and deployment environments.

## Decision Drivers

- Compatibility with .NET and Entity Framework Core
- Mature community support and documentation
- Easily containerized for development with Docker

## Considered Options

- PosgreSQL
- Microsoft SQL Server

## Decision Outcome

The decision was made to use PostgreSQL as the primary database.

### Positive Consequences

- Cross platform and open source
- Easy local development with Docker
- Excellent performance and feature set for modern web applications

### Negative Consequences

- Small learning curve for developers unfamiliar with PostgreSQL specific syntax

## Links
- [PostgreSQL official site](https://www.postgresql.org/)