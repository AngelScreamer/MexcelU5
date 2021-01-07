using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MexcelU5.Models;
using MexcelU5.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MexcelU5.Repositories
{
    public class Repository
    {
		celularesContext context = new celularesContext();

		public virtual IEnumerable<Smartphones> GetAll()
		{
			return context.Set<Smartphones>();
		}
		public IEnumerable<CelViewModel> GetCel()
		{
			return context.Smartphones.Where(x => x.Eliminado == 0).OrderBy(x => x.Nombre).Select(x => new CelViewModel { Id = x.Id, Nombre = x.Nombre });
		}
		public Smartphones GetCelById(uint id)
		{
			return context.Smartphones.Include(x => x.Caracteristicas)
				.Include(x => x.IdMarcaNavigation)
				.FirstOrDefault(x => x.Id == id);
		}

		public IEnumerable<CelViewModel> GetCelByLetra(string letra)
		{
			return GetCel().Where(x => x.Nombre.StartsWith(letra));
		}

		public virtual void Delete(Smartphones entidad)
		{
			context.Remove(entidad);
			context.SaveChanges();
		}
		public IEnumerable<char> GetLetras()
		{
			return context.Smartphones.OrderBy(x => x.Nombre).Select(x => x.Nombre.First());
		}

		public Smartphones GetCelByNombre(string nombre)
		{
			nombre = nombre.Replace("-", " ");
			return context.Smartphones.Include(x => x.Caracteristicas).Include(x => x.IdMarcaNavigation).FirstOrDefault(x => x.Nombre == nombre);
		}

		public IEnumerable<Marcas> GetCelByMarca()
		{
			return context.Marcas.Include(x => x.Smartphones).OrderBy(x => x.Nombre);
		}
		public IEnumerable<Marcas> GetMarca()
		{
			return context.Marcas.OrderBy(x => x.Nombre);
		}
		public virtual void Agregar(Smartphones entidad)
		{
			if (Validar(entidad))
			{
				context.Add(entidad);		
				context.SaveChanges();
			}
		}

		public virtual void Editar(Smartphones entidad)
		{
			if (Validar(entidad))
			{
				context.Update(entidad);
				context.SaveChanges();
			}
		}
		public virtual bool Validar(Smartphones entidad)
		{
			if (string.IsNullOrWhiteSpace(entidad.Nombre))
				throw new Exception("Escriba el nombre.");

			if (string.IsNullOrWhiteSpace(entidad.Descripcion))
				throw new Exception("Escriba la descripción.");
			
			if (entidad.IdMarca <= 0)
				throw new Exception("Escriba la marca.");

			if (entidad.Caracteristicas.Peso <= 0)
				throw new Exception("Escriba el peso.");
			
			if (entidad.Caracteristicas.Tamaño <= 0)
				throw new Exception("Escriba el tamaño."); 
			
			if (entidad.Almacenamiento < 0)
				throw new Exception("Escriba el almacenamiento.");

			if (string.IsNullOrWhiteSpace(entidad.Pantalla))
				throw new Exception("Escriba la resolusion.");

			if (string.IsNullOrWhiteSpace(entidad.Procesador))
				throw new Exception("Escriba el procesador.");

			if (entidad.Ram < 0)
				throw new Exception("Escriba la memoria RAM.");

			if (entidad.Caracteristicas.Bateria < 0)
				throw new Exception("Escriba la batería");

			if (string.IsNullOrWhiteSpace(entidad.Caracteristicas.Camara))
				throw new Exception("Escriba caracteristicas de la cámara.");

			if (entidad.Caracteristicas.Precio < 0)
				throw new Exception("Escriba el precio.");

			if (string.IsNullOrWhiteSpace(entidad.Caracteristicas.So))
				throw new Exception("Escriba el sistema operativo.");

			if (string.IsNullOrWhiteSpace(entidad.Expansion))
				throw new Exception("Escriba la disponibilidad de expancion.");
			return true;
			
		}
		
	}
}
