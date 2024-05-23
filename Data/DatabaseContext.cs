using fw_secure_notes_api.Models;
using Microsoft.EntityFrameworkCore;

namespace fw_secure_notes_api.Data;

public class DatabaseContext : DbContext
{
    public DbSet<PageModel> Pages { get; set; }
    public DbSet<FileModel> Files { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileModel>()
            .HasOne(f => f.Page)
            .WithMany(p => p.Files)
            .HasForeignKey(f => f.PageId);
    }
}
