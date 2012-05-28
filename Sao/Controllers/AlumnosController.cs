using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sao.Models;

namespace Sao.Controllers
{ 
    public class AlumnosController : Controller
    {
        private SaoEntities db = new SaoEntities();

        //
        // GET: /Alumnos/

        public ViewResult Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.TipoMensaje = TempData["TipoMensaje"];
            return View(db.Alumnos.ToList());
        }

        //
        // GET: /Alumnos/Details/5

        public ViewResult Details(int id)
        {
            Alumnos alumnos = db.Alumnos.Single(a => a.Id == id);
            return View(alumnos);
        }

        //
        // GET: /Alumnos/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Alumnos/Create

        [HttpPost]
        public ActionResult Create(Alumnos alumnos)
        {
            try
            {
                alumnos.FechaIngreso = null;
                db.Alumnos.AddObject(alumnos);
                db.SaveChanges();
                TempData["Message"] = "El alumno se ha creado con éxito.";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            } catch {
                ViewBag.Message = "Error al crear alumno";
                ViewBag.TipoMensaje = "Error";
                return View(alumnos);
            }
        }
        
        //
        // GET: /Alumnos/Edit/5
 
        public ActionResult Edit(int id)
        {
            Alumnos alumnos = db.Alumnos.Single(a => a.Id == id);
            return View(alumnos);
        }

        //
        // POST: /Alumnos/Edit/5

        [HttpPost]
        public ActionResult Edit(Alumnos alumnos, FormCollection form)
        {
            try
            {
                if (form["Fecha"].Contains("true")) {
                    alumnos.FechaIngreso = null;                
                }
                db.Alumnos.Attach(alumnos);
                db.ObjectStateManager.ChangeObjectState(alumnos, EntityState.Modified);
                db.SaveChanges();
                TempData["Message"] = "La información ha sido actualizada correctamente";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Error al crear alumno";
                ViewBag.TipoMensaje = "Error";
                return View(alumnos);
            }
        }
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Alumnos alumno)
        {
            try
            {
                Alumnos oAlumno = db.Alumnos.Single(z => z.Clave == alumno.Clave && z.DNI == alumno.DNI);
                /*Entro alguna vez? NO => Lo dejo entrar*/
                bool permiso = false;
                if ((oAlumno.FechaIngreso == null) || (oAlumno.DNI == "Admin"))
                {
                    permiso = true;
                    //Actualizo la fecha para que empiece a contrar 24 horas!!
                    oAlumno.FechaIngreso = DateTime.Now;
                    db.ExecuteStoreCommand("UPDATE Alumnos SET FechaIngreso = getdate() where id=" + oAlumno.Id);
                } else {
                    /*SI => Hace cuanto entro??*/
                    TimeSpan ts = DateTime.Now - oAlumno.FechaIngreso.Value;
                    int horas = ts.Hours + ts.Days*24;
                    if (horas < 24)
                    {
                        permiso = true;
                        db.ExecuteStoreCommand("UPDATE Alumnos SET FechaIngreso = getdate() where id=" + oAlumno.Id);
                    }
                }
                if (permiso) {
                    Session["id"] = oAlumno.Id;
                    Session["Usuario"] = oAlumno.DNI;

                    if (oAlumno.DNI.Trim() != "Admin")
                    {
                        return RedirectToAction("Ver", "Cursos");
                    } else { 
                        return RedirectToAction("Index", "Cursos");
                    }
                   
                }
                else
                {
                    ViewBag.Message = "Ha habido un error al ingresar al sistema. <br>Seguramente se deba a que han pasado más de 24 horas desde el último inicio de sesión. <br>Si tienes dudas, contáctate con el administrador del sistema.";
                    ViewBag.TipoMensaje = "Error";
                    return View(alumno);
                }
            }
            catch (Exception algo)
            {
                ViewBag.Message = "Ha habido un error al ingresar al sistema. Pueden haber ocurrido los siguientes problemas: <br> <ul><li>El DNI es incorrecto.</li><li>La Clave es incorrecta.</li></ul>";
                ViewBag.TipoMensaje = "Error";
                return View(alumno);
            }
          
        }
        //
        // GET: /Alumnos/Delete/5
 
        public ActionResult Delete(int id)
        {
            Alumnos alumnos = db.Alumnos.Single(a => a.Id == id);
            return View(alumnos);
        }

        //
        // POST: /Alumnos/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Alumnos alumnos = db.Alumnos.Single(a => a.Id == id);
                db.Alumnos.DeleteObject(alumnos);
                db.SaveChanges();
                TempData["Message"] = "La información del alumno ha sido eliminada correctamente";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Message"] = "Error al eliminar el alumno";
                TempData["TipoMensaje"] = "Error";
                return RedirectToAction("Index");
            }
        }

        public ActionResult LogOut()
        {
            Session.Abandon();
            
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}