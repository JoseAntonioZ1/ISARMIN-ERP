using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Taller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.CreateTable(
                name: "ordenes_trabajo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipo_descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    falla_reportada = table.Column<string>(type: "text", nullable: false),
                    fecha_recepcion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    usuario_recepcion_id = table.Column<Guid>(type: "uuid", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Recibido"),
                    fecha_entrega = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_entrega_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estado_pago = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    monto_pagado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    saldo_pendiente = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    usuario_autorizo_saldo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resultado_pruebas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ordenes_trabajo", x => x.id);
                    table.CheckConstraint("ck_ordenes_trabajo_estado", "estado IN ('Recibido','Diagnosticado','Cotizado','Aprobado','Rechazado','EnReparacion','EnPruebas','ListoParaEntrega','Entregado')");
                    table.CheckConstraint("ck_ordenes_trabajo_estado_pago", "estado_pago IN ('CompletoAntes','CompletoAlMomento','Adelanto','SaldoPendiente') OR estado_pago IS NULL");
                    table.ForeignKey(
                        name: "fk_ordenes_trabajo_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ordenes_trabajo_usuarios_usuario_autorizo_saldo_id",
                        column: x => x.usuario_autorizo_saldo_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ordenes_trabajo_usuarios_usuario_entrega_id",
                        column: x => x.usuario_entrega_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ordenes_trabajo_usuarios_usuario_recepcion_id",
                        column: x => x.usuario_recepcion_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "consumos_repuesto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_trabajo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    producto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consumos_repuesto", x => x.id);
                    table.CheckConstraint("ck_consumos_repuesto_cantidad", "cantidad > 0");
                    table.ForeignKey(
                        name: "fk_consumos_repuesto_ordenes_trabajo_orden_trabajo_id",
                        column: x => x.orden_trabajo_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_consumos_repuesto_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cotizaciones_reparacion",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_trabajo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monto_estimado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    decision_cliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    cobro_diagnostico_rechazo = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    evidencia_aprobacion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cotizaciones_reparacion", x => x.id);
                    table.CheckConstraint("ck_cotizaciones_reparacion_decision_cliente", "decision_cliente IN ('Aprobada','Rechazada') OR decision_cliente IS NULL");
                    table.ForeignKey(
                        name: "fk_cotizaciones_reparacion_ordenes_trabajo_orden_trabajo_id",
                        column: x => x.orden_trabajo_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "diagnosticos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_trabajo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_diagnosticos", x => x.id);
                    table.ForeignKey(
                        name: "fk_diagnosticos_ordenes_trabajo_orden_trabajo_id",
                        column: x => x.orden_trabajo_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_diagnosticos_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "garantias",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    orden_trabajo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin = table.Column<DateOnly>(type: "date", nullable: false),
                    orden_trabajo_reingreso_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_garantias", x => x.id);
                    table.CheckConstraint("ck_garantias_fecha_fin", "fecha_fin > fecha_inicio");
                    table.ForeignKey(
                        name: "fk_garantias_ordenes_trabajo_orden_trabajo_id",
                        column: x => x.orden_trabajo_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_garantias_ordenes_trabajo_orden_trabajo_reingreso_id",
                        column: x => x.orden_trabajo_reingreso_id,
                        principalTable: "ordenes_trabajo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar','Abrir','Cerrar','Registrar','Recepcionar','Diagnosticar','Cotizar','Reparar','Entregar')");

            migrationBuilder.CreateIndex(
                name: "ix_consumos_repuesto_orden_trabajo_id",
                table: "consumos_repuesto",
                column: "orden_trabajo_id");

            migrationBuilder.CreateIndex(
                name: "ix_consumos_repuesto_producto_id",
                table: "consumos_repuesto",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_cotizaciones_reparacion_orden_trabajo_id",
                table: "cotizaciones_reparacion",
                column: "orden_trabajo_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_diagnosticos_orden_trabajo_id",
                table: "diagnosticos",
                column: "orden_trabajo_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_diagnosticos_usuario_id",
                table: "diagnosticos",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_garantias_orden_trabajo_id",
                table: "garantias",
                column: "orden_trabajo_id");

            migrationBuilder.CreateIndex(
                name: "ix_garantias_orden_trabajo_reingreso_id",
                table: "garantias",
                column: "orden_trabajo_reingreso_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_trabajo_cliente_id",
                table: "ordenes_trabajo",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_trabajo_usuario_autorizo_saldo_id",
                table: "ordenes_trabajo",
                column: "usuario_autorizo_saldo_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_trabajo_usuario_entrega_id",
                table: "ordenes_trabajo",
                column: "usuario_entrega_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_trabajo_usuario_recepcion_id",
                table: "ordenes_trabajo",
                column: "usuario_recepcion_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consumos_repuesto");

            migrationBuilder.DropTable(
                name: "cotizaciones_reparacion");

            migrationBuilder.DropTable(
                name: "diagnosticos");

            migrationBuilder.DropTable(
                name: "garantias");

            migrationBuilder.DropTable(
                name: "ordenes_trabajo");

            migrationBuilder.DropCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos");

            migrationBuilder.AddCheckConstraint(
                name: "ck_permisos_accion",
                table: "permisos",
                sql: "accion IN ('Crear','Editar','Eliminar','Consultar','Anular','Ajustar','Abrir','Cerrar','Registrar')");
        }
    }
}
