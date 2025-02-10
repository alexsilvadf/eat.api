using Domain.Interfaces.InterfaceServicos;
using Domain.Interfaces.IUsuarioSistemaFinanceiro;
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
    public class RepositorioUsuarioSistemaFinanceiro : RepositoryGenerics<UsuarioSistemaFinanceiro>, InterfaceUsuarioSistemaFinanceiro
    {
        private readonly DbContextOptions<ContextoBase> _optionsBuilder;

        public RepositorioUsuarioSistemaFinanceiro()
        {
            _optionsBuilder = new DbContextOptions<ContextoBase>();
        }

        public async Task<IList<UsuarioSistemaFinanceiro>> ListarUsuarioSistema(int IdSistema)
        {
            using (var banco = new ContextoBase(_optionsBuilder))
            {
                return await
                    banco.UsuarioSistemaFinanceiro
                    .Where(s => s.IdSistema == IdSistema).AsNoTracking()
                    .ToListAsync();

            }
        }

        public async Task<UsuarioSistemaFinanceiro> ObterUsuarioPorEmail(string emailUsuario)
        {
            using (var banco = new ContextoBase(_optionsBuilder))
            {
                //return await 
                //    banco.UsuarioSistemaFinanceiro
                //    .Where(s => s.Equals(emailUsuario)).FirstOrDefaultAsync();

                return await
                    banco.UsuarioSistemaFinanceiro.AsNoTracking().FirstOrDefaultAsync(x => x.EmailUsuario.Equals(emailUsuario));
            }

        }


        public async Task RemoverUsuarios(List<UsuarioSistemaFinanceiro> usuarios)
        {
            using (var banco = new ContextoBase(_optionsBuilder))
            {
                banco.UsuarioSistemaFinanceiro.RemoveRange(usuarios);

                await banco.SaveChangesAsync();
            }
        }
    }
}

