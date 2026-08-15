using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FarmaLog.Nucleo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InicialNucleo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoLaboratorio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CuentaCliente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DireccionDespacho = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoOrdenVenta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EsCenabast = table.Column<bool>(type: "bit", nullable: false),
                    DocumentoVentaCenabast = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    FechaEntregaSolicitada = table.Column<DateOnly>(type: "date", nullable: false),
                    NumeroDelivery = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrdenCompra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Urgencia = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    MotivosDeRechazo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineasDeSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    EstadoInventario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasDeSolicitud", x => new { x.SolicitudId, x.Id });
                    table.ForeignKey(
                        name: "FK_LineasDeSolicitud_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_Solicitudes_Laboratorio_Delivery",
                table: "Solicitudes",
                columns: new[] { "CodigoLaboratorio", "NumeroDelivery" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasDeSolicitud");

            migrationBuilder.DropTable(
                name: "Solicitudes");
        }
    }
}
