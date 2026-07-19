namespace ISARMIN.Application.Common;

/// <summary>Resultado para Commands que no necesitan devolver datos.</summary>
public readonly struct Unit
{
    public static readonly Unit Value = default;
}
