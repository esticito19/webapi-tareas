/*
 * Web API Tareas
 *
 * Servicio de Gestión de Tareas: proporciona los métodos necesarios para la consulta, creación, actualización y borrado de las tareas a los usuarios autorizados.
 *
 * OpenAPI spec version: 1.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using IO.Swagger.Attributes;
using IO.Swagger.Models;

namespace IO.Swagger.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    public class ActualizarApiController : Controller
    { 
        /// <summary>
        /// Actualizar Tarea
        /// </summary>
        /// <remarks>Realiza la actualización de una Tarea</remarks>
        /// <param name="body">Tarea. Para la actualización de Tarea se requiere el atributo Id</param>
        /// <response code="200">successful operation</response>
        /// <response code="404">Not Found Response (La Tarea no existe)</response>
        [HttpPost]
        [Route("/api/Tareas/actualizar")]
        [ValidateModelState]
        [SwaggerOperation("UpdateTarea")]
        [SwaggerResponse(statusCode: 200, type: typeof(Tarea), description: "successful operation")]
        [SwaggerResponse(statusCode: 404, type: typeof(string), description: "Not Found Response (La Tarea no existe)")]
        public virtual IActionResult UpdateTarea([FromBody]Tarea body)
        { 
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Tarea));

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404, default(string));
            string exampleJson = null;
            
                        var example = exampleJson != null
                        ? JsonConvert.DeserializeObject<Tarea>(exampleJson)
                        : default(Tarea);            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
