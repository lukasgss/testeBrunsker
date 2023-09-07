using Application.Common.Interfaces.Autorizacao;
using Application.Common.Interfaces.Entidades.Locacoes;
using Application.Common.Interfaces.Entidades.Locacoes.DTOs;
using Application.Common.Validacoes.Erros;
using Application.Common.Validacoes.ValidacoesLocacao;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/locacoes")]
public class LocacaoController : ControllerBase
{
    private readonly ILocacaoService _locacaoService;
    private readonly IUsuarioAuthService _usuarioAuthService;

    public LocacaoController(ILocacaoService locacaoService, IUsuarioAuthService usuarioAuthService)
    {
        _locacaoService = locacaoService ?? throw new ArgumentNullException(nameof(locacaoService));
        _usuarioAuthService = usuarioAuthService ?? throw new ArgumentNullException(nameof(usuarioAuthService));
    }

    [HttpGet("{idLocacao:int}", Name = "ObterLocacaoPorId")]
    public async Task<ActionResult<RespostaLocacao>> ObterLocacaoPorId(int idLocacao)
    {
        return await _locacaoService.ObterPorIdAsync(idLocacao);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RespostaLocacao>> Cadastrar(CriarLocacaoRequest criarLocacaoRequest)
    {
        ValidacaoCadastroLocacao validador = new();
        ValidationResult validationResult = validador.Validate(criarLocacaoRequest);
        if (!validationResult.IsValid)
        {
            var erros = validationResult.Errors.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
            return BadRequest(erros);
        }

        int idUsuario = _usuarioAuthService.ObterIdPorTokenJwt(User);

        RespostaLocacao locacaoCadastrada = await _locacaoService.CadastrarAsync(criarLocacaoRequest, idUsuario);
        return new CreatedAtRouteResult(
            nameof(ObterLocacaoPorId),
            new { idLocacao = locacaoCadastrada.Id },
            locacaoCadastrada);
    }

    [Authorize]
    [HttpPut("{idLocacao:int}")]
    public async Task<ActionResult<RespostaLocacao>> Editar(EditarLocacaoRequest editarLocacaoRequest, int idLocacao)
    {
        ValidacaoEditarLocacao validador = new();
        ValidationResult validationResult = validador.Validate(editarLocacaoRequest);
        if (!validationResult.IsValid)
        {
            var erros = validationResult.Errors.Select(e => new ErroValidacao(e.PropertyName, e.ErrorMessage));
            return BadRequest(erros);
        }
        
        int idUsuario = _usuarioAuthService.ObterIdPorTokenJwt(User);

        return await _locacaoService.EditarAsync(editarLocacaoRequest, idUsuario, idLocacao);
    }

    [Authorize]
    [HttpDelete("{idLocacao:int}")]
    public async Task<ActionResult> Excluir(int idLocacao)
    {
        int idUsuario = _usuarioAuthService.ObterIdPorTokenJwt(User);

        await _locacaoService.DeletarAsync(idLocacao, idUsuario);
        return Ok();
    }
}