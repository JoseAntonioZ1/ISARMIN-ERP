using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configuracion_empresa",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    razon_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ruc = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    logo = table.Column<string>(type: "text", nullable: true),
                    monto_apertura_caja_predeterminado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configuracion_empresa", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "configuracion_empresa",
                columns: new[] { "id", "direccion", "logo", "monto_apertura_caja_predeterminado", "razon_social", "ruc" },
                values: new object[] { new Guid("00000000-0000-0000-0000-0000000000e1"), null, null, null, "ISARMIN PERÚ S.A.C.", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "configuracion_empresa");
        }
    }
}
