using Application.Common.Interfaces.Autorizacao;
using Application.Common.Interfaces.Entidades.Imoveis;
using Application.Common.Interfaces.Entidades.Imoveis.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/imoveis")]
public class ImovelController : ControllerBase
{
    private readonly IImovelService _imovelService;
    private readonly IUsuarioAuthService _usuarioAuthService;

    public ImovelController(IImovelService imovelService, IUsuarioAuthService usuarioAuthService)
    {
        _imovelService = imovelService ?? throw new ArgumentNullException(nameof(imovelService));
        _usuarioAuthService = usuarioAuthService ?? throw new ArgumentNullException(nameof(usuarioAuthService));
    }

    [HttpGet("{imovelId:int}", Name = "ObterImovelPorId")]
    public async Task<ActionResult<RespostaImovel>> ObterImovelPorId(int imovelId)
    {
        return await _imovelService.ObterPorIdAsync(imovelId);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RespostaImovel>> Cadastrar(CriarImovelRequest criarImovelRequest)
    {
        int idUsuario = _usuarioAuthService.ObterIdPorTokenJwt(User);

        RespostaImovel imovelCadastrado = await _imovelService.CadastrarAsync(criarImovelRequest, idUsuario);

        return new CreatedAtRouteResult(
            nameof(ObterImovelPorId),
            new { imovelId = imovelCadastrado.Id },
            imovelCadastrado);
    }

    [Authorize]
    [HttpPut("{imovelId:int}")]
    public async Task<ActionResult<RespostaImovel>> Editar(EditarImovelRequest editarImovelRequest, int imovelId)
    {
        int idUsuario = _usuarioAuthService.ObterIdPorTokenJwt(User);

        return await _imovelService.EditarAsync(editarImovelRequest, idUsuario, idRota: imovelId);
    }

    [Authorize]
    [HttpDelete("{imovelId:int}")]
    public async Task<ActionResult> Excluir(int imovelId)
    {
        int idUsuario = _usuarioAuthService.ObterIdPorTokenJwt(User);

        await _imovelService.DeletarAsync(idUsuario, imovelId);
        return Ok();
    }
}