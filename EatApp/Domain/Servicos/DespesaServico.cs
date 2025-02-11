using Domain.Interfaces.Generics;
using Domain.Interfaces.IDespesa;
using Domain.Interfaces.InterfaceServicos;
using Entities.Entidades;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Servicos
{
    public class DespesaServico : IDespesaServico
    {
        private readonly InterfaceDespesa _interfaceDespesa;

        public DespesaServico(InterfaceDespesa despesaServico)
        {
            _interfaceDespesa = despesaServico;
        }

        public Task Add(Despesa Objeto)
        {
            throw new NotImplementedException();
        }

        public async Task AdicionarDespesa(Despesa despesa)
        {
            var data = DateTime.UtcNow;

            despesa.DataCadastro = data;
            despesa.Ano = data.Year;
            despesa.Mes = data.Month;

            var valido = despesa.ValidarPropriedadeString(despesa.Nome, "Nome");

            if (valido)
            {
                await _interfaceDespesa.Add(despesa);
            }
        }

        public async Task AtualizarDespesa(Despesa despesa)
        {
            var data = DateTime.UtcNow;

            despesa.DataAlteracao = data;

            if (despesa.Pago)
            {
                despesa.DataPagmento = data;
            }

            var valido = despesa.ValidarPropriedadeString(despesa.Nome, "Nome");

            if (valido)
            {
                await _interfaceDespesa.Update(despesa);
            }
        }

        public async Task<object> CarregaGraficos(string emailUsuario)
        {
            var despesaUsuario = await _interfaceDespesa.ListarDespesaUsuario(emailUsuario);

            var despesasAnteriores = await _interfaceDespesa.ListarDespesaUsuarioNaoPagasMesesAnterior(emailUsuario);

            var despesas_naoPagasMesesAnteriores = despesasAnteriores.Any() ?
                despesasAnteriores.ToList().Sum(x => x.Valor) : 0;

            var despesasPagas = despesaUsuario.Where(d => d.Pago && d.TipoDespesa == EnumTipoDespesa.Contas)
                .Sum(x => x.Valor);

            var despesasPendentes = despesaUsuario.Where(d => !d.Pago && d.TipoDespesa == EnumTipoDespesa.Contas)
                .Sum(x => x.Valor);

            var investimentos = despesaUsuario.Where(d => d.TipoDespesa == EnumTipoDespesa.Investimento)
              .Sum(x => x.Valor);

            return new
            {
                sucesso = "OK",
                despesas_pagas = despesasPagas,
                despesas_pendentes = despesasPendentes,
                despesas_naoPagasMesesAnteriores = despesas_naoPagasMesesAnteriores,
                investimentos = investimentos
            };
        }
    }
}
