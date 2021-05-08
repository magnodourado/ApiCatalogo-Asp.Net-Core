using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
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
        public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters)
        {
            return Get()
                .OrderBy(on => on.Nome)
                .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
                .Take(produtosParameters.PageSize)
                .ToList();
        }
    }
}
