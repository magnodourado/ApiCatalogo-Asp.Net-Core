using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using System.Collections.Generic;

namespace ApiCatalogo.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        PagedList<Categoria> GetCategorias(CategoriasParameters categoriasParameters);
        IEnumerable<Categoria> GetCategoriasProdutos();
    }
}
