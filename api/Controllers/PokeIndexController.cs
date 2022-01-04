using api.ApiModel;
using api.Extensions;
using api.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Dynamic;

namespace api.Controllers
{
    [Route("pokeindex")]
    public class PokeIndexController : BaseController
    {
        public PokeIndexController(Endpoint endpoint) : base(endpoint) { }

        [HttpGet]
        //GET: pokemon/1
        //GET: pokemon/bulbasaur
        [Route("pokemon/{idOrName}")]
        public JsonResult GetPokemon(string idOrName = "1")
        {
            var response = new Response
            {
                StatusCode = 200,
                Success = true,
                Message = string.Empty
            };

            try
            {
                var pokemon = GetInternalPokemon(idOrName);
                response.Message = pokemon;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.StatusCode = 400;
            }

            return Json(response);

        }

        [HttpGet]
        //pokemon/list/10
        [Route("pokemon/list/{limit}")]
        public JsonResult GetList(int limit = 10)
        {
            var pokeId = 0;
            List<Pokemon> pokemonsList = new List<Pokemon>();
            List<int> generatedIds = new List<int> { 0 };

            var response = new Response
            {
                Message = string.Empty,
                StatusCode = 200,
                Success = true
            };

            try
            {
                for (int i = 0; i < limit; i++)
                {
                    pokeId = RandomizePokeId(pokeId, generatedIds);

                    var pokemon = GetInternalPokemon(pokeId.ToString());
                    pokemonsList.Add(pokemon);

                    generatedIds.Add(pokeId);
                }

            }catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.StatusCode = 400;
            }

            return Json(pokemonsList);

        }

        private Pokemon GetInternalPokemon(string pokemonIdOrName, bool isEvolution = false)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Pokemon, pokemonIdOrName);
            var response = GetUrl(uri).Result;
            Pokemon pokemon = null;

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                pokemon = JsonSerializer.Deserialize<Pokemon>(json);
                pokemon.Evolutions = isEvolution ? null : GetEvolutions(pokemon);
            }

            return pokemon;
        }

        private string ExtractEvolutionChainUrl(Pokemon pokemon)
        {
            var uri = string.Format(_endpoints.Base + _endpoints.Species, pokemon.Id);
            string url = string.Empty;

            try
            {
                var responseMessage = GetUrl(uri).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    dynamic data = JsonSerializer.Deserialize<ExpandoObject>(responseMessage.Content.ReadAsStringAsync().Result);
                    pokemon.IsBaby = data?.is_baby?.ToString().ToLower().Equals("true");
                    var evolutionChain = JsonSerializer.Deserialize<ExpandoObject>(data?.evolution_chain?.ToString());
                    url = evolutionChain.url?.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return url;
        }

        private List<Pokemon> GetEvolutions(Pokemon pokemon)
        {
            var uri = ExtractEvolutionChainUrl(pokemon);
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
                        for (var i = 0; i < match.Count; i++)
                        {
                            var evo = match[i].ToString().Replace("species\":{\"name\":\"", "");
                            if (evo.Equals(pokemon.Name.ToLower()))
                                continue;

                            var pokemonEvo = GetInternalPokemon(evo, true);
                            evolutions.Add(pokemonEvo);
                        }
                    }
                }
            }
            evolutions.Reverse();
            var reversed = evolutions;

            return reversed;
        }
        private int RandomizePokeId(int currentId, List<int> generatedIds)
        {
            var randon = new Random();
            var newId = currentId;
            if (generatedIds.Contains(currentId))
            {
                currentId = randon.Next(1, _endpoints.MaxPokemonNumber);
                return RandomizePokeId(currentId, generatedIds);
            }
            else
            {
                return newId;
            }
        }

    }
}
