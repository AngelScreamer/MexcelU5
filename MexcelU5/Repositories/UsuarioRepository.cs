using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MexcelU5.Models;
using MexcelU5.Models.ViewModels;

namespace MexcelU5.Repositories
{
    public class UsuarioRepository<T> where T : class
    {
        public celularesContext Context { get; set; }
        public UsuarioRepository(celularesContext context)
        {
            Context = context;
        }

        public Usuario GetUserById(int id)
        {
            return Context.Usuario.FirstOrDefault(x => x.Id == id);
        }
        public Usuario GetUserByEmail(string correo)
        {
            return Context.Usuario.FirstOrDefault(x => x.Correo == correo);
        }
        public Usuario GetUser(Usuario id)
        {
            return Context.Find<Usuario>(id);
        }

        public bool Validate(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.NombreUsuario))
                throw new Exception("Ingrese el nombre de usuario.");
            if (string.IsNullOrEmpty(usuario.Correo))
                throw new Exception("Ingrese el correo electrónico del usuario.");
            if (string.IsNullOrEmpty(usuario.Contrasena))
                throw new Exception("Ingrese la contraseña del usuario.");
            return true;
        }

        public virtual void Insert(Usuario usuario)
        {
            if (Validate(usuario))
            {
                Context.Add(usuario);
                Context.SaveChanges();
            }
        }
        public virtual void Edit(Usuario usuario)
        {
            if (Validate(usuario))
            {
                Context.Update<Usuario>(usuario);
                Context.SaveChanges();
            }
        }
        public virtual void Delete(Usuario usuario)
        {
            Context.Remove<Usuario>(usuario);
            Context.SaveChanges();
        }
    }
}
