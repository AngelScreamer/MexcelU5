using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MexcelU5.Repositories;
using MexcelU5.Models;
using MexcelU5.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ControlUsuarioSamaniego.Helpers;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace MexcelU5.Controllers
{
    public class HomeController : Controller
    {
        public IWebHostEnvironment Enviroment { get; set; }
        public HomeController(IWebHostEnvironment env)
        {
            Enviroment = env;
        }
        [Authorize(Roles = "Cliente, Admin")]
        public IActionResult Index(string id)
		{
			Repository repos = new Repository();
			IndexViewModel vm = new IndexViewModel
			{
				Celulares = id == null ? repos.GetCel() : repos.GetCelByLetra(id),
				BusquedaLetra = repos.GetLetras()
			};
			return View(vm);
		}
		[Route("Celular/{id}")]
		public IActionResult CaracteristicasCel(string id)
		{
			Repository repos = new Repository();
			CaracteristicasViewModel vm = new CaracteristicasViewModel();
			vm.Smartphones = repos.GetCelByNombre(id);

			if (vm.Smartphones == null)
			{
				return RedirectToAction("Index");
			}
			else
			{
				return View(vm);
			}
		}

		[Authorize(Roles = "Cliente")]
		public IActionResult InfoUsuario()
		{
			return View();
		}
        [AllowAnonymous]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Registrar(Usuario us, string contrasena, string contrasena2)
        {
            celularesContext context = new celularesContext();
            try
            {
                UsuarioRepository<Usuario> reposUsuario = new UsuarioRepository<Usuario>(context);
                if (context.Usuario.Any(x => x.Correo == us.Correo))
                {
                    ModelState.AddModelError("", "Este usuario ya forma parte de Mexcel.");
                    return View(us);
                }
                else
                {
                    if (contrasena == contrasena2)
                    {
                        us.Contrasena = HashingHelpers.GetHelper(contrasena);
                        us.Codigo = CodeHelper.GetCodigo();
                        us.Activo = 0;
                        reposUsuario.Insert(us);

                        MailMessage mensajeXperience = new MailMessage();
                        mensajeXperience.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Mexcel");
                        mensajeXperience.To.Add(us.Correo);
                        mensajeXperience.Subject = "Verifica tu correo para Mexcel";
                        string text = System.IO.File.ReadAllText(Enviroment.WebRootPath + "/ConfirmacionDeCorreo.html");
                        mensajeXperience.Body = text.Replace("{##codigo##}", us.Codigo.ToString());
                        mensajeXperience.IsBodyHtml = true;

                        SmtpClient clienteXperience = new SmtpClient("smtp.gmail.com", 587);
                        clienteXperience.EnableSsl = true;
                        clienteXperience.UseDefaultCredentials = false;
                        clienteXperience.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                        clienteXperience.Send(mensajeXperience);
                        return RedirectToAction("Activar");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ambas contraseñas no coinciden entre sí, intentalo de nuevo.");
                        return View(us);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(us);
            }
        }
        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(Usuario us, bool mantener)
        {
            celularesContext context = new celularesContext();
            UsuarioRepository<Usuario> reposUsuario = new UsuarioRepository<Usuario>(context);
            var datos = reposUsuario.GetUserByEmail(us.Correo);
            if (datos != null && HashingHelpers.GetHelper(us.Contrasena) == datos.Contrasena)
            {
                if (datos.Activo == 1)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + datos.NombreUsuario));
                    info.Add(new Claim(ClaimTypes.Role, "Cliente"));
                    info.Add(new Claim("Correo", datos.Correo));
                    info.Add(new Claim("Nombre", datos.NombreUsuario));

                    var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);

                    if (mantener == true)
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                            new AuthenticationProperties { IsPersistent = true });
                    }
                    else
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                            new AuthenticationProperties { IsPersistent = false });
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Activa tu cuenta para poder iniciar sesión en Mexcel.");
                    return View(us);
                }
            }
            else
            {
                ModelState.AddModelError("", "El correo electrónico o la contraseña son incorrectas.");
                return View(us);
            }
        }
        [Authorize(Roles = "Cliente, Admin")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("IniciarSesion");
        }


        [AllowAnonymous]
        public IActionResult Activar()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Activar(int codigo)
        {
            celularesContext context = new celularesContext();
            UsuarioRepository<Usuario> reposUsuario = new UsuarioRepository<Usuario>(context);
            var usuario = context.Usuario.FirstOrDefault(x => x.Codigo == codigo);

            if (usuario != null && usuario.Activo == 0)
            {
                var cgo = usuario.Codigo;
                if (codigo == cgo)
                {
                    usuario.Activo = 1;
                    reposUsuario.Edit(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "Tu codigo para Mexcel no es correcto, intentalo de nuevo.");
                    return View((object)codigo);
                }
            }
            else
            {
                ModelState.AddModelError("", "El usuario no existe en nuestra plataforma.");
                return View((object)codigo);
            }
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult CambiarContra()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult CambiarContra(string correo, string contra, string nuevaContrasena, string nvaContrasena)
        {
            celularesContext context = new celularesContext();

            try
            {
                UsuarioRepository<Usuario> reposUsuario = new UsuarioRepository<Usuario>(context);
                var usuario = reposUsuario.GetUserByEmail(correo);

                if (usuario.Contrasena != HashingHelpers.GetHelper(contra))
                {
                    ModelState.AddModelError("", "La contraseña Mexcel es incorrecta.");
                    return View();
                }
                else
                {
                    if (nuevaContrasena != nvaContrasena)
                    {
                        ModelState.AddModelError("", "Ambas contraseñas no coinciden entre sí, intentelo de nuevo.");
                        return View();
                    }
                    else if (usuario.Contrasena == HashingHelpers.GetHelper(nuevaContrasena))
                    {
                        ModelState.AddModelError("", "Esta introduciendo una contraseña antigua, intentelo una cez mas con una contaseña distinta.");
                        return View();
                    }
                    else
                    {
                        usuario.Contrasena = HashingHelpers.GetHelper(nuevaContrasena);
                        reposUsuario.Edit(usuario);
                        return RedirectToAction("IniciarSesion");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult RecuperarContra()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult RecuperarContra(string correo)
        {
            try
            {
                celularesContext context = new celularesContext();
                UsuarioRepository<Usuario> repository = new UsuarioRepository<Usuario>(context);
                var usuario = repository.GetUserByEmail(correo);

                if (usuario != null)
                {
                    var contraTemp = CodeHelper.GetCodigo();
                    MailMessage mensaje = new MailMessage();
                    mensaje.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Mexcel");
                    mensaje.To.Add(correo);
                    mensaje.Subject = "Recupera tu contraseña en Mexcel";
                    string text = System.IO.File.ReadAllText(Enviroment.WebRootPath + "/RecuperacionDeContrasena.html");
                    mensaje.Body = text.Replace("{##contraTemp##}", contraTemp.ToString());
                    mensaje.IsBodyHtml = true;

                    SmtpClient cliente = new SmtpClient("smtp.gmail.com", 587);
                    cliente.EnableSsl = true;
                    cliente.UseDefaultCredentials = false;
                    cliente.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                    cliente.Send(mensaje);
                    usuario.Contrasena = HashingHelpers.GetHelper(contraTemp.ToString());
                    repository.Edit(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "El correo Mexcel no se encuentra registrado en nuestra página.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View((object)correo);
            }
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult EliminarCuenta()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult EliminarCuenta(string correo, string contra)
        {
            celularesContext context = new celularesContext();
            try
            {
                UsuarioRepository<Usuario> reposUsuario = new UsuarioRepository<Usuario>(context);
                var usuario = reposUsuario.GetUserByEmail(correo);
                if (usuario != null)
                {
                    if (HashingHelpers.GetHelper(contra) == usuario.Contrasena)
                    {
                        reposUsuario.Delete(usuario);
                    }
                    else
                    {
                        ModelState.AddModelError("", "La contraseña Mexcel introducida es incorrecta, intentelo de nuevo.");
                        return View();
                    }
                }
                return RedirectToAction("IniciarSesion");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error. Inténtelo de nuevo en otro momento.");
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}
