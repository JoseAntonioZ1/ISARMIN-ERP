using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PersonalizacionEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "color_acento",
                table: "configuracion_empresa",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mensaje_bienvenida",
                table: "configuracion_empresa",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "configuracion_empresa",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-0000000000e1"),
                columns: new[] { "color_acento", "mensaje_bienvenida" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color_acento",
                table: "configuracion_empresa");

            migrationBuilder.DropColumn(
                name: "mensaje_bienvenida",
                table: "configuracion_empresa");
        }
    }
}
