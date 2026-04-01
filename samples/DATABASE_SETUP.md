# Database Setup Guide

This sample demonstrates how to use Quartz.NET with different database providers. The library supports **PostgreSQL, SQL Server, MySQL, and SQLite** with automatic table creation using Entity Framework Core migrations.

## Supported Databases

| Database | Package | Status |
|----------|---------|--------|
| PostgreSQL | `AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL` | ✅ Active in sample |
| SQL Server | `AppAny.Quartz.EntityFrameworkCore.Migrations.SqlServer` | 💡 Commented (ready to use) |
| MySQL | `AppAny.Quartz.EntityFrameworkCore.Migrations.MySql` | 💡 Commented (ready to use) |
| SQLite | `AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite` | 💡 Commented (ready to use) |

## How It Works

The sample uses **AppAny.Quartz.EntityFrameworkCore.Migrations** library which:
- ✅ Automatically creates Quartz.NET tables on startup
- ✅ Handles schema migrations
- ✅ No manual SQL scripts needed
- ✅ Production-ready approach

## Switch to SQL Server

### 1. Update AppHost.cs

```csharp
// Comment out PostgreSQL
// var postgres = builder.AddPostgres("postgres")
//     .WithLifetime(ContainerLifetime.Persistent)
//     .WithPgAdmin()
//     .AddDatabase("quartzdb");

// Uncomment SQL Server
var sqlserver = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("quartzdb");

var apiService = builder.AddProject<Projects.QuartzSample_ApiService>("apiservice")
    .WithReference(sqlserver); // Change to sqlserver
```

### 2. Update QuartzDbContext.cs

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Comment out PostgreSQL
    // modelBuilder.AddQuartz(builder => builder.UsePostgreSql());

    // Uncomment SQL Server
    modelBuilder.AddQuartz(builder => builder.UseSqlServer());
}
```

### 3. Update Program.cs

```csharp
// Comment out PostgreSQL DbContext
// builder.Services.AddDbContext<QuartzDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("quartzdb")));

// Uncomment SQL Server DbContext
builder.Services.AddDbContext<QuartzDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("quartzdb")));

// Comment out PostgreSQL Quartz config
// q.UsePersistentStore(store =>
// {
//     store.UsePostgres(builder.Configuration.GetConnectionString("quartzdb")!);
//     store.UseNewtonsoftJsonSerializer();
// });

// Uncomment SQL Server Quartz config
q.UsePersistentStore(store =>
{
    store.UseSqlServer(builder.Configuration.GetConnectionString("quartzdb")!);
    store.UseNewtonsoftJsonSerializer();
});
```

### 4. Install SQL Server Migration Package

```bash
cd samples/QuartzSample.ApiService
dotnet add package AppAny.Quartz.EntityFrameworkCore.Migrations.SqlServer
```

## Switch to MySQL

### 1. Update AppHost.cs

```csharp
var mysql = builder.AddMySql("mysql")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("quartzdb");

var apiService = builder.AddProject<Projects.QuartzSample_ApiService>("apiservice")
    .WithReference(mysql);
```

### 2. Update QuartzDbContext.cs

```csharp
modelBuilder.AddQuartz(builder => builder.UseMySql());
```

### 3. Update Program.cs

```csharp
builder.Services.AddDbContext<QuartzDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("quartzdb"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("quartzdb"))));

q.UsePersistentStore(store =>
{
    store.UseMySql(builder.Configuration.GetConnectionString("quartzdb")!);
    store.UseNewtonsoftJsonSerializer();
});
```

### 4. Install MySQL Migration Package

```bash
dotnet add package AppAny.Quartz.EntityFrameworkCore.Migrations.MySql
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

## Switch to SQLite

### 1. Update AppHost.cs

```csharp
// SQLite doesn't need a container - just use a file path
var apiService = builder.AddProject<Projects.QuartzSample_ApiService>("apiservice");
```

### 2. Update appsettings.json

```json
{
  "ConnectionStrings": {
    "quartzdb": "Data Source=quartz.db"
  }
}
```

### 3. Update QuartzDbContext.cs

```csharp
modelBuilder.AddQuartz(builder => builder.UseSQLite());
```

### 4. Update Program.cs

```csharp
builder.Services.AddDbContext<QuartzDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("quartzdb")));

q.UsePersistentStore(store =>
{
    store.UseSQLite(builder.Configuration.GetConnectionString("quartzdb")!);
    store.UseNewtonsoftJsonSerializer();
});
```

### 5. Install SQLite Migration Package

```bash
dotnet add package AppAny.Quartz.EntityFrameworkCore.Migrations.SQLite
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

## Automatic Table Creation

The sample automatically creates all Quartz.NET tables on startup:

```csharp
// Run database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<QuartzDbContext>();
    await dbContext.Database.MigrateAsync();
}
```

This creates:
- `qrtz_job_details` - Job definitions
- `qrtz_triggers` - Trigger definitions
- `qrtz_cron_triggers` - Cron trigger details
- `qrtz_simple_triggers` - Simple trigger details
- `qrtz_fired_triggers` - Currently executing jobs
- `qrtz_scheduler_state` - Scheduler state
- `qrtz_locks` - Distributed locking
- `qrtz_calendars` - Calendar definitions
- `qrtz_paused_trigger_grps` - Paused trigger groups
- `qrtz_blob_triggers` - Blob trigger data
- `qrtz_idempotency_keys` - Idempotency tracking (custom)

## Production Deployment

For production, you can:

1. **Use automatic migrations** (current approach) - Simple and works well
2. **Generate SQL scripts** - For controlled deployments:

```bash
dotnet ef migrations add InitialQuartz --context QuartzDbContext
dotnet ef migrations script --context QuartzDbContext --output quartz-schema.sql
```

3. **Use external migration tools** - Like Flyway, Liquibase, or DbUp

## References

- [AppAny.Quartz.EntityFrameworkCore.Migrations](https://github.com/appany/AppAny.Quartz.EntityFrameworkCore.Migrations)
- [Quartz.NET Documentation](https://www.quartz-scheduler.net/)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
