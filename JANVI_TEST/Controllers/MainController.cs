using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JANVI_TEST.EDM;
using System.Data.Entity;
using System.IO;

namespace JANVI_TEST.Controllers
{
    public class MainController : Controller
    {

        JP1Entities db = new JP1Entities();

        public void getcountry()
        {
            var c = db.Countries.ToList();
            List<SelectListItem> objlist = new List<SelectListItem>();
            foreach (var item in c)
            {
                SelectListItem obj = new SelectListItem();
                obj.Text = item.country_name;
                obj.Value = item.country_id.ToString();
                objlist.Add(obj);
            }
            ViewData["countrylist"] = objlist;
        }


        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin obj)
        {
            var aa = db.Admins.Where(c => c.admin_email == obj.admin_email && c.admin_password == obj.admin_password).ToList();
            var ee = db.Employees.Where(c => c.e_email == obj.admin_email && c.e_password == obj.admin_password).ToList();

            if (aa.Count == 1)
            {
                Session["admin_id"] = aa.FirstOrDefault().admin_id;
                Session["admin_email"] = aa.FirstOrDefault().admin_email;

                return RedirectToAction("Display");
            }

            else if (ee.Count == 1)
            {
                Session["e_id"] = ee.FirstOrDefault().e_id;
                Session["e_email"] = ee.FirstOrDefault().e_email;

                return RedirectToAction("Display");
            }
            else
            {
                ViewBag.msg = "Invalid username and password..";
                return View();
            }
        }

        public ActionResult Register()
        {
            getcountry();
            return View();
        }

        [HttpPost]
        public ActionResult Register(Employee obj,HttpPostedFileBase file)
        {
            var allowext = new[] { ".jpg", ".png", ".jpeg", ".Jpeg" };
            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName);
                if (allowext.Contains(ext))
                {
                    var filename = file.FileName;
                    var path = Path.Combine(Server.MapPath("~/Profile"), filename);
                    obj.e_profile = filename;
                    file.SaveAs(path);

                    db.Employees.Add(obj);
                    db.SaveChanges();
                    ViewBag.msg = "Record inserted..";
                }
                else
                {
                    ViewBag.msg = "Only images is uploaded..";
                    return View();
                }
            }
            ModelState.Clear();
            getcountry();
            return RedirectToAction("Login");
        }

        public ActionResult Display()
        {
            if (Session["admin_id"] != null)
            {
                var p = db.Employees.ToList();
                return View(p);
            }

            else if (Session["e_id"] != null)
            {
                var userid = int.Parse(Session["e_id"].ToString());
                var p = db.Employees.Where(c => c.e_id == userid).ToList();
                return View(p);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Edit(int id)
        {
            getcountry();
            var pp = db.Employees.Find(id);
            return View(pp);
        }

        [HttpPost]
        public ActionResult Edit(Employee obj,HttpPostedFileBase file)
        {
            var allowext = new[]
            { ".jpg", ".jpeg", ".png", ".Jpg", ".Jpeg" };
            if (file != null)
            {
                var ext = Path.GetExtension(file.FileName);
                if (allowext.Contains(ext))
                {
                    var filename = file.FileName;
                    var path = Path.Combine(Server.MapPath("~/Profile"), filename);
                    obj.e_profile = filename;
                    file.SaveAs(path);

                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    ViewBag.msg = "Only images are uploaded..";
                    return View();
                }

            }
            else
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
            ModelState.Clear();
            return RedirectToAction("Display");
        }

        public ActionResult Delete(int id)
        {
            db.Employees.Remove(db.Employees.Find(id));
            db.SaveChanges();
            return RedirectToAction("Display");
        }
    }
}