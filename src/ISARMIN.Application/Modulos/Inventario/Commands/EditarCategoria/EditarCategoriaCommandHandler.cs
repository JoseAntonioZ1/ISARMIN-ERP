using FluentValidation;
using ISARMIN.Application.Common;
using ISARMIN.Application.Common.Excepciones;
using ISARMIN.Application.Modulos.Inventario.DTOs;

namespace ISARMIN.Application.Modulos.Inventario.Commands.EditarCategoria;

public class EditarCategoriaCommandHandler : ICommandHandler<EditarCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IValidator<EditarCategoriaCommand> _validator;

    public EditarCategoriaCommandHandler(ICategoriaRepository categoriaRepository, IValidator<EditarCategoriaCommand> validator)
    {
        _categoriaRepository = categoriaRepository;
        _validator = validator;
    }

    public async Task<CategoriaDto> ManejarAsync(EditarCategoriaCommand comando, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(comando, cancellationToken);

        var categoria = await _categoriaRepository.ObtenerPorIdAsync(comando.Id, cancellationToken)
            ?? throw new CategoriaNoEncontradaException(comando.Id);

        if (!string.Equals(categoria.Nombre, comando.Nombre, StringComparison.Ordinal))
        {
            var existente = await _categoriaRepository.ObtenerPorNombreAsync(comando.Nombre, cancellationToken);
            if (existente is not null)
            {
                throw new NombreCategoriaDuplicadoException(comando.Nombre);
            }
        }

        if (comando.CategoriaPadreId is { } padreId && !await _categoriaRepository.ExisteAsync(padreId, cancellationToken))
        {
            throw new CategoriaPadreInvalidaException(padreId);
        }

        categoria.ActualizarDatos(comando.Nombre, comando.CategoriaPadreId);
        await _categoriaRepository.GuardarCambiosAsync(cancellationToken);

        return categoria.ADto();
    }
}
