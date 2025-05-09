# MovieLogger — Server

This is the backend server for the MovieLogger application.

- **Framework**: ASP.NET
- **API Spec**: OpenAPI
- **Language**: C#
- **Architecture**: N-Tier Monolith
- **Project Structure**: 
  - `movielogger.api` – API layer
  - `movielogger.services` – Business logic layer
  - `movielogger.dal` – Data access layer
  - `movielogger.tests` - Unit Tests
  - **Database**:

## Testing

- **Framework**: xUnit
- **Mocking**: NSubstitute

## OpenAPI

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/campbelljgilpin/movielogger.git
cd movielogger/server
```

## Dependencies

- **Automapper** - For mapping objects
- **Entity Framework** - For Database Access
- **FluentValidation** — For request validation
- **xUnit & NSubstitute** — Unit testing