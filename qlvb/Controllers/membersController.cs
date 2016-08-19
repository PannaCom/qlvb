using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlvb.Models;
using System.Security.Cryptography;
using System.Collections;
using System.DirectoryServices.ActiveDirectory;

namespace qlvb.Controllers
{
    public class membersController : Controller
    {
        private qlvbEntities db = new qlvbEntities();

        //
        // GET: /members/

        public ActionResult Index()
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            return View(db.members.ToList());
        }

        //
        // GET: /members/Details/5

        public ActionResult Details(int id = 0)
        {
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // GET: /members/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /members/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(member member)
        {
            if (ModelState.IsValid)
            {
                MD5 md5Hash = MD5.Create();
                member.pass = Config.GetMd5Hash(md5Hash, member.pass);
                db.members.Add(member);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(member);
        }

        //
        // GET: /members/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // POST: /members/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(member member)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            if (ModelState.IsValid)
            {
                MD5 md5Hash = MD5.Create();
                member.pass = Config.GetMd5Hash(md5Hash, member.pass);
                db.Entry(member).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(member);
        }

        public ActionResult Login()
        {
            return View();
        }
        public string checkLogin(string name, string pass)
        {
            MD5 md5Hash = MD5.Create();
            pass = Config.GetMd5Hash(md5Hash, pass);
            var id = db.members.Where(o => o.name == name && o.pass == pass).FirstOrDefault();
            if (id != null)
            {
                Config.setCookie("logged", name);
                Config.setCookie("userid", id.id.ToString());
                return "1";
            }
            else { return "0"; }
        }
        public string checkLoged()
        {
            return Config.getCookie("userid");
        }
        public ActionResult Logout()
        {
            if (Request.Cookies["logged"] != null)
            {
                Response.Cookies["logged"].Expires = DateTime.Now.AddDays(-1);

            }
            if (Request.Cookies["userid"] != null)
            {
                Response.Cookies["userid"].Expires = DateTime.Now.AddDays(-1);
            }

            Session.Abandon();
            return View();
        }
        //
        // GET: /members/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            member member = db.members.Find(id);
            if (member == null)
            {
                return HttpNotFound();
            }
            return View(member);
        }

        //
        // POST: /members/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Config.getCookie("userid") == "") return RedirectToAction("Login", "members");
            member member = db.members.Find(id);
            db.members.Remove(member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public bool ValidateUser(string userName, string password)
        {
            bool validation;
            try
            {
                //LdapConnection ldc = new LdapConnection(new LdapDirectoryIdentifier((string)null, false, false));
                //NetworkCredential nc = new NetworkCredential(userName, password, "DOMAIN NAME HERE");
                //ldc.Credential = nc;
                //ldc.AuthType = AuthType.Negotiate;
                //ldc.Bind(nc); // user has authenticated at this point, as the credentials were used to login to the dc.
                //validation = true;
                ArrayList alDcs = new ArrayList();
                Domain domain = Domain.GetCurrentDomain();
                //foreach (DomainController dc in domain.DomainControllers)
                //{
                //    alDcs.Add(dc.Name);
                //}
                LdapAuthentication la = new LdapAuthentication(null);
                validation = la.IsAuthenticated("WORKGROUP", userName, password);
            }
            catch (Exception ex)
            {
                validation = false;
            }
            return validation;
        }
    }
}