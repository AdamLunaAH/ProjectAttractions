using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Configuration;
using DbModels;
using Microsoft.Extensions.Hosting.Internal;
using DbContext.Extensions;
using Models;
using Models.DTO;

namespace DbContext;

//DbContext namespace is a fundamental EFC layer of the database context and is
//used for all Database connection as well as for EFC CodeFirst migration and database updates
public class MainDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    DatabaseConnections _databaseConnections;

#if DEBUG
    // remove password from connection string in debug mode
    // this is useful for debugging and logging purposes, but should not be used in production code
    public string dbConnection => System.Text.RegularExpressions.Regex.Replace(
        this.Database.GetConnectionString() ?? "", @"(pwd|password)=[^;]*;?", "",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
#endif

    #region C# model of database tables
    // public DbSet<CreditCardDbM> CreditCards { get; set; }
    public DbSet<AttractionsDbM> Attractions { get; set; }
    public DbSet<AttractionAddressesDbM> AttractionAddresses { get; set; }
    public DbSet<CategoriesDbM> Categories
    {
        get; set;
    }
    // public DbSet<AttractionCategoriesDbM> AttractionCategories { get; set; }
    public DbSet<UsersDbM> Users { get; set; }
    public DbSet<ReviewsDbM> Reviews { get; set; }
    #endregion


    #region model the Views
    public DbSet<SupUsrInfoDbDto> SUInfoDbView { get; set; }
    #endregion


    #region constructors
    public MainDbContext() { }
    public MainDbContext(DbContextOptions options, DatabaseConnections databaseConnections) : base(options)
    {
        _databaseConnections = databaseConnections;
    }
    #endregion

    //Here we can modify the migration building


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region  model the Views
        modelBuilder.Entity<SupUsrInfoDbDto>()
            .ToView("vw_SupUsrInfoDb", "supusr")
            .HasNoKey();
        #endregion

        #region override modelbuilder
        // Users
        modelBuilder.Entity<UsersDbM>(entity =>
        {
            entity.ToTable("UsersDb", schema: "supusr");

            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => new { e.FirstName, e.LastName });
            entity.HasIndex(e => new { e.LastName, e.FirstName });

            entity.HasIndex(e => e.Email).IsUnique();

            // One User -> Many Reviews
            entity.HasMany(u => u.ReviewsDbM)
                    .WithOne(r => r.UsersDbM)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Categories
        modelBuilder.Entity<CategoriesDbM>(entity =>
        {
            entity.ToTable("CategoriesDb", schema: "supusr");

            entity.HasKey(e => e.CategoryId);

            entity.HasIndex(e => e.CategoryName).IsUnique();
        });

        // AttractionAddresses
        modelBuilder.Entity<AttractionAddressesDbM>(entity =>
        {
            entity.ToTable("AttractionAddressesDb", schema: "supusr");

            entity.HasKey(e => e.AddressId);

            entity.HasIndex(e => new { e.StreetAddress, e.ZipCode, e.CityPlace, e.Country }).IsUnique();
        });

        // Attractions
        modelBuilder.Entity<AttractionsDbM>(entity =>
        {
            entity.ToTable("AttractionsDb", schema: "supusr");

            entity.HasKey(e => e.AttractionId);

            entity.HasIndex(e => new { e.AttractionName, e.AttractionDescription, e.AddressId })
                    .IsUnique();

            // One Address -> Many Attractions
            entity.HasOne(a => a.AttractionAddressesDbM)
                    .WithMany()
                    .HasForeignKey(a => a.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

            // Many Attractions <-> Many Categories (join table)
            entity.HasMany(a => a.CategoriesDbM)
                    .WithMany(c => c.AttractionsDbM)
                    .UsingEntity<Dictionary<string, object>>(
                        "AttractionCategories",
                        j => j.HasOne<CategoriesDbM>()
                            .WithMany()
                            .HasForeignKey("CategoryId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<AttractionsDbM>()
                            .WithMany()
                            .HasForeignKey("AttractionId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.ToTable("AttractionCategories", schema: "supusr");
                            j.HasKey("AttractionId", "CategoryId");
                        });

            // One Attraction -> Many Reviews
            entity.HasMany(a => a.ReviewsDbM)
                    .WithOne(r => r.AttractionsDbM)
                    .HasForeignKey(r => r.AttractionId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        // Reviews
        modelBuilder.Entity<ReviewsDbM>(entity =>
        {
            entity.ToTable("ReviewsDb", schema: "supusr");

            entity.HasKey(e => e.ReviewId);

            entity.HasIndex(e => new { e.ReviewScore, e.ReviewText });
            entity.HasIndex(e => new { e.AttractionId, e.UserId }).IsUnique();

        });
        base.OnModelCreating(modelBuilder);
        #endregion
    }

    #region DbContext for some popular databases
    public class SqlServerDbContext : MainDbContext
    {
        public SqlServerDbContext() { }
        public SqlServerDbContext(DbContextOptions options, DatabaseConnections databaseConnections)
            : base(options, databaseConnections) { }


        //Used only for CodeFirst Database Migration and database update commands
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder = optionsBuilder.ConfigureForDesignTime(
                    (options, connectionString) => options.UseSqlServer(connectionString, options => options.EnableRetryOnFailure()));
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HaveColumnType("money");
            configurationBuilder.Properties<string>().HaveColumnType("nvarchar(200)");

            base.ConfigureConventions(configurationBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add your own modelling based on done migrations
            base.OnModelCreating(modelBuilder);
        }
    }

    public class MySqlDbContext : MainDbContext
    {
        public MySqlDbContext() { }
        public MySqlDbContext(DbContextOptions options) : base(options, null) { }


        //Used only for CodeFirst Database Migration
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder = optionsBuilder.ConfigureForDesignTime(
                    (options, connectionString) =>
                        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                            b => b.SchemaBehavior(Pomelo.EntityFrameworkCore.MySql.Infrastructure.MySqlSchemaBehavior.Translate, (schema, table) => $"{schema}_{table}")));
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().HaveColumnType("nvarchar(200)");

            base.ConfigureConventions(configurationBuilder);

        }
    }

    public class PostgresDbContext : MainDbContext
    {
        public PostgresDbContext() { }
        public PostgresDbContext(DbContextOptions options) : base(options, null) { }


        //Used only for CodeFirst Database Migration
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder = optionsBuilder.ConfigureForDesignTime(
                    (options, connectionString) => options.UseNpgsql(connectionString));
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>().HaveColumnType("nvarchar(200)");
            base.ConfigureConventions(configurationBuilder);
        }
    }
    #endregion
}
