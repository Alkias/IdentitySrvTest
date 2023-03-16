using System.Security.Claims;
using System.Web.Mvc;

namespace IdS3test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// Adding a protected resource and showing claims
        /// To initiate the authentication with IdentityServer you need
        /// to create a protected resource, e.g. by adding a global authorization filter.
        /// For our sample we will simply protect the About action on the Home controller.
        /// In addition we will hand over the claims to the view so we can see
        /// which claims got emitted by IdentityServer:
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View((User as ClaimsPrincipal).Claims);
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}