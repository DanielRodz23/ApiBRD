using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ApiBRD.Models.Entities;

public partial class LabsystePwaBrdContext : DbContext
{
    public LabsystePwaBrdContext()
    {
    }

    public LabsystePwaBrdContext(DbContextOptions<LabsystePwaBrdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Menudeldia> Menudeldia { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categoria");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(20);
        });

        modelBuilder.Entity<Menudeldia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menudeldia");

            entity.HasIndex(e => e.IdProducto, "menudeldia_ibfk_1");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.IdProducto).HasColumnType("int(11)");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Menudeldia)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("menudeldia_ibfk_1");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("producto");

            entity.HasIndex(e => e.IdCategoria, "IdCategoria");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.IdCategoria).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasPrecision(10);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Producto)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("producto_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Password).HasMaxLength(128);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
