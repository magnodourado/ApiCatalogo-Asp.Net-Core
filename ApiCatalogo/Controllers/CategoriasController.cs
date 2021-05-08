using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork contexto, IConfiguration configuration,
            ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uof = contexto;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("autor")]
        public ActionResult<string> GetAutor()
        {
            var autor = _configuration["Autor"];
            var conexão = _configuration["ConnectionStrings:DefaultConnection"];
            return Ok($"Autor: {autor} \n\nConexão: {conexão}");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters) // O nome do método não altera o comportamento e sim o decorator [HttpGet]
        {
            _logger.LogInformation("#################### GET api/categorias ###########################");

            try
            {
                var categoria = await _uof.CategoriaRepository.
                        GetCategorias(categoriasParameters); // AsNoTracking - usado para otimizar consultas quando não vai alterar o retorno, desabilita o rastreamento de estado do EF. 

                var metadata = new
                {
                    categoria.TotalCount,
                    categoria.PageSize,
                    categoria.CurrentPage,
                    categoria.TotalPages,
                    categoria.HasNext,
                    categoria.HasPrevius
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);

                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao tentar obter as categorias do banco de dados");
            }
        }

        [HttpGet("produtos")] // Vai compor a rota padrao Ex: api/categorias/produtos
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            _logger.LogInformation("#################### GET api/categorias/produtos ###########################");

            var categoria = await _uof.CategoriaRepository.GetCategoriasProdutos();

            var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);

            return categoriaDTO;
        }

        [HttpGet("{id}", Name = "ObterCategoria")] // Atributo Name cria uma rota nomeada, que permite que vincule essa rota a uma resposta Http
        public async Task<ActionResult<CategoriaDTO>> Get(int id) // Pode retornar um ActionResult ou Categoria, ActionResult são por exemplo os códigos Http (200 = OK, 404 = Not Found)
        {
            _logger.LogInformation($"#################### GET api/categorias/produtos/id = {id} ###########################");
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
                if (categoria == null)
                {
                    _logger.LogInformation($"#################### GET api/categorias/produtos/id = {id} NOT FOUND ###########################");
                    return NotFound($"A categoria com id={id} não foi encontrada"); // Status Code 404 
                }

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Erro ao tentar obter as categorias do banco de dados");
            }

        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post([FromBody] CategoriaDTO categoriaDTO) // Pega os dados do corpo do requisição e passa para o parâmetro Categoria, usando o Model Binding
        {
            // A partir da versao 2.1 do Asp.NET Core a validação abaixo ocorre automaticamente desde que se use o atributo [ApiController]. O retorno do BadRequest também é feito automaticamente
            //if (!ModelState.IsValid) // Faz a validadação dos dados enviados do Categoria enviado no request, ModelState é uma propriedade da classe controller.  
            //{
            //    return BadRequest(ModelState);
            //}
            var categoria = _mapper.Map<Categoria>(categoriaDTO);
            try
            {
                _uof.CategoriaRepository.Add(categoria);
                await _uof.Commit();

                // Tem que fazer assim para retorna o criado com id, se retornar direto, categoriaDTO não possui ID ainda 
                var categoriaDTOCreated = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoria.CategoriaId }, categoriaDTOCreated);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Erro ao tentar criar uma nova categoria");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDTO)
        {
            try
            {
                if (id != categoriaDTO.CategoriaId)
                {
                    return BadRequest($"Não foi possível alterar a categoria com o id={id}.");
                }

                var categoria = _mapper.Map<Categoria>(categoriaDTO);

                _uof.CategoriaRepository.Update(categoria);
                await _uof.Commit();
                return Ok($"Categoria com id={id} foi atualizada com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   $"Erro ao tentar atualizar a categoria com id={id}.");
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id); // FirstOrDefault sempre vai no banco
                //Outra forma de pesquisar
                //var categoria = _uof.Categorias.Find(id); // Find procura na memória antes de ir no banco, mas só posso usar se o parametro pesquisado for chave primária na tabela

                if (categoria == null)
                {
                    return NotFound($"A categoria com o id={id} não foi encontrada.");
                }

                _uof.CategoriaRepository.Delete(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                return categoriaDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   $"Erro ao excluir a cetegoria de id={id}");
            }

        }

    }
}
