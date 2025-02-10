using Domain.Interfaces.ICategoria;
using Domain.Interfaces.IDespesa;
using Domain.Interfaces.InterfaceServicos;
using Domain.Servicos;
using Entities.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DespesasController : ControllerBase
    {
        private readonly InterfaceDespesa _interfaceDespesa;
        private readonly IDespesaServico _despesaServico;

        public DespesasController(InterfaceDespesa interfaceDespesa, IDespesaServico despesaServico)
        {
            _interfaceDespesa = interfaceDespesa;
            _despesaServico = despesaServico;
        }


        [HttpGet("/api/ListarDespesaUsuario")]
        [Produces("application/json")]
        public async Task<object> ListarDespesaUsuario(string emailUsuario)
        {
            return await _interfaceDespesa.ListarDespesaUsuario(emailUsuario);
        }

        [HttpPost("/api/AdicionarCategoria")]
        [Produces("application/json")]
        public async Task<object> AdicionarCategoria(Despesa despesa)
        {
            await _despesaServico.AdicionarDespesa(despesa);

            return despesa;
        }

        [HttpPut("/api/AtualizarDespesa")]
        [Produces("application/json")]
        public async Task<object> AtualizarDespesa(Despesa despesa)
        {
            await _despesaServico.AtualizarDespesa(despesa);

            return despesa;
        }

        [HttpGet("/api/ObterDespesa")]
        [Produces("application/json")]
        public async Task<object> ObterDespesa(int id)
        {
            return await _interfaceDespesa.GetEntityById(id);

        }

        [HttpDelete("/api/DeleteDespesa")]
        [Produces("application/json")]
        public async Task<object> DeleteDespesa(int id)
        {
            try
            {
                var despesa = await _interfaceDespesa.GetEntityById(id);

                await _interfaceDespesa.Delete(despesa);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }

        [HttpGet("/api/CarregaGraficos")]
        [Produces("application/json")]
        public async Task<object> CarregaGraficos(string emailUsuario)
        {
            return await _despesaServico.CarregaGraficos(emailUsuario);

        }
    }



}
