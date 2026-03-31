# Contributing to CommunityToolkit.Aspire.Quartz

Thank you for your interest in contributing! We welcome contributions from the community.

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Git
- Docker Desktop (for integration tests)
- Code editor (Visual Studio 2022, VS Code, or Rider)

### Setup

1. **Fork and clone:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/aspire-hosting-quartz.git
   cd aspire-hosting-quartz
   ```

2. **Build:**
   ```bash
   dotnet build
   ```

3. **Run tests:**
   ```bash
   dotnet test
   ```

## 📝 Development Workflow

### Before You Start

1. Check existing [issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues) and [pull requests](https://github.com/alnuaimicoder/aspire-hosting-quartz/pulls)
2. Open an issue to discuss major changes
3. Fork the repository

### Making Changes

1. **Create a branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes:**
   - Write code following our [coding standards](#coding-standards)
   - Add tests for new features
   - Update documentation

3. **Test locally:**
   ```bash
   # Quick check
   .\scripts\quick-check.ps1

   # Full CI
   .\scripts\local-ci.ps1
   ```

4. **Commit:**
   ```bash
   git commit -m "feat: add your feature"
   ```
   Follow [Conventional Commits](https://www.conventionalcommits.org/)

5. **Push and create PR:**
   ```bash
   git push origin feature/your-feature-name
   ```

## 🧪 Testing

### Running Tests

```bash
# All tests
dotnet test

# Specific project
dotnet test tests/CommunityToolkit.Aspire.Quartz.Tests

# With coverage
dotnet test /p:CollectCoverage=true
```

### Test Categories

- **Unit Tests**: Fast, no external dependencies
- **Integration Tests**: Require Docker (SQL Server/PostgreSQL)
- **Sample Tests**: End-to-end scenarios

### Writing Tests

- Use xUnit
- Follow AAA pattern (Arrange, Act, Assert)
- Name tests clearly: `MethodName_Scenario_ExpectedResult`
- Aim for 80%+ code coverage

## 📋 Coding Standards

### C# Guidelines

- Follow [.NET coding conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use `.editorconfig` settings (enforced automatically)
- Enable nullable reference types
- Write XML documentation for public APIs

### Code Style

```csharp
// ✅ Good
public async Task<string> EnqueueJobAsync(JobOptions options)
{
    ArgumentNullException.ThrowIfNull(options);

    // Implementation
}

// ❌ Bad
public async Task<string> EnqueueJob(JobOptions opts)
{
    // No validation
}
```

### Documentation

- XML comments for all public APIs
- Include `<summary>`, `<param>`, `<returns>`, `<exception>`
- Add code examples for complex features

## 📦 Project Structure

```
aspire-hosting-quartz/
├── src/                                    # Source code
│   ├── CommunityToolkit.Aspire.Quartz.Abstractions/
│   ├── CommunityToolkit.Aspire.Quartz/
│   └── CommunityToolkit.Aspire.Hosting.Quartz/
├── tests/                                  # Tests
├── samples/                                # Sample applications
├── docs/                                   # Documentation
└── scripts/                                # Build scripts
```

## 🔄 Commit Message Format

We follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `test`: Adding tests
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `chore`: Maintenance tasks
- `ci`: CI/CD changes

### Examples

```
feat(client): add retry policy builder
fix(hosting): resolve connection string issue
docs: update getting started guide
test(abstractions): add retry policy tests
```

## 🔍 Pull Request Guidelines

### Before Submitting

- [ ] All tests pass locally
- [ ] Code follows style guidelines
- [ ] Documentation updated
- [ ] Commit messages follow conventions
- [ ] No merge conflicts

### PR Template

Your PR should include:

1. **Description**: What and why
2. **Changes**: List of changes
3. **Testing**: How you tested
4. **Screenshots**: If UI changes
5. **Related Issues**: Link issues

### Review Process

1. Automated checks run (CI)
2. Maintainer reviews code
3. Address feedback
4. Approval and merge

## 🐛 Reporting Issues

### Bug Reports

Include:
- Clear description
- Steps to reproduce
- Expected vs actual behavior
- Environment (.NET version, OS)
- Code samples or error messages

### Feature Requests

Include:
- Use case description
- Proposed solution
- Alternatives considered
- Additional context

## 📖 Documentation

### What to Document

- New features
- API changes
- Breaking changes
- Migration guides

### Where to Document

- XML comments (code)
- README.md (overview)
- GETTING_STARTED.md (tutorials)
- CHANGELOG.md (changes)

## 🎯 Areas to Contribute

### Good First Issues

Look for issues labeled `good first issue`:
- Documentation improvements
- Test coverage
- Code cleanup
- Sample applications

### Help Wanted

Issues labeled `help wanted`:
- New features
- Performance improvements
- Bug fixes

## 🤝 Code of Conduct

Please read and follow our [Code of Conduct](CODE_OF_CONDUCT.md).

## 💬 Getting Help

- **Questions**: [GitHub Discussions](https://github.com/alnuaimicoder/aspire-hosting-quartz/discussions)
- **Bugs**: [GitHub Issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
- **Chat**: [Discord/Slack] (if available)

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing! 🎉

