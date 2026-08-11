using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Categoria
    {
        [Key]
        public int IDCategoria { get; set; }
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public StatusCategoria StatusCategoria { get; set; }
        public DateTime DataInclusao {get; set;} = DateTime.Now;
        public DateTime DataAlteracao {get; set;}
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
