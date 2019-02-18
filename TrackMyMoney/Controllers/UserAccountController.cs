using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using TrackMyMoney.Models;

namespace TrackMyMoney.Controllers
{
    [Authorize]
    public class UserAccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserAccount
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            // Displays the list of UserAccounts.
            IEnumerable<UserAccount> empList;
            HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("UserAccounts").Result;
            empList = response.Content.ReadAsAsync<IEnumerable<UserAccount>>().Result;
            return View(empList);
        }

        // GET: UserAccount/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserAccount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserAccount/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserAccount/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserAccount/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: UserAccount/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserAccount/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
