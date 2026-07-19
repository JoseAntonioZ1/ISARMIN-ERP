using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SembrarPermisosAdministradorUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permisos",
                columns: new[] { "id", "accion", "modulo", "rol_id" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-0000000000b1"), "Crear", "Usuarios", new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0000-0000000000b2"), "Editar", "Usuarios", new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0000-0000000000b3"), "Eliminar", "Usuarios", new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0000-0000000000b4"), "Consultar", "Usuarios", new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0000-0000000000b5"), "Consultar", "Roles", new Guid("00000000-0000-0000-0000-000000000001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-0000000000b1"));

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-0000000000b2"));

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-0000000000b3"));

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-0000000000b4"));

            migrationBuilder.DeleteData(
                table: "permisos",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-0000000000b5"));
        }
    }
}
