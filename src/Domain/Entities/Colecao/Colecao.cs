using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Colecao
    {
        [Key]
        public int IDColecao { get; set; }
        public required string Nome { get; set; }
        public string? Sigla { get; set; }
        public int IDJogoTCG { get; set; } 
        public DateTime DataLancamento { get; set; }
        public StatusColecao StatusColecao { get; set; }
        public DateTime DataInclusao { get; set; } = DateTime.Now;
        public DateTime DataAlteracao { get; set; }
    }
}
