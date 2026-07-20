namespace ISARMIN.Domain.Enums;

/// <summary>CAT-004 en Functional-Requirements.md/Physical-Data-Model.md es una referencia cruzada
/// equivocada (ese catálogo son Tipos de Servicio de Campo) — se implementa como un CHECK cerrado,
/// no como catálogo editable, igual que ya está declarado en el modelo físico.</summary>
public enum TipoComprobante
{
    Cotizacion,
    Boleta,
    Factura,
    NotaVenta,
    Ticket
}
