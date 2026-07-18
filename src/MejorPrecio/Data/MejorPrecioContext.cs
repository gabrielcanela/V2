using Microsoft.EntityFrameworkCore;
using MejorPrecio.Data.Entities;

namespace MejorPrecio.Data;

public class MejorPrecioContext : DbContext
{
    public MejorPrecioContext(DbContextOptions<MejorPrecioContext> options)
        : base(options)
    {
    }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<ProductoProveedor> ProductosProveedor => Set<ProductoProveedor>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<PrecioProveedor> PreciosProveedor => Set<PrecioProveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tablas maestras existentes: se mapean para consulta/validación pero no las gestiona
        // este proyecto, por eso quedan excluidas de las migraciones (ExcludeFromMigrations).
        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedor", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasColumnName("Proveedor").HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.Descripcion).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.CUIT).HasMaxLength(11).IsUnicode(false);
            entity.Property(e => e.Estado).HasMaxLength(1).IsUnicode(false).IsRequired();
            entity.Property(e => e.Tipo).HasMaxLength(10).IsFixedLength();
            entity.Property(e => e.Email).HasMaxLength(512).IsUnicode(false);
            entity.Property(e => e.ImporteMinOC).HasColumnType("numeric(18,0)");
            entity.Property(e => e.Domicilio).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Localidad).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Provincia).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.CodigoPostal).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.EstadoId).HasMaxLength(5).IsUnicode(false);
            entity.Property(e => e.Pais).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.ModalidadPed).HasMaxLength(1).IsUnicode(false);
        });

        modelBuilder.Entity<ProductoProveedor>(entity =>
        {
            entity.ToTable("ProductoProveedor", t => t.ExcludeFromMigrations());
            entity.HasKey(e => new { e.Proveedor, e.ProductoProveed });
            entity.Property(e => e.Proveedor).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.ProductoProveed).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Descripcion).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.UnidadProveed).HasMaxLength(10).IsUnicode(false);

            entity.HasOne(e => e.ProveedorNavigation)
                .WithMany(p => p.Productos)
                .HasForeignKey(e => e.Proveedor)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categoria", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasColumnName("Categoria").HasMaxLength(3).IsUnicode(false);
            entity.Property(e => e.Descripcion).HasMaxLength(30).IsUnicode(false).IsRequired();
            entity.Property(e => e.Grupo).HasMaxLength(3).IsUnicode(false).IsRequired();
            entity.Property(e => e.Presentar).HasMaxLength(3).IsUnicode(false).IsRequired();
        });

        // PrecioProveedor: tabla existente de la base real, igual que las tablas maestras
        // de arriba, por eso también queda excluida de las migraciones.
        modelBuilder.Entity<PrecioProveedor>(entity =>
        {
            entity.ToTable("PrecioProveedor", t => t.ExcludeFromMigrations());
            entity.HasKey(e => e.Precioid);
            entity.Property(e => e.Precioid).HasColumnType("numeric(18,0)");
            entity.Property(e => e.Proveedor).HasMaxLength(10).IsUnicode(false).IsRequired();
            entity.Property(e => e.Producto).HasMaxLength(50).IsUnicode(false).IsRequired();
            entity.Property(e => e.Region).HasMaxLength(3).IsUnicode(false).IsRequired();
            entity.Property(e => e.FchDDe).HasColumnType("date");
            entity.Property(e => e.FchHta).HasColumnType("date");
            entity.Property(e => e.Precio).HasColumnType("numeric(10,2)");
            entity.Property(e => e.FchCreacion).HasColumnType("datetime");
            entity.Property(e => e.Zona).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.ListaprecioId).HasColumnType("numeric(18,0)");
            entity.Property(e => e.EstadoLP).HasColumnType("numeric(1,0)");
            entity.Property(e => e.ComedorLP).HasMaxLength(10).IsUnicode(false).IsRequired();
            entity.Property(e => e.TipoPrecio).HasMaxLength(1).IsUnicode(false);
        });
    }
}
