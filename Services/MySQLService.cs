using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Zorro.Services;

public static class MySQLService
{
    public static MySqlDbContextOptionsBuilder GetDefaultOptions(MySqlDbContextOptionsBuilder builder)
    {
        /*
        var optionsBuilder = builder.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );
        */
        var optionsBuilder = builder.ExecutionStrategy(_ => new NonRetryingExecutionStrategy(_));
        return optionsBuilder;
    }

    public delegate MySqlConnectionStringBuilder ConnectionStringBuilder(MySqlConnectionStringBuilder builder);
    public static ConnectionStringBuilder? ConnectionStringMaster { get; set; } = null;

    public static MySqlConnectionStringBuilder DefaultConnectionStringBuilder { get; set; } = new();

    public static void UseMySQL(DbContextOptionsBuilder options)
    {
        UseMySQL(options, null, true);
    }

    public static void UseMySQL(
        DbContextOptionsBuilder options,
        Action<MySqlDbContextOptionsBuilder>? optionsBuilder,
        bool useDefaultOptions
    )
    {
        MySqlConnectionStringBuilder connectionStringBuilder = new(DefaultConnectionStringBuilder.ConnectionString);

        if (ConnectionStringMaster is not null)
            ConnectionStringMaster.Invoke(connectionStringBuilder);

        string connectionString = connectionStringBuilder.ToString();

        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString),
            mySQLOptions =>
            {
                if (useDefaultOptions)
                    mySQLOptions = GetDefaultOptions(mySQLOptions);
                optionsBuilder?.Invoke(mySQLOptions);
            }
        );
    }

}