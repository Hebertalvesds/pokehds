using System.ComponentModel.DataAnnotations;

namespace api.Model
{
    public class CapturedPokemon : IModel
    {
        [Key]
        public int Id { get; set; }
        public int PokemonId { get; set; }        
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
    }
}
