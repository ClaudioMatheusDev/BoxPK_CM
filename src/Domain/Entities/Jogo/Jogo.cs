using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Jogo
    {
        [Key]
        public int IDJogoTCG { get; set; }
        public required string Nome { get; set; }
        public string? Fabricante { get; set; }
        public StatusJogo StatusJogo { get; set; }
        public DateTime DataInclusao { get; set; } = DateTime.Now.AddHours(-3);
        public DateTime DataAtualizacao { get; set; }
    }
}
