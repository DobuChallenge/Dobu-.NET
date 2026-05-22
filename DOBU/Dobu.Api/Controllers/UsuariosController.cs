using Dobu.Domain.Entities;
using Dobu.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dobu.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController(DobuDbContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Usuario>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetAll()
    {
        return Ok(await context.Usuarios.AsNoTracking().ToListAsync());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Usuario>> GetById(Guid id)
    {
        var usuario = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpGet("tipo/{tipoUsuario}")]
    [ProducesResponseType(typeof(IEnumerable<Usuario>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetByTipo(string tipoUsuario)
    {
        return Ok(await context.Usuarios.AsNoTracking()
            .Where(x => x.TipoUsuario == tipoUsuario.ToUpper())
            .ToListAsync());
    }

    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Usuario>> GetByEmail(string email)
    {
        var usuario = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Usuario>> Create(UsuarioRequest request)
    {
        if (await context.Usuarios.CountAsync(x => x.Email == request.Email) > 0)
            return BadRequest("Ja existe usuario cadastrado com esse email.");

        try
        {
            var usuario = new Usuario(request.Nome, request.Email, request.Senha, request.TipoUsuario.ToUpper());
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Usuario), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Usuario>> Update(Guid id, UsuarioRequest request)
    {
        var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (usuario is null)
            return NotFound();

        try
        {
            usuario.Atualizar(request.Nome, request.Email, request.Senha, request.TipoUsuario.ToUpper());
            await context.SaveChangesAsync();
            return Ok(usuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var usuario = await context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        if (usuario is null)
            return NotFound();

        context.Usuarios.Remove(usuario);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record UsuarioRequest(string Nome, string Email, string Senha, string TipoUsuario);
