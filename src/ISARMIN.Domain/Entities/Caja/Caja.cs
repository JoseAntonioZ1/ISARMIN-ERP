using ISARMIN.Domain.Common;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Entities.Caja;

/// <summary>UC-19 — Abrir/Cerrar Caja (RF-046 a RF-049). RN-025: existe una única caja para todo el
/// negocio — solo puede haber una instancia con <see cref="Estado"/> Abierta a la vez (invariante
/// aplicado en <c>AbrirCajaCommandHandler</c>, ya que requiere consultar el repositorio).</summary>
public class Caja : Entity
{
    public DateTime FechaApertura { get; private set; }
    public decimal MontoApertura { get; private set; }
    public DateTime? FechaCierre { get; private set; }
    public decimal? MontoTeoricoCierre { get; private set; }
    public decimal? MontoFisicoDeclarado { get; private set; }
    public Guid UsuarioId { get; private set; }
    public EstadoCaja Estado { get; private set; }

    public bool EstaAbierta => Estado == EstadoCaja.Abierta;

    /// <summary>Descuadre de caja: positivo = sobrante, negativo = faltante. Solo tiene sentido tras el cierre.</summary>
    public decimal? Diferencia => MontoFisicoDeclarado is { } fisico && MontoTeoricoCierre is { } teorico ? fisico - teorico : null;

    private Caja() { }

    public Caja(decimal montoApertura, Guid usuarioId, DateTime fechaApertura)
    {
        if (montoApertura < 0)
        {
            throw new ArgumentException("El monto de apertura no puede ser negativo.", nameof(montoApertura));
        }

        MontoApertura = montoApertura;
        UsuarioId = usuarioId;
        FechaApertura = fechaApertura;
        Estado = EstadoCaja.Abierta;
    }

    /// <summary>RF-048/RN-015 — concilia el monto teórico (calculado por el sistema) contra el físico
    /// declarado por el responsable. El mecanismo de resolución de un descuadre sigue sin definirse
    /// (BQ-031); este método solo registra ambos montos y deja calculada la diferencia.</summary>
    public void Cerrar(decimal montoTeoricoCierre, decimal montoFisicoDeclarado, DateTime fechaCierre)
    {
        if (!EstaAbierta)
        {
            throw new InvalidOperationException("La caja ya está cerrada.");
        }

        if (montoFisicoDeclarado < 0)
        {
            throw new ArgumentException("El monto físico declarado no puede ser negativo.", nameof(montoFisicoDeclarado));
        }

        MontoTeoricoCierre = montoTeoricoCierre;
        MontoFisicoDeclarado = montoFisicoDeclarado;
        FechaCierre = fechaCierre;
        Estado = EstadoCaja.Cerrada;
    }
}
