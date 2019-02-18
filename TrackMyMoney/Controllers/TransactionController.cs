using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackMyMoney.Models;
using Microsoft.AspNet.Identity;
using System.Net.Http;

namespace TrackMyMoney.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transaction
        // Accessing Summary of transactions of separate users.
        public ActionResult Summary()
        {
            var userId = User.Identity.GetUserId();
            var userAccount = db.UserAccounts.Where(c => c.ApplicationUserId == userId).First();
            ViewBag.Balance = userAccount.Balance;
            return View(userAccount.Transactions.ToList().OrderByDescending(x => x.Date).ThenBy(x => x.Time));
        }

        // Summary for a particular user picked up by Admin.
        [Authorize(Roles = "Admin")]
        public ActionResult SummaryForAdmin(int userId)
        {
            var userAccount = db.UserAccounts.Find(userId);
            ViewBag.Name = userAccount.FirstName + " " + userAccount.LastName;
            return View(userAccount.Transactions.ToList().OrderByDescending(x => x.Date).ThenBy(x => x.Time));
        }

        // GET: Transaction/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Transaction/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Transaction/Create
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

        public ActionResult AddIncome()
        {
            return View();
        }

        // POST: Transaction/Create
        // Adding income.
        [HttpPost]
        public ActionResult AddIncome(Transaction transaction)
        {
            try
            {
                // Accessing current userId
                var userId = User.Identity.GetUserId();
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("UserAccounts/GetUserAccountByAUId/" + userId).Result;
                var userAccountApi = response.Content.ReadAsAsync<UserAccount>().Result;
                var userAccount = db.UserAccounts.Where(c => c.ApplicationUserId == userId).First();
                transaction.UserAccountId = userAccountApi.Id;
                if ( ModelState.IsValid )
                {
                    // Updating the balance of the current user.
                    userAccountApi.Balance = userAccountApi.Balance + transaction.Amount;
                    userAccount.Balance = userAccount.Balance + transaction.Amount;
                    HttpResponseMessage newResponse = GlobalVariables.WebApiClient.PutAsJsonAsync("UserAccounts/" + userAccountApi.Id, userAccountApi).Result;
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    HttpResponseMessage newTransactionResponse = GlobalVariables.WebApiClient.PostAsJsonAsync("Transactions", new {
                        Amount = transaction.Amount,
                        From = transaction.From,
                        Description = transaction.Description,
                        Date = transaction.Date,
                        Time = transaction.Time,
                        UserAccountId = transaction.UserAccountId
                    }).Result;
                    return RedirectToAction("Summary");
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AddExpense()
        {
            return View();
        }

        // POST: Transaction/Create
        // Adding Expense.
        [HttpPost]
        public ActionResult AddExpense(Transaction transaction)
        {
            try
            {
                // Accessing the current user id.
                var userId = User.Identity.GetUserId();
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("UserAccounts/GetUserAccountByAUId/" + userId).Result;
                var userAccountApi = response.Content.ReadAsAsync<UserAccount>().Result;
                var userAccount = db.UserAccounts.Where(c => c.ApplicationUserId == userId).First();

                // Differentiating expense with a minus sign.
                transaction.Amount = -1 * transaction.Amount;
                transaction.UserAccountId = userAccountApi.Id;

                if (ModelState.IsValid)
                {
                    // Updating the balance of the current user.
                    userAccountApi.Balance = userAccountApi.Balance + transaction.Amount;
                    userAccount.Balance = userAccount.Balance + transaction.Amount;
                    HttpResponseMessage newResponse = GlobalVariables.WebApiClient.PutAsJsonAsync("UserAccounts/" + userAccountApi.Id, userAccountApi).Result;
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    HttpResponseMessage newTransactionResponse = GlobalVariables.WebApiClient.PostAsJsonAsync("Transactions", new
                    {
                        Amount = transaction.Amount,
                        From = transaction.From,
                        Description = transaction.Description,
                        Date = transaction.Date,
                        Time = transaction.Time,
                        UserAccountId = transaction.UserAccountId
                    }).Result;
                    return RedirectToAction("Summary");
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: Transaction/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Transaction/Edit/5
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

        // GET: Transaction/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // finding that particular transaction from the list.
                var transaction = db.Transactions.Find(id);
                var userId = User.Identity.GetUserId();
                var userAccount = db.UserAccounts.Where(c => c.ApplicationUserId == userId).First();
                HttpResponseMessage responseDeleteTransaction = GlobalVariables.WebApiClient.DeleteAsync("Transactions/" + id).Result;

                // Updating the balance of the current user.
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("UserAccounts/GetUserAccountByAUId/" + userId).Result;
                var userAccountApi = response.Content.ReadAsAsync<UserAccount>().Result;
                userAccountApi.Balance = userAccountApi.Balance - transaction.Amount;
                HttpResponseMessage responsePutUserAccount = GlobalVariables.WebApiClient.PutAsJsonAsync("UserAccounts/" + userAccountApi.Id, userAccountApi).Result;
                userAccount.Balance = userAccount.Balance - transaction.Amount;
                db.Transactions.Remove(transaction);
                db.SaveChanges();

            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Summary");
        }



        // POST: Transaction/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Summary");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteForAdmin(int id, int userId)
        {
            try
            {
                // Finding the selected transaction.
                var transaction = db.Transactions.Find(id);
                HttpResponseMessage responseDeleteTransaction = GlobalVariables.WebApiClient.DeleteAsync("Transactions/" + id).Result;
                var userAccount = db.UserAccounts.Find(userId);

                // updating the balance of the user that this transaction related to.
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("UserAccounts/" + userId).Result;
                var userAccountApi = response.Content.ReadAsAsync<UserAccount>().Result;
                userAccountApi.Balance = userAccountApi.Balance - transaction.Amount;
                HttpResponseMessage responsePutUserAccount = GlobalVariables.WebApiClient.PutAsJsonAsync("UserAccounts/" + userAccountApi.Id.ToString(), userAccountApi).Result;
                userAccount.Balance = userAccount.Balance - transaction.Amount;
                db.Transactions.Remove(transaction);
                db.SaveChanges();
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("SummaryForAdmin", new { userId = userId });
        }
    }
}
