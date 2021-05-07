using ApiCatalogo.Context;
using ApiCatalogo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext contexto) : base (contexto)
        {
        }
        public IEnumerable<Produto> GetProdutosPorPreço()
        {
            return Get().OrderBy(c => c.Preco).ToList();
        }
        public Produto GetProdutoPrimeiro()
        {
            return Get().FirstOrDefault();
        }
    }
}
