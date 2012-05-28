using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sao.Models;
using System.IO;

namespace Sao.Controllers
{ 
    public class CursosController : Controller
    {
        private SaoEntities db = new SaoEntities();

        //
        // GET: /Cursos/

        public ViewResult Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.TipoMensaje = TempData["TipoMensaje"];
            return View(db.Cursos.ToList());
        }

        //
        // GET: /Cursos/Details/5

        public ViewResult Details(int id)
        {
            Cursos cursos = db.Cursos.Single(c => c.Id == id);
            return View(cursos);
        }

        //
        // GET: /Cursos/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Cursos/Create

        [HttpPost]
        public ActionResult Create(Cursos cursos)
        {
            try
            {
                db.Cursos.AddObject(cursos);
                db.SaveChanges();
                TempData["Message"] = "Se ha creado el curso";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Error al crear curso";
                ViewBag.TipoMensaje = "Error";
                return View(cursos);
            }

        }
        
        //
        // GET: /Cursos/Edit/5
 
        public ActionResult Edit(int id)
        {
            Cursos cursos = db.Cursos.Single(c => c.Id == id);
            return View(cursos);
        }

        //
        // POST: /Cursos/Edit/5

        [HttpPost]
        public ActionResult Edit(Cursos cursos)
        {
            try
            {
                db.Cursos.Attach(cursos);
                db.ObjectStateManager.ChangeObjectState(cursos, EntityState.Modified);
                db.SaveChanges();
                TempData["Message"] = "Se ha modificado la info correctamente";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Error al editar curso";
                ViewBag.TipoMensaje = "Error";
                return View(cursos);
            }
        }

        //
        // GET: /Cursos/Delete/5
 
        public ActionResult Delete(int id)
        {
            Cursos cursos = db.Cursos.Single(c => c.Id == id);
            return View(cursos);
        }

        //
        // POST: /Cursos/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Cursos cursos = db.Cursos.Single(c => c.Id == id);
                db.Cursos.DeleteObject(cursos);
                db.SaveChanges();
                TempData["Message"] = "Se ha eliminado el curso";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Message"] = "Error al eliminar curso";
                TempData["TipoMensaje"] = "Error";
                return RedirectToAction("Index");
            }
        }
        public ActionResult Subir(int id)
        {
            Cursos cursos = db.Cursos.Single(c => c.Id == id);
            return View(cursos);
        }

        //
        // POST: /Cursos/Edit/5

        [HttpPost]
        public ActionResult Subir(Cursos cursos)
        {
            try
            {
                Adjuntos(cursos.Id);
                TempData["Message"] = "Archivos subidos correctamente";
                TempData["TipoMensaje"] = "Correcto";
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Error al editar curso";
                ViewBag.TipoMensaje = "Error";
                return View(cursos);
            }
        }

        //Archivos
        public void Adjuntos(int id)
        {
            string savedFileName = "";
            bool creo = false;
            var uploadpath = "";

            foreach (string file in Request.Files)
            {
                var hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.FileName != "")
                {
                    if (creo == false)
                    {
                        uploadpath = System.IO.Path.Combine(Server.MapPath("~/ArchivosCursos/"), id.ToString());
                        System.IO.Directory.CreateDirectory(uploadpath);
                        creo = true;
                    }
                    savedFileName = Path.Combine(uploadpath, Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName);
                }
            }
        }
         [OutputCache(Duration = 10, VaryByParam = "none")]
        public ViewResult Ver()
        {
            return View(db.Cursos.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
    }
}