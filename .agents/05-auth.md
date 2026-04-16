# Agent: Auth - ASP.NET Core JWT

## Genel Akis

```text
[Vue] POST /api/v1/auth/login
   -> [ASP.NET] credentials validate + JWT produce
   -> [Vue] token localStorage
   -> [Vue] her request'te Authorization: Bearer <token>
   -> [ASP.NET] JwtBearer middleware token validate
```

## Program.cs JWT Configuration

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });
```

## Auth Endpoints
- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `GET /api/v1/auth/me` (auth gerekli)

## Password Kurallari
- Plain password asla DB'ye yazilmaz.
- `BCrypt.Net.BCrypt.HashPassword(...)`
- Login'de `BCrypt.Verify(...)`

## appsettings.json (ornek)
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=devboard;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Issuer": "devboard-api",
    "Audience": "devboard-client",
    "Secret": "CHANGE_ME_TO_A_LONG_RANDOM_SECRET_KEY",
    "ExpirationMinutes": 1440
  }
}
```

## Frontend Tarafi
- Login sonrasi token Pinia store + localStorage
- Axios request interceptor token ekler
- 401 alindiginda logout + login sayfasina donus
