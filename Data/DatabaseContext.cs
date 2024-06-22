using fw_secure_notes_api.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PageModel = fw_secure_notes_api.Models.PageModel;

namespace fw_secure_notes_api.Data;

public class DatabaseContext : DbContext
{
    public DbSet<PageModel> Pages { get; set; }
    public DbSet<FileModel> Files { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PageModel>()
            .HasMany(p => p.Files)
            .WithOne(f => f.Page)
            .HasForeignKey(f => f.PageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FileModel>()
            .Property(f => f.Content)
            .HasColumnType("text");
    }
}
