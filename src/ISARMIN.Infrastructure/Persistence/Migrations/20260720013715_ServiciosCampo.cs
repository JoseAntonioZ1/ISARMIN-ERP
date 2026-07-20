using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ServiciosCampo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.CreateTable(
                name: "servicios_campo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    descripcion_trabajo = table.Column<string>(type: "text", nullable: false),
                    fecha_solicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tecnico_asignado_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Solicitado"),
                    monto_estimado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    fecha_ejecucion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado_final = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    usuario_cierre_id = table.Column<Guid>(type: "uuid", nullable: true),
                    medio_pago_id = table.Column<Guid>(type: "uuid", nullable: true),
                    monto_pagado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    saldo_pendiente = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    usuario_autorizo_saldo_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_servicios_campo", x => x.id);
                    table.CheckConstraint("ck_servicios_campo_estado", "estado IN ('Solicitado','Agendado','EnEjecucion','Cerrado')");
                    table.ForeignKey(
                        name: "fk_servicios_campo_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_servicios_campo_medios_pago_medio_pago_id",
                        column: x => x.medio_pago_id,
                        principalTable: "medios_pago",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_servicios_campo_usuarios_tecnico_asignado_id",
                        column: x => x.tecnico_asignado_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_servicios_campo_usuarios_usuario_autorizo_saldo_id",
                        column: x => x.usuario_autorizo_saldo_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_servicios_campo_usuarios_usuario_cierre_id",
                        column: x => x.usuario_cierre_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "servicios_campo_detalle",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    servicio_campo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_servicios_campo_detalle", x => x.id);
                    table.CheckConstraint("ck_servicios_campo_detalle_cantidad", "cantidad > 0");
                    table.ForeignKey(
                        name: "fk_servicios_campo_detalle_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_servicios_campo_detalle_servicios_campo_servicio_campo_id",
                        column: x => x.servicio_campo_id,
                        principalTable: "servicios_campo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar','Abrir','Cerrar','Registrar','Recepcionar','Diagnosticar','Cotizar','Reparar','Entregar','Cobrar')");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_cliente_id",
                table: "servicios_campo",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_medio_pago_id",
                table: "servicios_campo",
                column: "medio_pago_id");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_tecnico_asignado_id",
                table: "servicios_campo",
                column: "tecnico_asignado_id");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_usuario_autorizo_saldo_id",
                table: "servicios_campo",
                column: "usuario_autorizo_saldo_id");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_usuario_cierre_id",
                table: "servicios_campo",
                column: "usuario_cierre_id");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_detalle_producto_id",
                table: "servicios_campo_detalle",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_servicios_campo_detalle_servicio_campo_id",
                table: "servicios_campo_detalle",
                column: "servicio_campo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "servicios_campo_detalle");

            migrationBuilder.DropTable(
                name: "servicios_campo");

            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar','Abrir','Cerrar','Registrar','Recepcionar','Diagnosticar','Cotizar','Reparar','Entregar')");
        }
    }
}
