using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebAPITareas.Controllers
{
    public class TareasController : ApiController
    {
        private ci2_test_Entities db = new ci2_test_Entities();

        // GET: api/Tareas
        [Authorize]
        public IQueryable<Tarea> GetTareas()
        {
            return db.Tareas.OrderBy(t => t.fecha_vencimiento);
        }

        // GET: api/Tareas?usuario=admin
        [Authorize]
        public IQueryable<Tarea> GetTareasByUsuario(string usuario)
        {
            return db.Tareas.Where(t => t.usuario == usuario).OrderBy(t => t.fecha_vencimiento);
        }

        // GET: api/Tareas?finalizada=true
        [Authorize]
        public IQueryable<Tarea> GetTareasByFinalizada(string finalizada)
        {
            IQueryable<Tarea> tareas = null;

            if (finalizada != null)
            {
                bool ind_finalizada = bool.Parse(finalizada);
                tareas = db.Tareas.Where(t => t.ind_finalizada == ind_finalizada).OrderBy(t => t.fecha_vencimiento);
            }
            else
            {
                tareas = db.Tareas.OrderBy(t => t.fecha_vencimiento);
            }

            return tareas;
        }

        // GET: api/Tareas?usuario=admin&finalizada=true
        [Authorize]
        public IQueryable<Tarea> GetTareas(string usuario, string finalizada)
        {
            IQueryable<Tarea> tareas = null;

            if (finalizada != null)
            {
                bool ind_finalizada = bool.Parse(finalizada);
                tareas = db.Tareas.Where(t => t.usuario == usuario && t.ind_finalizada == ind_finalizada).OrderBy(t => t.fecha_vencimiento);
            }
            else
            {
                tareas = db.Tareas.Where(t => t.usuario == usuario).OrderBy(t => t.fecha_vencimiento);
            }

            return tareas;
        }

        // GET: api/Tareas/5
        /*[Authorize]
        [ResponseType(typeof(Tarea))]
        public IHttpActionResult GetTarea(int id)
        {
            Tarea tarea = db.Tareas.Find(id);
            if (tarea == null)
            {
                return NotFound();
            }

            return Ok(tarea);
        }*/

        // POST: api/Tareas/
        [HttpPost]
        [Route("api/Tareas/crear")]
        [Authorize]
        [ResponseType(typeof(Tarea))]
        public IHttpActionResult PostTarea(Tarea tarea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (tarea.descripcion == null || tarea.fecha_vencimiento == default(DateTime))
            {
                // Los parametros descripcion y fecha_vencimiento de la Tarea son obligatorios
                return StatusCode(HttpStatusCode.BadRequest);
            }

            tarea.fecha_creacion = DateTime.Now;
            tarea.ind_finalizada = false;

            var identity = Thread.CurrentPrincipal.Identity;
            tarea.usuario = identity.Name;

            db.Tareas.Add(tarea);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TareaExists(tarea.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tarea);            
        }

        // POST: api/Tareas/actualizar
        [HttpPost]
        [Route("api/Tareas/actualizar")]
        [Authorize]
        [ResponseType(typeof(Tarea))]
        public IHttpActionResult UpdateTarea(Tarea tarea)
        {
            if (!TareaExists(tarea.Id))
            {
                // Tarea no existe
                return NotFound();
            }

            Tarea tareaActual = db.Tareas.Find(tarea.Id);

            var identity = Thread.CurrentPrincipal.Identity;
            if (tareaActual.usuario != identity.Name)
            {
                // Solo se le permite modificar al usuario dueño de la tarea
                return Unauthorized();
            }

            if (tarea.descripcion != null)
                tareaActual.descripcion = tarea.descripcion;
            if (tarea.fecha_vencimiento != default(DateTime))
                tareaActual.fecha_vencimiento = tarea.fecha_vencimiento;
            if (tarea.ind_finalizada)
                tareaActual.ind_finalizada = tarea.ind_finalizada;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(tareaActual).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TareaExists(tarea.Id))
                {
                    // Tarea no existe
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tareaActual);
        }

        // DELETE: api/Tareas/borrar
        [HttpPost]
        [Route("api/Tareas/borrar")]
        [Authorize]
        [ResponseType(typeof(Tarea))]
        public IHttpActionResult DeleteTarea(Tarea tareaId)
        {
            Tarea tarea = db.Tareas.Find(tareaId.Id);
 
            if (tarea == null || tareaId.Id == 0)
            {
                // Tarea Nula o parametro Id Nulo
                return NotFound();
            }

            var identity = Thread.CurrentPrincipal.Identity;
            if (tarea.usuario != identity.Name)
            {
                // Solo se le permite borrar al usuario dueño de la tarea
                return Unauthorized();
            }

            db.Tareas.Remove(tarea);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TareaExists(int id)
        {
            return db.Tareas.Count(e => e.Id == id) > 0;
        }
    }
}