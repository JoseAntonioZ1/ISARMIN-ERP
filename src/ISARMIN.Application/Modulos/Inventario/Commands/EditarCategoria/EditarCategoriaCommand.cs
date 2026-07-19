namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarCategoria;

public record EditarCategoriaCommand(Guid Id, string Nombre, Guid? CategoriaPadreId);
