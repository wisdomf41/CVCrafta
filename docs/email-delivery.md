# Email verification delivery

ResumeApp selects its email-verification sender with
`EmailDelivery:Provider`.

- Use `Development` only in the ASP.NET Core `Development` or `Testing`
  environment. This sender writes the confirmation link to development logs.
- Use `Smtp` for production-capable delivery. Production rejects the
  development sender.

Production SMTP settings must be supplied through environment variables or
ASP.NET Core user secrets. Do not place credentials in `appsettings*.json`,
Docker Compose files, source control, command output, or screenshots.

## Configuration names

| Environment variable | User-secret key | Purpose |
| --- | --- | --- |
| `EmailDelivery__Provider` | `EmailDelivery:Provider` | Selects `Smtp` or `Development`. |
| `App__PublicBaseUrl` | `App:PublicBaseUrl` | Absolute public HTTP/HTTPS base URL used in confirmation links. |
| `EmailDelivery__Smtp__Host` | `EmailDelivery:Smtp:Host` | SMTP server host. |
| `EmailDelivery__Smtp__Port` | `EmailDelivery:Smtp:Port` | SMTP server port. |
| `EmailDelivery__Smtp__Security` | `EmailDelivery:Smtp:Security` | MailKit security mode. |
| `EmailDelivery__Smtp__Username` | `EmailDelivery:Smtp:Username` | SMTP authentication username. |
| `EmailDelivery__Smtp__Password` | `EmailDelivery:Smtp:Password` | SMTP authentication password or app password. |
| `EmailDelivery__Smtp__SenderAddress` | `EmailDelivery:Smtp:SenderAddress` | Address shown in the message From header. |
| `EmailDelivery__Smtp__SenderName` | `EmailDelivery:Smtp:SenderName` | Display name shown in the message From header. |

Production accepts only `StartTls` or `SslOnConnect`, according to the
provider's documented port and TLS requirements. The other MailKit modes
(`Auto`, `None`, and `StartTlsWhenAvailable`) are rejected in Production
because they do not guarantee encryption. Use `None` only in Development or
Testing with an explicitly trusted local relay.

## Safe local configuration

For local SMTP testing, set each user secret interactively without sharing its
value:

```powershell
dotnet user-secrets set "EmailDelivery:Provider" "<provider>" --project ResumeApp.Server
dotnet user-secrets set "App:PublicBaseUrl" "<public-base-url>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:Host" "<smtp-host>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:Port" "<smtp-port>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:Security" "<tls-mode>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:Username" "<smtp-username>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:Password" "<smtp-password>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:SenderAddress" "<sender-address>" --project ResumeApp.Server
dotnet user-secrets set "EmailDelivery:Smtp:SenderName" "<sender-name>" --project ResumeApp.Server
```

Restart the server after configuration changes. Register a disposable local
test account, verify that the multipart message arrives, open the confirmation
link, and confirm that login remains blocked before confirmation and succeeds
afterward. Then use the resend action once and confirm that its response remains
generic.

Never use production recipients or credentials during local testing. Do not
copy confirmation URLs or tokens into issue trackers, chat, screenshots, or
shared logs.
