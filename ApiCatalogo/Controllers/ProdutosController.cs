using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Route("api/[Controller]")] // Base do endpoint
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProdutosController(AppDbContext contexto)
        {
            _context = contexto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAsync() // O nome do método não altera o comportamento e sim o decorator [HttpGet]
        {
            return await _context.Produtos.AsNoTracking().ToListAsync(); // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
        }

        [HttpGet("primeiro")] // Vai atender as duas rotas /api/primeiro e /primeiro
        [HttpGet("/primeiro")] //Barra antes da rota vai fazer a rota ignorar o endpoint principal se tornando apenas https://localhost:44340/primeiro
        public IActionResult Get2() // Tipo de retorno IActionResult
        {
            var produto = _context.Produtos.FirstOrDefault();
            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto); // Retorna um tipo generico de IActionResult OK com o produto no body do result.
        }

        [HttpGet("{id}", Name ="ObterProduto")] // Atributo Name cria uma rota nomeada, que permite que vincule essa rota a uma resposta Http
        public ActionResult<Produto> Get(int id) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var produto = _context.Produtos.AsNoTracking(). // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
                FirstOrDefault(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound(); // Status Code 404 
            }
            return produto;
        }

        //[HttpGet("{valor:alpha:length(5)}")] valor de a-z maiusculo ou minusculo tamanho = 5, para tamanho maximo seria maxlength
        [HttpGet("id/{id:int:min(1)}")] // int:min(1) restrição de parametro inteiro e minimos 1
        public ActionResult<Produto> GetId(int id) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var produto = _context.Produtos.AsNoTracking(). // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
                FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }
            return produto;
        }

        //api/produtos/porid/1/texto
        [HttpGet("porid/{id}/{param2?}")] // Dois parametros e o segundo é opcional sem a interrogação é opcional
        public ActionResult<Produto> GetporId2(int id, string param2) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var segundoParametro = param2;

            var produto = _context.Produtos.AsNoTracking(). // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
                FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }
            return produto;
        }

        //api/produtos/porid/1/texto
        [HttpGet("porid2/{id}/{param2=textoPadrao}")] // Dois parametros e o segundo possui um valor padrao caso venha nulo
        public ActionResult<Produto> GetporId3(int id, string param2) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var segundoParametro = param2;

            var produto = _context.Produtos.AsNoTracking(). // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
                FirstOrDefault(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }
            return produto;
        }

        [HttpPost]
        public ActionResult Post([FromBody]Produto produto) // Pega os dados do corpo do requisição e passa para o parâmetro produto, usando o Model Binding
        {
            // A partir da versao 2.1 do Asp.NET Core a validação abaixo ocorre automaticamente desde que se use o atributo [ApiController]. O retorno do BadRequest também é feito automaticamente
            //if (!ModelState.IsValid) // Faz a validadação dos dados enviados do produto enviado no request, ModelState é uma propriedade da classe controller.  
            //{
            //    return BadRequest(ModelState);
            //}

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produto);

        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Produto produto)
        {
            if(id != produto.ProdutoId)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult<Produto> Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id); // FirstOrDefault sempre vai no banco
            //Outra forma de pesquisar
            //var produto = _context.Produtos.Find(id); // Find procura na memória antes de ir no banco, mas só posso usar se o parametro pesquisado for chave primária na tabela


            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return produto;
        }

    }
}
