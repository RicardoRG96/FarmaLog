using FarmaLog.Nucleo.Domain.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    internal sealed class SolicitudDeIngresoPedidoConfiguration
    : IEntityTypeConfiguration<SolicitudDeIngresoPedido>
    {
        public void Configure(EntityTypeBuilder<SolicitudDeIngresoPedido> builder)
        {
            builder.ToTable("Solicitudes");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedNever();

            builder.Property(s => s.CodigoLaboratorio)
                .HasConversion(v => v.Codigo, v => CodigoLaboratorio.Create(v))
                .HasMaxLength(20).IsRequired();

            builder.Property(s => s.NumeroDelivery)
                .HasConversion(v => v.Numero, v => NumeroDelivery.Create(v))
                .HasMaxLength(50).IsRequired();

            builder.Property(s => s.CuentaCliente)
                .HasConversion(v => v.Cuenta, v => CuentaCliente.Create(v))
                .HasMaxLength(50).IsRequired();

            builder.Property(s => s.DireccionDespacho)
                .HasConversion(v => v.Direccion, v => DireccionDespacho.Create(v))
                .HasMaxLength(50).IsRequired();

            builder.Property(s => s.TipoOrdenVenta)
                .HasConversion(v => v.Tipo, v => TipoOrdenVenta.Create(v))
                .HasMaxLength(20).IsRequired();

            builder.Property(s => s.DocumentoVentaCenabast)
                .HasConversion(v => v!.Documento, v => DocumentoVentaCenabast.Create(v))
                .HasMaxLength(50);

            builder.Property(s => s.EsCenabast).IsRequired();
            builder.Property(s => s.Urgencia).IsRequired();
            builder.Property(s => s.Estado).HasConversion<int>().IsRequired();
            builder.Property(s => s.FechaEntregaSolicitada).IsRequired();
            builder.Property(s => s.Observacion).HasMaxLength(300);
            builder.Property(s => s.OrdenCompra).HasMaxLength(50);

            builder.PrimitiveCollection(s => s.MotivosDeRechazo)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(s => s.Lineas, linea =>
            {
                linea.ToTable("LineasDeSolicitud");
                linea.WithOwner().HasForeignKey("SolicitudId");
                linea.Property(l => l.Sku).HasMaxLength(50).IsRequired();
                linea.Property(l => l.Cantidad).IsRequired();
                linea.Property(l => l.EstadoInventario).HasMaxLength(50).IsRequired();
                linea.Property(l => l.Lote).HasMaxLength(50);
            });

            builder.Navigation(s => s.Lineas).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
