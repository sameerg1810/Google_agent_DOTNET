using Microsoft.EntityFrameworkCore;
using GenAiApp.Backend.Models;

namespace GenAiApp.Backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    // DbSet represents the 'Chats' table in your SQL Database
    public DbSet<ChatHistory> Chats => Set<ChatHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Industrial Best Practice: Use Fluent API for complex configuration
        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasIndex(e => e.UserId); // Speeds up searches for a specific user's chat
        });
    }
}