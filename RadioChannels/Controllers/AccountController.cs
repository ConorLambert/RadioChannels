using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Owin.Security;
using RadioChannels.DAL;
using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static RadioChannels.App_Start.IdentityConfig;

namespace RadioChannels.Controllers
{
    public class AccountController : Controller
    {
        private WebApiAccess access = new WebApiAccess();
        private RadioContext context;
        private ApplicationUserManager userManager;

        
        public ActionResult Index()
        {
            return PartialView();
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            setContextProperties();
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        public ActionResult Info()
        {
            return View();
        }

        


        // EXTERNAL LOGIN/REGISTER

        public ActionResult LoginGoogle()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, "Google");
            return new HttpUnauthorizedResult();
        }

        public ActionResult LoginFacebook()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, "Facebook");
            return new HttpUnauthorizedResult();
        }

        public ActionResult LoginTwitter()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, "Twitter");
            return new HttpUnauthorizedResult();
        }

        public ActionResult LoginExternal(string provider)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new Microsoft.Owin.Security.AuthenticationProperties
            {
                RedirectUri = "/Account/ExternalLinkLoginCallback"
            }, provider);
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> ExternalLinkLoginCallback()
        {
            setContextProperties();
            
            // Handle external Login Callback
            var loginInfo = await AuthenticationManagerExtensions.GetExternalLoginInfoAsync(HttpContext.GetOwinContext().Authentication);
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            // check if the user already exists                
            var lastNameClaim = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            var givenNameClaim = loginInfo.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);            
            var firstName = givenNameClaim.Value;
            var lastname = lastNameClaim.Value;

            var user = new User { UserName = loginInfo.Email, Email = loginInfo.Email, FirstName = firstName, LastName = lastname, EmailConfirmed = true};
            var result = await userManager.CreateAsync(user);
            if (result.Succeeded) // if not
            {
                result = await userManager.AddLoginAsync(user.Id, loginInfo.Login);
                if (result.Succeeded)
                {
                    await new SignInManager<User, string>(userManager, HttpContext.GetOwinContext().Authentication).ExternalSignInAsync(loginInfo, isPersistent: false);
                    return RedirectToAction("all", "favourites");
                } else {
                    ModelState.AddModelError("", "Invalid email or password");
                    return RedirectToAction("Register", ModelState);
                }
            }
            else  // else its a basic external login
            {
                await new SignInManager<User, string>(userManager, HttpContext.GetOwinContext().Authentication).ExternalSignInAsync(loginInfo, isPersistent: false);
                return RedirectToAction("all", "favourites");
            }
        }    


        // LOCAL LOGIN/REGISTER

        [HttpGet]
        public ActionResult LogIn(string returnUrl)
        {
            setContextProperties();
            var model = new User
            {
                ReturnUrl = returnUrl
            };

            var providers = HttpContext.GetOwinContext()
                .Authentication.GetAuthenticationTypes(x => !string.IsNullOrEmpty(x.Caption))
                .ToList();
            //model.AuthProviders - providers;

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult LogInFull(string returnUrl)
        {
            setContextProperties();
            var model = new User
            {
                ReturnUrl = returnUrl
            };

            var providers = HttpContext.GetOwinContext()
                .Authentication.GetAuthenticationTypes(x => !string.IsNullOrEmpty(x.Caption))
                .ToList();
            //model.AuthProviders - providers;

            return View("LogIn", model);
        }

        [HttpPost]
        public async Task<ActionResult> LogInFull(User model)
        {
            return await LogIn(model);
        }

        [HttpPost]
        public async Task<ActionResult> LogIn(User model)
        {
            setContextProperties();
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await userManager.FindAsync(model.Email, model.PasswordHash); 
            if (user != null)
            {
                if (!await userManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");

                    // Uncomment to debug locally  
                    ViewBag.Link = callbackUrl;
                    ViewBag.errorMessage = "You must have a confirmed email to log on. "
                                         + "The confirmation token has been resent to your email account.";
                    return View("Error");
                } else {
                    await SignIn(user);
                    return Redirect(Url.Action("all", "favourites")); // GetRedirectUrl(model.ReturnUrl));
                }
            }

            // user authN failed
            ModelState.AddModelError("", "Invalid email or password");
            return View("LogIn");
        }

        [HttpGet]
        public ActionResult Register()
        {
            setContextProperties();
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> Register(User model)
        {
            setContextProperties();
            if (!ModelState.IsValid)
            {
                return View();
            }

            model.UserName = model.Email;
            var result = await userManager.CreateAsync(model, model.PasswordHash); // var result = await userManager.CreateAsync(model, model.PasswordHash);

            if (result.Succeeded)
            {
                string code = await userManager.GenerateEmailConfirmationTokenAsync(model.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = model.Id, code = code }, protocol: Request.Url.Scheme);
                string callbackUrl = await SendEmailConfirmationTokenAsync(model.Id, "Confirm your account");

               //  await userManager.SendEmailAsync(model.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                         + "before you can log in.";

                // ViewBag.Link = callbackUrl; local debug only

                return View("Info");

                // await SignIn(model);
                // return RedirectToAction("index", "favourites");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View();
        }        
        
        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("index", "home");
        }

        private async Task SignIn(User user)
        {
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            GetAuthenticationManager().SignIn(identity);
        }


        // PASSWORD RECOVERY

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            setContextProperties();

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                string code = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await userManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            setContextProperties();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        // EDIT AND DELETE ACCOUNT
        [HttpGet]
        public ActionResult Account()
        {
            setContextProperties();
            var user = userManager.FindById(User.Identity.GetUserId());
            return PartialView(user);
        }

        [HttpGet]
        public ActionResult AccountFull()
        {
            setContextProperties();
            var user = userManager.FindById(User.Identity.GetUserId());
            return View("Account", user);
        }

        [HttpPost]
        public ActionResult Delete(User user)
        {
            setContextProperties();
            var current_user = userManager.FindById(User.Identity.GetUserId());

            context.Favourite.RemoveRange(context.Favourite.Where(x => x.UserId == current_user.Id));
            context.SaveChanges();

            context.Users.Remove(current_user);
            context.SaveChanges();

            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");

            ViewBag.Message = "Account Successfully Deleted";
            return RedirectToAction("Index", "Home");            
        }

        [HttpGet]
        public ActionResult Edit()
        {
            setContextProperties();
            var user = userManager.FindById(User.Identity.GetUserId());
            return PartialView(user);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                setContextProperties();
                var current_user = userManager.FindById(User.Identity.GetUserId());

                current_user.FirstName = user.FirstName;
                current_user.LastName = user.LastName;
                var send_email_conf = false;
                if (current_user.Email != user.Email)
                {
                    current_user.Email = user.Email;
                    current_user.UserName = user.Email;
                    current_user.EmailConfirmed = false;
                    send_email_conf = true;
                }

                var result = await userManager.UpdateAsync(current_user);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View();
                }

                if (send_email_conf)
                {
                    string code = await userManager.GenerateEmailConfirmationTokenAsync(current_user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = model.Id, code = code }, protocol: Request.Url.Scheme);
                    string callbackUrl = await SendEmailConfirmationTokenAsync(current_user.Id, "Confirm your account");

                    //  await userManager.SendEmailAsync(model.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    ViewBag.Message = "Check your email and confirm your account, you must be confirmed "
                             + "before you can log in.";

                    // sign the user out and inform them to check their email
                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignOut("ApplicationCookie");

                    // ViewBag.Link = callbackUrl; local debug only
                    return View("Info");
                }

                return RedirectToAction("AccountFull", "Account");
            }

            ViewBag.message = "Something went wrong upating account information";
            return View();
        }

        [HttpGet]
        public ActionResult EditPassword()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> EditPassword(EditPassword editPassword)
        {
            setContextProperties();
            User user = userManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                ViewBag.Message = "No logged in user was found";
                return View();
            }
            user.PasswordHash = userManager.PasswordHasher.HashPassword(editPassword.Password);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                //throw exception......
                AddErrors(result);
                return View();
            }
            ViewBag.message = "Password changed successfully";            
            return View("Account", userManager.FindById(User.Identity.GetUserId()));
            // return RedirectToAction("AccountFull", "Account");
        }



        // HELPER METHODS

        public void setContextProperties()
        {
            this.context = HttpContext.GetOwinContext().Get<RadioContext>();
            this.userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private IAuthenticationManager GetAuthenticationManager()
        {
            var ctx = Request.GetOwinContext();
            return ctx.Authentication;
        }

        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("index", "home");
            }

            return returnUrl;
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await userManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await userManager.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && userManager != null)
            {
                userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}