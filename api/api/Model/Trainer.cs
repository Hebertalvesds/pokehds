using System;
using System.ComponentModel.DataAnnotations;

namespace api.Model
{
    public class Trainer : IModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string CPF { get; set; }
        [Required]
        public DateTime DataNascimento  { get; set; }
        public string Cidade { get; set; }        
        public string Uf { get; set; }
        public DateTime CreatAt { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
