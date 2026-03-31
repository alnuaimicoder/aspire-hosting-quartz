# Contributing to Aspire.Hosting.Quartz

Thank you for your interest in contributing to Aspire.Hosting.Quartz! We welcome contributions from the community.

## Development Setup

### Prerequisites

- .NET 8 SDK or later
- Git
- A code editor (Visual Studio, VS Code, or Rider)

### Getting Started

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/aspire-hosting-quartz.git
   cd aspire-hosting-quartz
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the solution:
   ```bash
   dotnet build
   ```

## Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests only
dotnet test --filter "Category=Unit"

# Run integration tests (requires Docker)
dotnet test --filter "Category=Integration"

# Run property-based tests
dotnet test --filter "Category=Property"
```

## Coding Standards

- Follow C# coding conventions
- Use meaningful variable and method names
- Write XML documentation for all public APIs
- Maintain test coverage above 80%
- Use `.editorconfig` settings (enforced automatically)

## Commit Message Guidelines

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: add new feature
fix: fix bug
docs: update documentation
test: add tests
refactor: refactor code
chore: update dependencies
```

## Pull Request Process

1. Create a feature branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```
2. Make your changes with tests
3. Ensure all tests pass:
   ```bash
   dotnet test
   ```
4. Commit your changes:
   ```bash
   git commit -m "feat: add your feature"
   ```
5. Push to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```
6. Open a Pull Request against the `main` branch

### PR Requirements

- All tests must pass
- Code coverage must not decrease
- Follow coding standards
- Include relevant documentation updates
- Reference any related issues

## Reporting Issues

When reporting issues, please include:

- A clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Environment details (.NET version, OS, etc.)
- Relevant code samples or error messages

## Code of Conduct

Please read and follow our [Code of Conduct](CODE_OF_CONDUCT.md).

## Questions?

Feel free to open an issue for questions or discussions.

Thank you for contributing! 🎉