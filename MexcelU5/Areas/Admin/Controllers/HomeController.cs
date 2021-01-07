using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using MexcelU5.Models.ViewModels;
using MexcelU5.Models;
using MexcelU5.Repositories;
using System.Threading.Tasks;
using MexcelU5.Areas.Admin.HashHelpers;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace MexcelU5.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IWebHostEnvironment Enviroment { get; set; }
        public HomeController(IWebHostEnvironment env)
        {
            Enviroment = env;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string id)
        {
            
            celularesContext context = new celularesContext();
            Repositories.Repository repos = new Repositories.Repository();
            
            return View(repos.GetAll().OrderBy(x => x.Id));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Agregar()
        {
            AdminCelularesViewModel vm = new AdminCelularesViewModel();
            Repository repository = new Repository();
            vm.Marcas = repository.GetMarca();
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Agregar(AdminCelularesViewModel vm)
        {
            Repository repos = new Repository();
            try
            {
                if (vm.Archivo == null)
                {
                    ModelState.AddModelError("", "Seleccione la imágen del celular.");

                    vm.Marcas = repos.GetMarca();
                    return View(vm);
                }
                else
                {
                    if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un archivo jpg de menos de 2MB.");
                        vm.Marcas = repos.GetMarca();
                        return View(vm);
                    }
                }

                repos.Agregar(vm.Smartphones);

                if (vm.Archivo != null)
                {
                    FileStream fs = new FileStream(Enviroment.WebRootPath + "/CatalogoS/" + vm.Smartphones.Id + ".jpg", FileMode.Create);
                    vm.Archivo.CopyTo(fs);
                    fs.Close();
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Marcas = repos.GetMarca();
                return View(vm);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Editar(uint id)
        {
            AdminCelularesViewModel vm = new AdminCelularesViewModel();
            Repository repository = new Repository();
            vm.Marcas = repository.GetMarca();
            vm.Smartphones = repository.GetCelById(id);
            if (vm.Smartphones == null)
            {
                return RedirectToAction("Index");
            }
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]

        public IActionResult Editar(AdminCelularesViewModel vm)
        {
            Repository repository = new Repository();
            var celular = repository.GetCelById(vm.Smartphones.Id);
            try
            {

                vm.Marcas = repository.GetMarca();
                if (celular != null)
                {
                    celular.Caracteristicas.Bateria = vm.Smartphones.Caracteristicas.Bateria;
                    celular.Caracteristicas.Camara = vm.Smartphones.Caracteristicas.Camara;
                    celular.Caracteristicas.Peso = vm.Smartphones.Caracteristicas.Peso;
                    celular.Caracteristicas.Precio = vm.Smartphones.Caracteristicas.Precio;
                    celular.Caracteristicas.So = vm.Smartphones.Caracteristicas.So;
                    celular.Caracteristicas.Tamaño = vm.Smartphones.Caracteristicas.Tamaño;

                    celular.Nombre = vm.Smartphones.Nombre;
                    celular.Pantalla = vm.Smartphones.Pantalla;
                    celular.Procesador = vm.Smartphones.Procesador;
                    celular.Ram = vm.Smartphones.Ram;
                    celular.Expansion = vm.Smartphones.Expansion;
                    celular.Descripcion = vm.Smartphones.Descripcion;
                    celular.Almacenamiento = vm.Smartphones.Almacenamiento;
                    repository.Editar(celular);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Marcas = repository.GetMarca();
                return View(vm);
            }

        }
        [Authorize(Roles = "Admin")]
        public IActionResult Eliminar(uint id)
        {
            Repository repository = new Repository();
            var cel = repository.GetCelById(id);
            return View(cel);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Eliminar(Smartphones cel)
        {
            Repository repository = new Repository();
            var smart = repository.GetCelById(cel.Id);
            if (smart != null)
            {
                smart.Eliminado = 1;
                repository.Editar(smart);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Imagen(uint id)
        {
            celularesContext context = new celularesContext();
            Repository repos = new Repository();
            AdminCelularesViewModel vm = new AdminCelularesViewModel();
            vm.Marcas = repos.GetMarca();
            vm.Smartphones = repos.GetCelById(id);
            if (System.IO.File.Exists(Enviroment.WebRootPath + $"/CatalogoS/{vm.Smartphones.Id}.jpg"))
            {
                vm.Imagen = vm.Smartphones.Id + ".jpg";
            }
            else
            {
                vm.Imagen = "NoDisp.jpg";
            }
            return View(vm);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Imagen(AdminCelularesViewModel vm)
        {

            try
            {
                if (vm.Archivo == null)
                {
                    ModelState.AddModelError("", "Debe seleccionar la imagen de la especie.");
                    return View(vm);
                }
                else
                {
                    if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un archivo jpg maximo de 2MB.");
                        return View(vm);
                    }
                }
                if (vm.Archivo != null)
                {
                    FileStream fs = new FileStream(Enviroment.WebRootPath + "/CatalogoS/" + vm.Smartphones.Id + ".jpg", FileMode.Create);
                    vm.Archivo.CopyTo(fs);
                    fs.Close();
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception error)
            {
                ModelState.AddModelError("", error.Message);
                return View(vm);
            }
        }
        public IActionResult InicioDeSesionAdmin()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> InicioDeSesionAdmin(Administrador ad)
        {
           celularesContext context = new celularesContext();
            UsuarioRepository<Administrador> directorRepos = new UsuarioRepository<Administrador>(context);
            var director = context.Administrador.FirstOrDefault(x => x.Clave == ad.Clave);
            try
            {
                if (director != null && director.Contrasena == HashHelp.GetHelper(ad.Contrasena))
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + director.Nombre));
                    info.Add(new Claim(ClaimTypes.Role, "Admin"));
                    info.Add(new Claim("Clave", director.Nombre.ToString()));
                    info.Add(new Claim("Nombre", director.Nombre));

                    var claimsidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsprincipal = new ClaimsPrincipal(claimsidentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsprincipal,
                        new AuthenticationProperties { IsPersistent = true });
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "La clave o la contraseña del administrador son incorrectas.");
                    return View(ad);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ad);
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("IniciarSesion");
        }
    }
}
