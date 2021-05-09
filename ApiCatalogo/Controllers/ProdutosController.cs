using ApiCatalogo.DTOs;
using ApiCatalogo.Filters;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository;
using ApiCatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[Controller]")] // Base do endpoint
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof; // Unit of Work
        private readonly IMapper _mapper; // Mapeamento - DTO
        public ProdutosController(IUnitOfWork contexto, IMapper mapper)
        {
            _uof = contexto;
            _mapper = mapper;
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters) // O nome do método não altera o comportamento e sim o decorator [HttpGet]
        {
            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevius
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDTO;

            // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutoPorPreco()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPreço();
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDTO;
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuservico, string nome)
        {
            return meuservico.Saudacao(nome);
        }

        [HttpGet("primeiro")] // Vai atender as duas rotas /api/primeiro e /primeiro
        [HttpGet("/primeiro")] //Barra antes da rota vai fazer a rota ignorar o endpoint principal se tornando apenas https://localhost:44340/primeiro
        public async Task<ActionResult<ProdutoDTO>> Get2([BindRequired] string name) // Tipo de retorno IActionResult / BindRequired obriga o Model Binding a ser valido
        {
            var nome = name;
            var produto = await _uof.ProdutoRepository.GetProdutoPrimeiro();
            if (produto == null)
            {
                return NotFound();
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO); // Retorna um tipo generico de IActionResult OK com o produto no body do result.
        }

        [HttpGet("{id}", Name = "ObterProduto")] // Atributo Name cria uma rota nomeada, que permite que vincule essa rota a uma resposta Http
        public async Task<ActionResult<Produto>> Get(int id) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            //Forçando exception
            //throw new Exception("Exception ao retornar o produto pelo id");
            //Forçando uma exception de referencia nula 
            //string[] teste = null;
            //if(teste.Length > 0)
            //{}

            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 

            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }
            return produto;
        }

        //[HttpGet("{valor:alpha:length(5)}")] valor de a-z maiusculo ou minusculo tamanho = 5, para tamanho maximo seria maxlength
        [HttpGet("id/{id:int:min(1)}")] // int:min(1) restrição de parametro inteiro e minimos 1
        public async Task<ActionResult<ProdutoDTO>> GetId(int id) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return produtoDTO;
        }

        //api/produtos/porid/1/texto
        [HttpGet("porid/{id}/{param2?}")] // Dois parametros e o segundo é opcional sem a interrogação é opcional
        public async Task<ActionResult<ProdutoDTO>> GetporId2(int id, string param2) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var segundoParametro = param2;

            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return produtoDTO;
        }

        //api/produtos/porid/1/texto
        [HttpGet("porid2/{id}/{param2=textoPadrao}")] // Dois parametros e o segundo possui um valor padrao caso venha nulo
        public async Task<ActionResult<ProdutoDTO>> GetporId3(int id, string param2) // Pode retornar um ActionResult ou Produto, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            var segundoParametro = param2;

            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
            {
                return NotFound(); // Status Code 404 
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return produtoDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDTO) // Pega os dados do corpo do requisição e passa para o parâmetro produto, usando o Model Binding
        {
            // A partir da versao 2.1 do Asp.NET Core a validação abaixo ocorre automaticamente desde que se use o atributo [ApiController]. O retorno do BadRequest também é feito automaticamente
            //if (!ModelState.IsValid) // Faz a validadação dos dados enviados do produto enviado no request, ModelState é uma propriedade da classe controller.  
            //{
            //    return BadRequest(ModelState);
            //}
            
            var produto = _mapper.Map<Produto>(produtoDTO);

            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtoDTOCreated = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto",
                new { id = produto.ProdutoId }, produtoDTOCreated);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDTO);

            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id); // FirstOrDefault sempre vai no banco
            //Outra forma de pesquisar
            //var produto = _uof.Produtos.Find(id); // Find procura na memória antes de ir no banco, mas só posso usar se o parametro pesquisado for chave primária na tabela


            if (produto == null)
            {
                return NotFound();
            }

            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return produtoDTO;
        }

    }
}
