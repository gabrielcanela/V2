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
    public DbSet<Precio> Precios => Set<Precio>();

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

        // Precio: única tabla nueva, gestionada por las migraciones de este proyecto.
        modelBuilder.Entity<Precio>(entity =>
        {
            entity.ToTable("Precio");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Proveedor).HasMaxLength(10).IsUnicode(false).IsRequired();
            entity.Property(e => e.ProductoProveed).HasMaxLength(20).IsUnicode(false).IsRequired();
            entity.Property(e => e.Categoria).HasMaxLength(3).IsUnicode(false).IsRequired();
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
            entity.Property(e => e.VigenciaDesde).HasColumnType("date");
            entity.Property(e => e.VigenciaHasta).HasColumnType("date");

            entity.HasOne(e => e.ProveedorNavigation)
                .WithMany(p => p.Precios)
                .HasForeignKey(e => e.Proveedor)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CategoriaNavigation)
                .WithMany(c => c.Precios)
                .HasForeignKey(e => e.Categoria)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProductoProveedorNavigation)
                .WithMany(pp => pp.Precios)
                .HasForeignKey(e => new { e.Proveedor, e.ProductoProveed })
                .OnDelete(DeleteBehavior.Restrict);

            // RNF-04: la consulta filtra siempre por vigencia y suele acotar por familia,
            // y agrupa/ordena por producto — estos índices cubren esos accesos.
            entity.HasIndex(e => new { e.VigenciaDesde, e.VigenciaHasta, e.Categoria })
                .HasDatabaseName("IX_Precio_Vigencia_Categoria");

            entity.HasIndex(e => e.ProductoProveed)
                .HasDatabaseName("IX_Precio_ProductoProveed");
        });
    }
}
