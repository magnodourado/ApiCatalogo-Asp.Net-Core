using ApiCatalogo.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogo.Models
{
    [Table("Produtos")] // Não precisa o EF identifica pelo DbContext, somente para mostrar que pode reforçar aqui.
    public class Produto : IValidatableObject
    {
        [Key] // Não precisa o EF identifica pelo Sufixo ID, somente para mostrar que existe
        public int ProdutoId { get; set; }
        [Required]
        [MaxLength(80)]
        //[PrimeiraLetraMaiuscula] // Validação por atributo (Attribute)
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8, 2)")]
        [Range(1,1000,ErrorMessage = "O preço deve estar entre {1} e {2}")]
        public decimal Preco { get; set; }
        [Required]
        [MaxLength(500)]
        public string ImagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; }

        // Mais uma forma de validação além do attribute
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(this.Nome))
            {
                var primeiraLetra = this.Nome[0].ToString();
                if (primeiraLetra != primeiraLetra.ToUpper())
                {
                    yield return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula",
                        new[]
                        { nameof(this.Nome) }
                        );
                }
            }

            if(this.Estoque <= 0)
            {
                yield return new ValidationResult("O estoque deve ser maior que zero",
                        new[]
                        { nameof(this.Estoque) }
                        );
            }
        }
    }
}
