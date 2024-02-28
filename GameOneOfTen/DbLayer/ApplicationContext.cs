using GameOneOfTen.Models;
using Microsoft.EntityFrameworkCore;

namespace GameOneOfTen.DbLayer
{
    /// <summary>
    /// Custom implementation of DbContext for managing database access and entity objects.
    /// </summary>
    public class ApplicationContext : DbContext
    {
        /// <summary>
        /// Gets or sets the DbSet for accessing Player's entities in the database.
        /// </summary>
        public DbSet<Player> Players { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for accessing Bet's History entities in the database.
        /// </summary>
        public DbSet<BetHistory> BetHistories { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the ApplicationContext class with the specified options.
        /// </summary>
        /// <param name="options">The options for configuring the context.</param>
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
