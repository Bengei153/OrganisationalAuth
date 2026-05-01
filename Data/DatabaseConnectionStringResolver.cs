using Microsoft.Extensions.Configuration;
using Npgsql;

namespace OrganisationalAuth.Data;

internal static class DatabaseConnectionStringResolver
{
    public static string Resolve(IConfiguration configuration)
    {
        var rawConnectionString =
            Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
            ?? Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? configuration.GetConnectionString("conString")
            ?? throw new InvalidOperationException(
                "No database connection string was found. Set DATABASE_CONNECTION_STRING, DATABASE_URL, or ConnectionStrings:conString.");

        return Normalize(rawConnectionString);
    }

    private static string Normalize(string rawConnectionString)
    {
        rawConnectionString = rawConnectionString.Trim();

        if (Uri.TryCreate(rawConnectionString, UriKind.Absolute, out var connectionUri)
            && (connectionUri.Scheme.Equals("postgres", StringComparison.OrdinalIgnoreCase)
                || connectionUri.Scheme.Equals("postgresql", StringComparison.OrdinalIgnoreCase)))
        {
            var credentials = connectionUri.UserInfo.Split(':', 2);
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = connectionUri.Host,
                Port = connectionUri.Port > 0 ? connectionUri.Port : 5432,
                Database = connectionUri.AbsolutePath.Trim('/'),
                Username = credentials.Length > 0 ? Uri.UnescapeDataString(credentials[0]) : string.Empty,
                Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : string.Empty,
                SslMode = SslMode.Require
            };

            return builder.ConnectionString;
        }

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(rawConnectionString);

        var host = connectionStringBuilder.Host ?? string.Empty;

        if (host.StartsWith("tcp://", StringComparison.OrdinalIgnoreCase))
        {
            var hostValue = host["tcp://".Length..];

            if (hostValue.Contains(':'))
            {
                var hostParts = hostValue.Split(':', 2);
                connectionStringBuilder.Host = hostParts[0];

                if (int.TryParse(hostParts[1], out var parsedPort))
                {
                    connectionStringBuilder.Port = parsedPort;
                }
            }
            else
            {
                connectionStringBuilder.Host = hostValue;
            }
        }

        if (connectionStringBuilder.Timeout == 15)
        {
            connectionStringBuilder.Timeout = 30;
        }

        if (connectionStringBuilder.CommandTimeout == 30)
        {
            connectionStringBuilder.CommandTimeout = 60;
        }

        if (connectionStringBuilder.KeepAlive == 0)
        {
            connectionStringBuilder.KeepAlive = 30;
        }

        return connectionStringBuilder.ConnectionString;
    }
}
