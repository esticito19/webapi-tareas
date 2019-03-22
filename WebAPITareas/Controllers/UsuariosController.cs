using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;

namespace WebAPITareas.Controllers
{
    public class UsuariosController : ApiController
    {
        private ci2_test_Entities db = new ci2_test_Entities();

        // GET: api/Usuarios
        [Authorize]
        [ResponseType(typeof(IQueryable<Usuario>))]
        public IHttpActionResult GetUsuarios()
        {
            if (!principalIsAdmin())
                return Unauthorized();

            return Ok(db.Usuarios);
        }

        // CREATE: api/Usuarios
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult PostUsuario(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (usuario.ind_admin == null)
            {
                usuario.ind_admin = "F";
            }

            if (!principalIsAdmin())
                usuario.ind_admin = "F";

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = usuario.Id }, usuario);
        }

        // UPDATE: api/Usuarios/actualizar
        [Authorize]
        [HttpPost]
        [Route("api/Usuarios/actualizar")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult UpdateUsuario(Usuario usuario)
        {
            if (usuario == null || usuario.username == null)
            {
                // Usuario Nulo o parametro Username Nulo
                return NotFound();
            }

            if (!UsuarioExists(usuario.username))
            {
                // Usuario no existe
                return NotFound();
            }

            string nombreUsuario = usuario.username;
            Usuario usuarioActual = db.Usuarios.Where(u => u.username == nombreUsuario).First();

            // Se actualiza ind_admin solo si el Usuario conectado es de tipo Admin
            if (principalIsAdmin())
            {
                if (usuario.ind_admin != null)
                    usuarioActual.ind_admin = usuario.ind_admin;

                // Se actualiza password
                if (usuario.password != null)
                    usuarioActual.password = usuario.password;
            }
            else
            {   
                // Si el usuario conectado es el mismo a actualizar, se permite modificar el password
                if (usuario.username == Thread.CurrentPrincipal.Identity.Name)
                {
                    // Se actualiza password
                    if (usuario.password != null)
                        usuarioActual.password = usuario.password;
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(usuarioActual).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.username))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(usuarioActual);
        }

        // DELETE: api/Usuarios/borrar
        [Authorize]
        [HttpPost]
        [Route("api/Usuarios/borrar")]
        [ResponseType(typeof(Usuario))]
        public IHttpActionResult DeleteUsuario(Usuario usuario)
        {
            if (usuario == null || usuario.username == null)
            {
                // Usuario Nulo o parametro Username Nulo
                return NotFound();
            }

            string nombreUsuario = usuario.username;
            if (!UsuarioExists(nombreUsuario))
            {
                // Usuario no existe
                return NotFound();
            }

            if (!principalIsAdmin())
                return Unauthorized();

            Usuario usuarioActual = db.Usuarios.Where(u => u.username == nombreUsuario).First();

            db.Usuarios.Remove(usuarioActual);
            db.SaveChanges();

            return Ok(usuarioActual);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(string nombreUsuario)
        {
            return db.Usuarios.Count(e => e.username == nombreUsuario) > 0;
        }

        private bool principalIsAdmin()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            string nombreUsuario = identity.Name;
            Usuario usuario = db.Usuarios.Where(u => u.username == nombreUsuario).First();

            if (usuario.ind_admin == "T")
                return true;

            return false;
        }
    }
}