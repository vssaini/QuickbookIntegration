using Intuit.Ipp.OAuth2PlatformClient;
using QuickbookIntegrate.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace QuickbookIntegrate.Controllers
{
    public class HomeController : Controller
    {
        public static string ClientId = ConfigurationManager.AppSettings["clientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["clientSecret"];
        public static string RedirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
        public static string Environment = ConfigurationManager.AppSettings["appEnvironment"];

        public static OAuth2Client Auth2Client = new OAuth2Client(ClientId, ClientSecret, RedirectUrl, Environment);

        public ActionResult Index()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Session.Clear();
            Session.Abandon();
            Request.GetOwinContext().Authentication.SignOut("Cookies");
            return View();
        }

        /// <summary>
        /// Start Auth flow
        /// </summary>
        public ActionResult InitiateAuth()
        {
            var scopes = new List<OidcScopes> { OidcScopes.Accounting };
            string authorizeUrl = Auth2Client.GetAuthorizationURL(scopes);
            return Redirect(authorizeUrl);
        }

        /// <summary>
        /// QBO API Request
        /// </summary>
        public ActionResult ApiCallService()
        {
            if (Session["realmId"] == null) return View("ApiCallService", (object)"QBO API call Failed!");

            string realmId = Session["realmId"].ToString();
            try
            {
                if (User is ClaimsPrincipal principal)
                {
                    var serviceContext = QboHelper.GetServiceContext(principal, realmId);
                    QboHelper.AddCustomer(serviceContext);

                    var output = QboHelper.GetCompanyInfo(serviceContext);
                    if (output != null)
                    {
                        return View("ApiCallService", (object)("QBO API call Successful!! Response: " + output));
                    }
                }
            }
            catch (Exception ex)
            {
                return View("ApiCallService", (object)("QBO API call Failed!" + " Error message: " + ex.Message));
            }

            return View("ApiCallService", (object)"QBO API call Failed!");
        }

        public JsonResult AddCustomer(CustomerModel cModel)
        {
            object data = new
            {
                Status = false,
                Message = "RealmId not found in session"
            };

            if (Session["realmId"] == null) return Json(data, JsonRequestBehavior.AllowGet);

            string realmId = Session["realmId"].ToString();
            try
            {
                if (!(User is ClaimsPrincipal principal))
                {
                    throw new Exception("User principal is empty.");
                }

                var serviceContext = QboHelper.GetServiceContext(principal, realmId);
                var addedCustomer = QboHelper.AddCustomer(serviceContext, cModel);
                data = new
                {
                    Status = addedCustomer != null,
                    Message = addedCustomer == null ? "Failed to add customer" : "Customer was added successfully!"
                };
            }
            catch (Exception ex)
            {
                data = new
                {
                    Status = false,
                    Message = $"Failed to add customer. More details - {ex.Message}"
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Tokens()
        {
            return View("Tokens");
        }
    }
}