# Security Policy

## Supported Versions

We release patches for security vulnerabilities in the following versions:

| Version | Supported          |
| ------- | ------------------ |
| 1.x.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of CommunityToolkit.Aspire.Quartz seriously. If you believe you have found a security vulnerability, please report it to us as described below.

### Please Do Not

- **Do not** open a public GitHub issue for security vulnerabilities
- **Do not** disclose the vulnerability publicly until we've had a chance to address it

### Please Do

1. **Email us directly** at: [your-email@example.com] (replace with actual email)
2. **Include the following information:**
   - Type of vulnerability
   - Full paths of source file(s) related to the vulnerability
   - Location of the affected source code (tag/branch/commit or direct URL)
   - Step-by-step instructions to reproduce the issue
   - Proof-of-concept or exploit code (if possible)
   - Impact of the vulnerability, including how an attacker might exploit it

### What to Expect

- **Acknowledgment**: We will acknowledge receipt of your vulnerability report within 48 hours
- **Assessment**: We will assess the vulnerability and determine its impact and severity
- **Timeline**: We will provide an estimated timeline for a fix
- **Updates**: We will keep you informed of our progress
- **Credit**: We will credit you in the security advisory (unless you prefer to remain anonymous)

### Security Update Process

1. **Confirmation**: We confirm the vulnerability and determine affected versions
2. **Fix Development**: We develop a fix in a private repository
3. **Testing**: We thoroughly test the fix
4. **Release**: We release a security patch
5. **Advisory**: We publish a security advisory on GitHub
6. **Notification**: We notify users through:
   - GitHub Security Advisories
   - Release notes
   - NuGet package update

## Security Best Practices

When using CommunityToolkit.Aspire.Quartz, we recommend:

### 1. Connection Strings
- **Never** hardcode connection strings in source code
- Use Azure Key Vault, user secrets, or environment variables
- Rotate credentials regularly

### 2. Job Parameters
- **Validate** all job parameters before processing
- **Sanitize** user input to prevent injection attacks
- **Limit** parameter size to prevent DoS attacks

### 3. Database Security
- Use **least privilege** database accounts
- Enable **encryption at rest** for sensitive data
- Use **TLS/SSL** for database connections
- Regularly **backup** your Quartz database

### 4. Network Security
- Use **private networks** for database connections
- Enable **firewall rules** to restrict access
- Use **VPN** or **private endpoints** in production

### 5. Monitoring
- Enable **audit logging** for job execution
- Monitor for **unusual patterns** (excessive failures, etc.)
- Set up **alerts** for security events

### 6. Updates
- Keep CommunityToolkit.Aspire.Quartz **up to date**
- Monitor for **security advisories**
- Test updates in **non-production** environments first

## Known Security Considerations

### SQL Injection
- The library uses **parameterized queries** to prevent SQL injection
- Job parameters are **serialized as JSON** and stored safely
- **Do not** construct SQL queries from job parameters

### Denial of Service
- Job parameters are **limited to 1MB** by default
- Configure **MaxConcurrency** to prevent resource exhaustion
- Use **rate limiting** on job enqueuing endpoints

### Information Disclosure
- Connection strings are **sanitized** in logs
- Sensitive data should **not** be stored in job parameters
- Use **encryption** for sensitive job data

### Authentication & Authorization
- The library **does not** provide authentication
- Implement **authentication** at the API level
- Use **authorization** to control who can enqueue jobs
- Consider **API keys** or **OAuth** for production

## Compliance

### GDPR
- Job parameters may contain personal data
- Implement **data retention** policies
- Provide **data deletion** capabilities
- Document **data processing** activities

### HIPAA
- **Do not** store PHI in job parameters
- Use **encryption** for all data
- Implement **audit logging**
- Follow **HIPAA guidelines** for cloud services

## Security Contacts

- **Primary**: [your-email@example.com]
- **GitHub**: [@yourusername](https://github.com/yourusername)

## Acknowledgments

We would like to thank the following individuals for responsibly disclosing security vulnerabilities:

- (List will be updated as vulnerabilities are reported and fixed)

## Additional Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [.NET Security Best Practices](https://learn.microsoft.com/dotnet/standard/security/)
- [Azure Security Best Practices](https://learn.microsoft.com/azure/security/fundamentals/best-practices-and-patterns)
- [Quartz.NET Security](https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/security.html)

---

**Last Updated**: 2026-03-31
