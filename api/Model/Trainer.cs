using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Model
{
    public class Trainer : IModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento  { get; set; }
        public string Cidade { get; set; }        
        public string Uf { get; set; }
        public DateTime CreatAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<CapturedPokemon> CapturedPokemons { get; set; }
    }
}
