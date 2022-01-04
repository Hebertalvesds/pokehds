using System.Collections.Generic;

namespace api.ApiModel
{
    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<Pokemon> Evolutions { get; set; }
    }
}
