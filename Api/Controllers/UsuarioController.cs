using Application.Common.Interfaces.Entidades.Usuarios;
using Application.Common.Interfaces.Entidades.Usuarios.DTOs;
using Application.Common.Validacoes.Erros;
using Application.Common.Validacoes.ValidacoesUsuario;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/usuarios")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
    }

    [HttpGet("{id:int}", Name = "ObterUsuarioPorId")]
    public async Task<ActionResult<RespostaDadosUsuario>> ObterUsuarioPorId(int id)
    {
        return await _usuarioService.ObterUsuarioPorId(id);
    }

    [HttpPost("cadastrar")]
    public async Task<ActionResult<RespostaUsuario>> Registrar(CriarUsuarioRequest criarUsuarioRequest)
    {
        ValidacaoCadastroUsuario validador = new();
        ValidationResult validationResult = validador.Validate(criarUsuarioRequest);
        if (!validationResult.IsValid)
        {
            var erros = validationResult.Errors
                .Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
            return BadRequest(erros);
        }

        RespostaUsuario usuarioCriado = await _usuarioService.RegisterAsync(criarUsuarioRequest);

        return new CreatedAtRouteResult(nameof(ObterUsuarioPorId), new { id = usuarioCriado.Id }, usuarioCriado);
    }

    [HttpPost("login")]
    public async Task<ActionResult<RespostaUsuario>> Login(LoginRequest loginRequest)
    {
        ValidacaoLoginUsuario validador = new();
        ValidationResult validationResult = validador.Validate(loginRequest);
        if (!validationResult.IsValid)
        {
            var erros = validationResult.Errors
                .Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
            return BadRequest(erros);
        }

        RespostaUsuario usuarioLogado = await _usuarioService.LoginAsync(loginRequest);
        
        return Ok(usuarioLogado);
    }
}