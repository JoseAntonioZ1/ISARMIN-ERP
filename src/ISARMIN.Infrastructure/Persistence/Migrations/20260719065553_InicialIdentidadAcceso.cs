using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ISARMIN.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InicialIdentidadAcceso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    nombre_usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    credencial_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    intentos_fallidos = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    bloqueado_hasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Activo"),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                    table.CheckConstraint("ck_usuarios_estado", "estado IN ('Activo','Inactivo')");
                });

            migrationBuilder.CreateTable(
                name: "permisos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rol_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modulo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    accion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permisos", x => x.id);
                    table.CheckConstraint("ck_permisos_accion", "accion IN ('Crear','Editar','Eliminar','Consultar','Anular')");
                    table.ForeignKey(
                        name: "fk_permisos_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_rol",
                columns: table => new
                {
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rol_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_rol", x => new { x.usuario_id, x.rol_id });
                    table.ForeignKey(
                        name: "fk_usuario_rol_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuario_rol_usuarios_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "descripcion", "nombre" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Acceso completo — Propietario.", "Administrador" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Atención al cliente, ventas, caja, consulta de inventario.", "Ventas" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Taller, diagnósticos, órdenes de trabajo, servicios de campo.", "Técnico" }
                });

            migrationBuilder.InsertData(
                table: "usuarios",
                columns: new[] { "id", "bloqueado_hasta", "credencial_hash", "fecha_creacion", "nombre", "nombre_usuario" },
                values: new object[] { new Guid("00000000-0000-0000-0000-0000000000a1"), null, "AQAAAAIAAYagAAAAEFF/mQcjo7I1pcwzMaEjr6h0JjtxjfNsg4QMpRJbGRHzyMbbydhPHhidcXlHS51ubQ==", new DateTime(2026, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc), "Administrador ISARMIN", "admin" });

            migrationBuilder.InsertData(
                table: "usuario_rol",
                columns: new[] { "rol_id", "usuario_id" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-0000000000a1") });

            migrationBuilder.CreateIndex(
                name: "ix_permisos_rol_id_modulo_accion",
                table: "permisos",
                columns: new[] { "rol_id", "modulo", "accion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_roles_nombre",
                table: "roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuario_rol_rol_id",
                table: "usuario_rol",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "ix_usuarios_nombre_usuario",
                table: "usuarios",
                column: "nombre_usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permisos");

            migrationBuilder.DropTable(
                name: "usuario_rol");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
