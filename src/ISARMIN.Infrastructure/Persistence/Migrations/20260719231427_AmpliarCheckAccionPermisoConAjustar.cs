using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AmpliarCheckAccionPermisoConAjustar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular')");
        }
    }
}
