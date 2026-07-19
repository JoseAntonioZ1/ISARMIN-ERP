using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Clientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_razon_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    tipo_documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    numero_documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tipo_cliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clientes", x => x.id);
                    table.CheckConstraint("ck_clientes_estado", "estado IN ('Activo','Inactivo')");
                    table.CheckConstraint("ck_clientes_tipo_cliente", "tipo_cliente IS NULL OR tipo_cliente IN ('Natural','Juridica')");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clientes");
        }
    }
}
