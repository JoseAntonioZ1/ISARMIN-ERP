using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Caja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.CreateTable(
                name: "cajas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_apertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    monto_apertura = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    fecha_cierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    monto_teorico_cierre = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    monto_fisico_declarado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Abierta")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cajas", x => x.id);
                    table.CheckConstraint("ck_cajas_estado", "estado IN ('Abierta','Cerrada')");
                    table.ForeignKey(
                        name: "fk_cajas_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movimientos_caja",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    caja_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    monto = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    concepto = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimientos_caja", x => x.id);
                    table.CheckConstraint("ck_movimientos_caja_concepto", "concepto IN ('GastoOperativo','RetiroPropietario','AporteCapital')");
                    table.CheckConstraint("ck_movimientos_caja_monto", "monto > 0");
                    table.CheckConstraint("ck_movimientos_caja_tipo", "tipo IN ('Ingreso','Egreso')");
                    table.ForeignKey(
                        name: "fk_movimientos_caja_cajas_caja_id",
                        column: x => x.caja_id,
                        principalTable: "cajas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movimientos_caja_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar','Abrir','Cerrar','Registrar')");

            migrationBuilder.CreateIndex(
                name: "ix_cajas_usuario_id",
                table: "cajas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_caja_caja_id_fecha",
                table: "movimientos_caja",
                columns: new[] { "caja_id", "fecha" });

            migrationBuilder.CreateIndex(
                name: "ix_movimientos_caja_usuario_id",
                table: "movimientos_caja",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movimientos_caja");

            migrationBuilder.DropTable(
                name: "cajas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar')");
        }
    }
}
