using api.ApiModel;
using api.Extensions;
using api.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace api.Controllers
{
    [Route("trainer")]
    public class TrainerController : BaseController
    {
        public TrainerController(Endpoint endpoint) : base(endpoint)
        {
        }

        [HttpPost]
        public JsonResult Index(Trainer trainer)
        {
            var response = new Response
            {
                StatusCode = 200,
                Success = true,
                Message = string.Empty
            };

            try
            {
                db.Trainers.Add(trainer);
                if (db.SaveChanges().ToBool())
                    response.Message = $"Trainner {trainer.Name} added.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.StatusCode = 400;
            }

            return Json(response);
        }
    }
}
