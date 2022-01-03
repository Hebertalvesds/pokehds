using api.ApiModel;
using api.Extensions;
using api.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("pokeindex")]
    public class PokeIndexController : BaseController
    {
        public PokeIndexController(Endpoint endpoint) : base(endpoint) { }

        [HttpGet]
        //GET: pokemon/1
        [Route("pokemon/{pokemonId}")]
        public JsonResult GetPokemon(int pokemonId)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Pokemon, pokemonId);
            var response = GetUrl(uri).Result;
            Pokemon pokemon = null;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                pokemon = JsonConvert.DeserializeObject<Pokemon>(json);
                pokemon.Evolutions = GetEvolutions(pokemon);
            }

            return Json(pokemon);

        }

        [HttpGet]
        //GET: pokemon/bulbasaur
        [Route("pokemon/name")]
        public JsonResult GetPokemon(string pokemonName)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Pokemon, pokemonName);
            var response = GetUrl(uri).Result;
            Pokemon pokemon = null;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                pokemon = JsonConvert.DeserializeObject<Pokemon>(json);
                pokemon.Evolutions = GetEvolutions(pokemon);
            }

            return Json(pokemon);

        }

        [HttpGet]
        //pokemon/list/10
        [Route("pokemon/list/{limit}")]
        public JsonResult GetList(int limit = 10)
        {
            var randon = new Random();
            var pokeId = randon.Next(1, 898);
            List<Pokemon> pokemonsList = new List<Pokemon>();

            for(int i = 0; i < limit; i++)
            {
                var uri = string.Format(_endpoints.Base + _endpoints.Pokemon, pokeId);
                var response = GetUrl(uri).Result;
                Pokemon pokemon = null;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    pokemon = JsonConvert.DeserializeObject<Pokemon>(json);
                    pokemon.Evolutions = GetEvolutions(pokemon);
                    pokemonsList.Add(pokemon);
                }
            }

            return Json(pokemonsList);

        }

        [HttpPost]
        [Route("trainer")]
        public JsonResult Trainer(Trainer trainer)
        {
            var jsonResult = new JsonResult("");
            try
            {
                db.Trainers.Add(trainer);
                if (db.SaveChanges().ToBool())
                    jsonResult = Json(new { success = "true", trainer = trainer });
            }
            catch(Exception ex)
            {
                jsonResult = Json(new { success = "false", message = ex.Message });
            }

            return jsonResult;
        }

        private List<string> GetEvolutions(Pokemon pokemon)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Evolution, pokemon.Id);
            List<string> evolutions = new List<string>();

            var response = GetUrl(uri).Result;

            if (response.IsSuccessStatusCode)
            {
                var textJson = response.Content.ReadAsStringAsync().Result.ToLower();

                if (textJson.Contains(pokemon.Name.ToLower()))
                {
                    var regex = new Regex("species\":{\"name\":\"([a-zA-Z0-9]*)[^\",]");
                    var match = regex.Matches(textJson);
                    if (match.Count > 0)
                    {
                        foreach(var evo in match)
                            evolutions.Add(evo.ToString().Replace("species\":{\"name\":\"", ""));
                    }
                }
            }
            evolutions.Reverse();
            var reversed = evolutions;

            return reversed;
        }


    }
}
