# Security Policy

## Supported Versions
This project targets .NET 9 and is actively maintained. Security updates are provided for the latest release.

## Reporting a Vulnerability
If you discover a security vulnerability, please report it privately to our security team at [security@sofdigital.net](mailto:security@sofdigital.net). 

Do not create public issues regarding security concerns.

Please include:
- A clear description of the vulnerability
- Steps to reproduce the issue
- Potential impact and affected components
- Any suggested remediation

We will acknowledge receipt within 3 business days and work to resolve the issue promptly.

## Security Best Practices
- **Authentication:** Use secure authentication mechanisms (e.g., Azure AD B2C, JWT). Never hard-code secrets or credentials.
- **Data Protection:** Protect sensitive data using encryption at rest and in transit.
- **Dependencies:** Keep all dependencies up to date and monitor for known vulnerabilities.
- **Input Validation:** Validate and sanitize all user input to prevent injection attacks.
- **Access Control:** Apply least privilege principles to all resources and services.
- **Configuration:** Store secrets and configuration securely (e.g., Azure Key Vault, environment variables).

## Disclosure Policy
We request responsible disclosure. Please allow us reasonable time to address vulnerabilities before public disclosure.

## Additional Resources
- [Microsoft Secure Development Lifecycle](https://www.microsoft.com/en-us/securityengineering/sdl/)
- [OWASP Top Ten](https://owasp.org/www-project-top-ten/)
