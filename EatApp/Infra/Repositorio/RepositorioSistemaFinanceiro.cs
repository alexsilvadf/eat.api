using Domain.Interfaces.ISistemaFinanceiro;
using Entities.Entidades;
using Infra.Configuracao;
using Infra.Repositorio.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositorio
{
    public class RepositorioSistemaFinanceiro : RepositoryGenerics<SistemaFinanceiro>, InterfaceSistemaFinanceiro
    {
        private readonly DbContextOptions<ContextoBase> _optionsBuilder;

        public RepositorioSistemaFinanceiro()
        {
            _optionsBuilder = new DbContextOptions<ContextoBase>();
        }

        public async Task<bool> ExecuteCopiaDespesasSistemaFinanceiro()
        {
            var listSistemaFinanceiro = new List<SistemaFinanceiro>();

            try
            {
                using (var banco = new ContextoBase(_optionsBuilder))
                {
                    listSistemaFinanceiro = await banco.SistemaFinanceiro.Where(s => s.GerarCopiaDespesa).ToListAsync();
                }

                foreach (var item in listSistemaFinanceiro)
                {
                    using (var banco = new ContextoBase(_optionsBuilder))
                    {
                        var mes = DateTime.Now.Month;
                        var ano = DateTime.Now.Year;

                        var despesaJaExiste = await (from d in banco.Despesa
                                               join c in banco.Categoria
                                               on d.IdCategoria equals c.Id
                                               where c.IdSistema == item.Id
                                               && d.Mes == mes
                                               && d.Ano == ano
                                               select d.Id).AnyAsync();

                        if (!despesaJaExiste)
                        {

                            var despesaSistema = await (from d in banco.Despesa
                                                   join c in banco.Categoria
                                                   on d.IdCategoria equals c.Id
                                                   where c.IdSistema == item.Id
                                                   && d.Mes == item.MesCopia
                                                   && d.Ano == item.AnoCopia
                                                   select d).ToListAsync();

                            despesaSistema.ForEach(d =>
                            {
                                d.DataVencimento = new DateTime(ano, mes, d.DataVencimento.Day);
                                d.Mes = mes;
                                d.Ano = ano;
                                d.DataAlteracao = DateTime.MinValue;
                                d.DataCadastro = DateTime.Now;
                                d.DataPagmento = DateTime.MinValue;
                                d.Pago = false;
                            });

                            if (despesaSistema.Any())
                            {
                                banco.Despesa.AddRange(despesaSistema);
                                banco.SaveChangesAsync();
                            }

                        }

                        
                    }


                }

            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

        public async Task<IList<SistemaFinanceiro>> ListaSistemasUsuario(string emailUsuario)
        {
            using (var banco = new ContextoBase(_optionsBuilder))
            {
                return await
                    (from s in banco.SistemaFinanceiro
                     join us in banco.UsuarioSistemaFinanceiro on s.Id equals us.IdSistema
                     where us.EmailUsuario.Equals(emailUsuario)
                     select s).AsNoTracking().ToListAsync();
            }
        }
    }
}
