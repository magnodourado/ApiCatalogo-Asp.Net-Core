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
        [MaxLength(80)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(300)]
        public string ImagemUrl { get; set; }
        public ICollection<Produto> Produtos { get; set; }
    }
}
