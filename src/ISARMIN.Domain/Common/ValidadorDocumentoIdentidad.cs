using System.Text.RegularExpressions;
using ISARMIN.Domain.Enums;

namespace ISARMIN.Domain.Common;

/// <summary>
/// RN-033 — validación de formato de documentos de identidad peruanos. Norma externa
/// (RENIEC/SUNAT), no una regla de negocio de ISARMIN.
/// </summary>
public static class ValidadorDocumentoIdentidad
{
    private static readonly int[] FactoresRuc = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];

    public static bool EsValido(TipoDocumento tipo, string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
        {
            return false;
        }

        return tipo switch
        {
            TipoDocumento.Dni => Regex.IsMatch(numero, "^[0-9]{8}$"),
            TipoDocumento.Ruc => EsRucValido(numero),
            TipoDocumento.CarneExtranjeria or TipoDocumento.Pasaporte => Regex.IsMatch(numero, "^[a-zA-Z0-9]{5,20}$"),
            _ => false,
        };
    }

    /// <summary>Algoritmo oficial de dígito verificador de RUC (módulo 11, SUNAT).</summary>
    private static bool EsRucValido(string numero)
    {
        if (!Regex.IsMatch(numero, "^[0-9]{11}$"))
        {
            return false;
        }

        var suma = 0;
        for (var i = 0; i < 10; i++)
        {
            suma += (numero[i] - '0') * FactoresRuc[i];
        }

        var resto = suma % 11;
        var digitoCalculado = 11 - resto;
        digitoCalculado = digitoCalculado switch
        {
            10 => 0,
            11 => 1,
            _ => digitoCalculado,
        };

        return digitoCalculado == numero[10] - '0';
    }
}
