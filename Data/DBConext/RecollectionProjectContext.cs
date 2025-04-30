using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.DBConext;

public partial class RecollectionProjectContext : DbContext
{
    public RecollectionProjectContext()
    {
    }

    public RecollectionProjectContext(DbContextOptions<RecollectionProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MaterialType> MaterialTypes { get; set; }

    public virtual DbSet<Recolection> Recolections { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-TTQK5FS\\SQLEXPRESS; Database=RecollectionProject; User Id=dev; Password=123456789; Encrypt=true; TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialType>(entity =>
        {
            entity.HasKey(e => e.MaterialTypeId).HasName("PK__material__03BBF50BB7B6BBD1");
        });

        modelBuilder.Entity<Recolection>(entity =>
        {
            entity.HasKey(e => e.RecolectionId).HasName("PK__recolect__F5F28655F9E721DD");

            entity.HasOne(d => d.MaterialType).WithMany(p => p.Recolections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__recolecti__mater__3C69FB99");

            entity.HasOne(d => d.User).WithMany(p => p.Recolections)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__recolecti__user___3B75D760");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370F81A9BCA1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
