using ISARMIN.Domain.Entities.Inventario;
using Xunit;

namespace ISARMIN.Domain.Tests.Entities.Inventario;

public class CategoriaTests
{
    [Fact]
    public void Constructor_NombreVacio_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() => new Categoria(""));
    }

    [Fact]
    public void ActualizarDatos_MismaIdComoPadre_LanzaExcepcion()
    {
        var categoria = new Categoria("Herramientas");

        Assert.Throws<ArgumentException>(() => categoria.ActualizarDatos("Herramientas", categoria.Id));
    }

    [Fact]
    public void ActualizarDatos_DatosValidos_ActualizaNombreYPadre()
    {
        var padre = new Categoria("Herramientas");
        var categoria = new Categoria("Eléctricas");

        categoria.ActualizarDatos("Herramientas Eléctricas", padre.Id);

        Assert.Equal("Herramientas Eléctricas", categoria.Nombre);
        Assert.Equal(padre.Id, categoria.CategoriaPadreId);
    }
}
