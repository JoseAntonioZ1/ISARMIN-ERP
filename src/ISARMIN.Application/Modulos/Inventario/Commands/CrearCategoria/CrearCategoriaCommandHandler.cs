using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;
using ISARMIN.Domain.Entities.Inventario;

namespace ISARMIN.Application.Modulos.Inventario.Commands.CrearCategoria;

public class CrearCategoriaCommandHandler : ICommandHandler<CrearCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IValidator<CrearCategoriaCommand> _validator;

    public CrearCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IValidator<CrearCategoriaCommand> validator)
    {
        _categoriaRepository = categoriaRepository;
        _validator = validator;
    }

    public async Task<CategoriaDto> ManejarAsync(CrearCategoriaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var existente = await _categoriaRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
        if (existente is not null)
        {
            throw new NombreCategoriaDuplicadoException(comando.Nombre);
        }

        if (comando.CategoriaPadreId is { } padreId && !await _categoriaRepository.ExisteAsync(padreId, cancellationToken))
        {
            throw new CategoriaPadreInvalidaException(padreId);
        }

        var categoria = new Categoria(comando.Nombre, comando.CategoriaPadreId);
        _categoriaRepository.Agregar(categoria);
        await _categoriaRepository.GuardarCambiosAsync(cancellationToken);

        return categoria.ADto();
    }
}
