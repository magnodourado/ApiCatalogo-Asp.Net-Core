using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoriasController(AppDbContext contexto)
        {
            _context = contexto;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get() // O nome do método não altera o comportamento e sim o decorator [HttpGet]
        {
            try
            {
                return _context.Categorias.AsNoTracking().ToList(); // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("produtos")] // Vai compor a rota padrao Ex: api/categorias/produtos
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            return _context.Categorias.Include(x => x.Produtos).ToList();
        }

        [HttpGet("{id}", Name = "ObterCategoria")] // Atributo Name cria uma rota nomeada, que permite que vincule essa rota a uma resposta Http
        public ActionResult<Categoria> Get(int id) // Pode retornar um ActionResult ou Categoria, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            try
            {
                var Categoria = _context.Categorias.AsNoTracking(). // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
                FirstOrDefault(p => p.CategoriaId == id);
                if (Categoria == null)
                {
                    return NotFound($"A categoria com id={id} não foi encontrada"); // Status Code 404 
                }
                return Categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Erro ao tentar obter as categorias do banco de dados");
            }
            
        }

        [HttpPost]
        public ActionResult Post([FromBody] Categoria categoria) // Pega os dados do corpo do requisição e passa para o parâmetro Categoria, usando o Model Binding
        {
            // A partir da versao 2.1 do Asp.NET Core a validação abaixo ocorre automaticamente desde que se use o atributo [ApiController]. O retorno do BadRequest também é feito automaticamente
            //if (!ModelState.IsValid) // Faz a validadação dos dados enviados do Categoria enviado no request, ModelState é uma propriedade da classe controller.  
            //{
            //    return BadRequest(ModelState);
            //}
            try
            {
                _context.Categorias.Add(categoria);
                _context.SaveChanges();

                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoria.CategoriaId }, categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Erro ao tentar criar uma nova categoria");
            }
            

        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.CategoriaId)
                {
                    return BadRequest($"Não foi possível alterar a categoria com o id={id}.");
                }

                _context.Entry(categoria).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok($"Categoria com id={id} foi atualizada com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   $"Erro ao tentar atualizar a categoria com id={id}.");
            }
            
        }

        [HttpDelete("{id}")]
        public ActionResult<Categoria> Delete(int id)
        {
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id); // FirstOrDefault sempre vai no banco
                //Outra forma de pesquisar
                //var categoria = _context.Categorias.Find(id); // Find procura na memória antes de ir no banco, mas só posso usar se o parametro pesquisado for chave primária na tabela

                if (categoria == null)
                {
                    return NotFound($"A categoria com o id={id} não foi encontrada.");
                }

                _context.Categorias.Remove(categoria);
                _context.SaveChanges();
                return categoria;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   $"Erro ao excluir a cetegoria de id={id}");
            }
            
        }

    }
}
