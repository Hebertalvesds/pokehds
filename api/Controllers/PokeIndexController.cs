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
            var response = new Response
            {
                Pokemon = null,
                StatusCode = 200,
                Success = true,
                ErrorMessage = string.Empty
            };

            try
            {
                var pokemon = Pokemon(pokemonId.ToString());
                response.Pokemon = pokemon;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return Json(response);

        }

        [HttpGet]
        //GET: pokemon/bulbasaur
        [Route("pokemon/name")]
        public JsonResult GetPokemon(string pokemonName)
        {
            var response = new Response { 
                Pokemon = null, 
                StatusCode = 200, 
                Success = true, 
                ErrorMessage = string.Empty 
            };

            try
            {
                var pokemon = Pokemon(pokemonName);
                response.Pokemon = pokemon;
            }catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }

            return Json(response);
        }

        [HttpGet]
        //pokemon/list/10
        [Route("pokemon/list/{limit}")]
        public JsonResult GetList(int limit = 10)
        {
            var randon = new Random();
            var pokeId = randon.Next(1, _endpoints.MaxPokemonNumber);
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

        private Pokemon Pokemon(string pokemonIdOrName)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Pokemon, pokemonIdOrName);
            var response = GetUrl(uri).Result;
            Pokemon pokemon = null;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                pokemon = JsonConvert.DeserializeObject<Pokemon>(json);
                pokemon.Evolutions = GetEvolutions(pokemon);
            }

            return pokemon;
        }
        private List<Pokemon> GetEvolutions(Pokemon pokemon)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Evolution, pokemon.Id);
            List<Pokemon> evolutions = new List<Pokemon>();

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
                        {
                            var evoName = evo.ToString().Replace("species\":{\"name\":\"", "");
                            var evolution = Pokemon(evoName);
                            evolutions.Add(evolution);
                        }
                    }
                }
            }
            evolutions.Reverse();
            var reversed = evolutions;

            return reversed;
        }


    }
}
