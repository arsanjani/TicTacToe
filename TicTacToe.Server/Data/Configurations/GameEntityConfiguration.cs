using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToe.Server.Entities;

namespace TicTacToe.Server.Data.Configurations;

public class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.GameId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.GameId)
            .IsUnique();

        builder.Property(e => e.Player1Id)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Player1Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Player1Character)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.Player2Id)
            .HasMaxLength(100);

        builder.Property(e => e.Player2Name)
            .HasMaxLength(100);

        builder.Property(e => e.Player2Character)
            .HasConversion<string>();

        builder.Property(e => e.IsPrivate)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.State)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Models.GameState.WaitingForPlayers);

        builder.Property(e => e.Result)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(Models.GameResult.None);

        builder.Property(e => e.WinnerId)
            .HasMaxLength(100);

        builder.Property(e => e.WinnerName)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.TotalMoves)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.BoardState)
            .HasColumnType("nvarchar(max)")
            .HasDefaultValue(string.Empty);

        builder.Property(e => e.CurrentPlayerId)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        // Indexes for better performance
        builder.HasIndex(e => e.State);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.Player1Id);
        builder.HasIndex(e => e.Player2Id);
    }
} 