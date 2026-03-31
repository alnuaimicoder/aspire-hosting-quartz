# Contributing to CommunityToolkit.Aspire.Quartz

Thank you for your interest in contributing! We welcome contributions from the community.

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Git
- Docker Desktop (for integration tests)
- Code editor (Visual Studio 2022, VS Code, or Rider)

### Setup

#### Step 1: Fork the Repository

1. **Go to the repository:**
   ```
   https://github.com/alnuaimicoder/aspire-hosting-quartz
   ```

2. **Click the "Fork" button** in the top-right corner

3. **Wait for GitHub to create your fork**
   - Your fork will be at: `https://github.com/YOUR_USERNAME/aspire-hosting-quartz`

#### Step 2: Clone Your Fork

```bash
# Clone your fork (replace YOUR_USERNAME with your GitHub username)
git clone https://github.com/YOUR_USERNAME/aspire-hosting-quartz.git
cd aspire-hosting-quartz
```

#### Step 3: Add Upstream Remote

```bash
# Add the original repository as "upstream"
git remote add upstream https://github.com/alnuaimicoder/aspire-hosting-quartz.git

# Verify remotes
git remote -v
# Should show:
# origin    https://github.com/YOUR_USERNAME/aspire-hosting-quartz.git (fetch)
# origin    https://github.com/YOUR_USERNAME/aspire-hosting-quartz.git (push)
# upstream  https://github.com/alnuaimicoder/aspire-hosting-quartz.git (fetch)
# upstream  https://github.com/alnuaimicoder/aspire-hosting-quartz.git (push)
```

#### Step 4: Build and Test

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test
```

## 📝 Development Workflow

### Fork Workflow (Recommended)

This project uses the **Fork & Pull Request** workflow:

```
┌─────────────────────────────────────────────────────────┐
│  Original Repository (upstream)                         │
│  github.com/alnuaimicoder/aspire-hosting-quartz        │
└────────────────────┬────────────────────────────────────┘
                     │ Fork
                     ▼
┌─────────────────────────────────────────────────────────┐
│  Your Fork (origin)                                     │
│  github.com/YOUR_USERNAME/aspire-hosting-quartz        │
└────────────────────┬────────────────────────────────────┘
                     │ Clone
                     ▼
┌─────────────────────────────────────────────────────────┐
│  Local Repository                                       │
│  Your Computer                                          │
└─────────────────────────────────────────────────────────┘
```

### Keeping Your Fork Updated

**Important:** Always sync your fork before starting new work!

```bash
# 1. Fetch latest changes from upstream
git fetch upstream

# 2. Switch to main branch
git checkout main

# 3. Merge upstream changes
git merge upstream/main

# 4. Push to your fork
git push origin main
```

### Creating a Feature Branch

**Never work directly on the main branch!**

```bash
# Create and switch to a new branch
git checkout -b feature/your-feature-name

# Or for bug fixes
git checkout -b fix/bug-description
```

### Before You Start

1. **Check existing work:**
   - Browse [issues](https://github.com/alnuaimicoder/aspire-hosting-quartz/issues)
   - Check [pull requests](https://github.com/alnuaimicoder/aspire-hosting-quartz/pulls)
   - Avoid duplicate work

2. **Discuss major changes:**
   - Open an issue first
   - Explain your proposal
   - Get feedback from maintainers

3. **Sync your fork:**
   ```bash
   # Fetch latest changes from upstream
   git fetch upstream

   # Merge upstream changes into your main branch
   git checkout main
   git merge upstream/main

   # Push updates to your fork
   git push origin main
   ```

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

5. **Push to your fork:**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create Pull Request:**
   - Go to your fork on GitHub
   - Click "Compare & pull request"
   - Fill in the PR template
   - Submit the PR

7. **Keep your PR updated:**
   ```bash
   # If main branch has new commits
   git checkout main
   git pull upstream main
   git checkout feature/your-feature-name
   git rebase main
   git push origin feature/your-feature-name --force
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

## 🔧 Troubleshooting

### Fork Issues

**Problem: Can't see "Fork" button**
- Solution: Make sure you're logged into GitHub
- The repository must be public

**Problem: Fork is out of date**
```bash
# Sync your fork with upstream
git fetch upstream
git checkout main
git merge upstream/main
git push origin main
```

**Problem: Merge conflicts**
```bash
# Update your branch with latest main
git checkout main
git pull upstream main
git checkout your-branch
git rebase main

# Resolve conflicts in your editor
# Then continue rebase
git add .
git rebase --continue
git push origin your-branch --force
```

### Build Issues

**Problem: Build fails**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

**Problem: Tests fail**
```bash
# Make sure Docker is running (for integration tests)
docker ps

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"
```

### Git Issues

**Problem: Accidentally committed to main**
```bash
# Create a new branch from current state
git branch feature/my-feature

# Reset main to upstream
git checkout main
git reset --hard upstream/main

# Switch to your feature branch
git checkout feature/my-feature
```

**Problem: Need to update PR with new commits**
```bash
# Add your changes
git add .
git commit -m "fix: address review comments"

# Push to your fork (updates PR automatically)
git push origin your-branch
```

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing! 🎉

