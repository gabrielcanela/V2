using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MejorPrecio.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatePrecio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Precio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Proveedor = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    ProductoProveed = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Categoria = table.Column<string>(type: "varchar(3)", unicode: false, maxLength: 3, nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VigenciaDesde = table.Column<DateOnly>(type: "date", nullable: false),
                    VigenciaHasta = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Precio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Precio_Categoria_Categoria",
                        column: x => x.Categoria,
                        principalTable: "Categoria",
                        principalColumn: "Categoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Precio_ProductoProveedor_Proveedor_ProductoProveed",
                        columns: x => new { x.Proveedor, x.ProductoProveed },
                        principalTable: "ProductoProveedor",
                        principalColumns: new[] { "Proveedor", "ProductoProveed" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Precio_Proveedor_Proveedor",
                        column: x => x.Proveedor,
                        principalTable: "Proveedor",
                        principalColumn: "Proveedor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Precio_Categoria",
                table: "Precio",
                column: "Categoria");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_ProductoProveed",
                table: "Precio",
                column: "ProductoProveed");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_Proveedor_ProductoProveed",
                table: "Precio",
                columns: new[] { "Proveedor", "ProductoProveed" });

            migrationBuilder.CreateIndex(
                name: "IX_Precio_Vigencia_Categoria",
                table: "Precio",
                columns: new[] { "VigenciaDesde", "VigenciaHasta", "Categoria" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Precio");
        }
    }
}
