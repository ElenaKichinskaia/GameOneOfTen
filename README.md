# GameOneOfTen

Project Startup Process
This README file outlines the two main steps to start the project.

1. Configuring Connection String
Before running the project, it's necessary to configure the connection string in the appsettings.json file.

Example appsettings.json file:
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabaseName;User=YourUsername;Password=YourPassword;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

Replace "YourDatabaseName", "YourUsername", and "YourPassword" with the corresponding values of your database.

2. Applying Database Migrations
The project includes database migrations that need to be applied before running.

Commands to Apply Migrations
Open the command prompt or terminal in the project directory and execute the following commands:

To apply all available migrations:

`dotnet ef database update`

To create a new migration (if there are changes in the models):

`dotnet ef migrations add <MigrationName>`

Where <MigrationName> is the name of the new migration.

After successfully completing these steps, you can run the project.
