namespace ISARMIN.Application.Common;

/// <summary>Genera valores aleatorios de alta entropía usados como refresh token.</summary>
public interface IGeneradorTokenOpaco
{
    string Generar();
}
