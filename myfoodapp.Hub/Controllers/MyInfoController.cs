using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using myfoodapp.Hub.Models;
using System.Web.Security;

namespace myfoodapp.Hub.Controllers
{   
    [Authorize]
    public class MyInfoController : Controller
    {        
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
             
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Update()
        {
            var currentUser = this.User.Identity.GetUserName();
            var applicationUser = UserManager.FindByEmail(currentUser);
            return View(new UserViewModel() { Email = applicationUser.Email });
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(UserViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var currentUser = this.User.Identity.GetUserName();
                var applicationUser = UserManager.FindByEmail(currentUser);

                if(model.Email != applicationUser.Email)
                {
                    applicationUser.Email = model.Email;
                    applicationUser.UserName = model.Email;
                    var resultMailChanged = await UserManager.UpdateAsync(applicationUser);
                    if(!resultMailChanged.Succeeded)
                    {
                        AddErrors(resultMailChanged);
                        return View(model);
                    }
                }

                if(model.OldPassword != null && model.NewPassword != null)
                {
                    var resultPasswordChanged = await UserManager.ChangePasswordAsync(applicationUser.Id, model.OldPassword, model.NewPassword);
                    if (resultPasswordChanged.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(resultPasswordChanged);
                }
            }

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}