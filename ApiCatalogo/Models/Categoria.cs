using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Models
{
    [Table("Categorias")] // Não precisa o EF identifica pelo DbContext, somente para mostrar que pode reforçar aqui.
    public class Categoria
    {
        public Categoria() // Não precisa mas é considerado boa prática
        {
            Produtos = new Collection<Produto>();
        }
        [Key] // Não precisa o EF identifica pelo Sufixo ID, somente para mostrar que existe
        public int CategoriaId { get; set; }

        
        [Required]
        [MaxLength(80)] // Determina o tamanho do campo na tabela (EF core)
        public string Nome { get; set; }
        [Required(ErrorMessage = "Informe o nome")]
        [MaxLength(300)]
        // [BindNever] informa ao Model Binding para não vincular a informação ao parâmetro
        public string ImagemUrl { get; set; }
        public ICollection<Produto> Produtos { get; set; }

        //[StringLength(10, MinimumLength =4)] // Tamanho mínimo e máximo permitido
        //[RegularExpression(".+\\@.+\\..+", ErrorMessage ="Informe um email válido")]
        //[Range(18,65)],[CreditCard],[Phone]

        //Permite comparar duas propriedades
        //[Compare("Senha")]  Comparar a propriedade ConfirmaSenha com a propriedade Senha
        //public string ConfirmaSenha { get; set; }

        // IMPORTANTE: Uma alternativa a esse tipo de validação seria utiizar Fluent API
        /// http://www.macoratti.net/15/03/ef_fluent1.htm

    }
}
