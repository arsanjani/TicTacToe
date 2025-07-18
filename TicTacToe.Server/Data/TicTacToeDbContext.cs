using Microsoft.EntityFrameworkCore;
using TicTacToe.Server.Entities;

namespace TicTacToe.Server.Data;

public class TicTacToeDbContext : DbContext
{
    public TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : base(options)
    {
    }

    public DbSet<GameEntity> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicTacToeDbContext).Assembly);
    }
} 