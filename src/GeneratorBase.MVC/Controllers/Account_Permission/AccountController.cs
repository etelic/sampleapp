using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GeneratorBase.MVC.Models;
using PagedList;
using System.Web.Security;
using System.Configuration;
using Microsoft.Web.WebPages.OAuth;
using System.Web.Routing;
using System.Web.WebPages;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using Microsoft.AspNet.Identity.Owin;
namespace GeneratorBase.MVC.Controllers
{
    [Authorize]
    public class AccountController : IdentityBaseController
    {
		/// <summary>
        /// captchastring propert rendom ganrated 
        /// </summary>
        public static string captchastring
        {
            get;
            set;
        }
        public AccountController()
        {
        }
        public AccountController(UserManager<ApplicationUser> userManager)
        {
        }
		public static string UrlforForgotpassword { get; private set; }
		public RedirectResult SwitchView(bool mobile, string returnUrl)
        {
            if (Request.Browser.IsMobileDevice == mobile)
            {
                HttpContext.ClearOverriddenBrowser();
            }
            else
            {
                HttpContext.SetOverriddenBrowser(mobile ? BrowserOverride.Mobile : BrowserOverride.Desktop);
            }
            return Redirect(returnUrl);
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl,string ThirdPartyLoginError)
        {
			if (!Request.IsAjaxRequest())
                UrlforForgotpassword = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            ViewBag.ReturnUrl = returnUrl;
			if (User != null && User.Identity is System.Security.Principal.WindowsIdentity)
                return RedirectToAction("Index", "Home");
            ViewData["ThirdPartyLoginError"] = ThirdPartyLoginError;
            return View();
        }
		[HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ApplicationContext db = new ApplicationContext(new SystemUser());
            string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;

            if (GeneratorBase.MVC.Models.CommonFunction.Instance.NeedSharedUserSystem() == "yes")
            {
                if (ModelState.IsValid)
                {
                    var Db = new AuthenticationDbContext();
                    var ssoUsers = Db.Users.Where(p => p.UserName == model.UserName).ToList();
                    if (ssoUsers != null && ssoUsers.Count > 0)
                    {
                        var user = ssoUsers[0];
                        var roles = user.Roles;
                        PasswordHasher ph = new PasswordHasher();
                        string hashedpwd = ph.HashPassword(model.Password);
                        var result = ph.VerifyHashedPassword(user.PasswordHash, model.Password);
                        if (result.ToString() == "Success")
                        {
                           // ApplicationDbContext localAppDB = new ApplicationDbContext();
                            var localAppUser = Identitydb.Users.Where(p => p.UserName == model.UserName).ToList();
                            if (localAppUser != null && localAppUser.Count > 0)
                            {
                                await SignInAsync(localAppUser[0], isPersistent: false);
                                return RedirectToLocal(returnUrl);
                            }
                            else
                            {
                                var newUserInLocalDB = new ApplicationUser()
                                {
                                    UserName = user.UserName,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Email = user.Email
                                };
                                var result1 = await UserManager.CreateAsync(newUserInLocalDB);
                                if (result1.Succeeded)
                                {
                                    await SignInAsync(newUserInLocalDB, isPersistent: false);
                                    return RedirectToLocal(returnUrl);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid username or password.");
                        }
                    }
                    else
                    {
                        var localDBUser = await UserManager.FindAsync(model.UserName, model.Password);
                        if (localDBUser != null)
                        {
                            await SignInAsync(localDBUser, isPersistent: false);
                            return RedirectToLocal(returnUrl);
                        }
                        else
                            ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindAsync(model.UserName, model.Password);
                    if (user != null)
                    {
                        if ((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                        {
                            if (await UserManager.IsLockedOutAsync(user.Id))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
	                            return View(model);
                            }
                            else
                            {
                                await SignInAsync(user, model.RememberMe);
                                // When token is verified correctly, clear the access failed count used for lockout
                                await UserManager.ResetAccessFailedCountAsync(user.Id);
                            }
                        }
                        else if (await UserManager.IsLockedOutAsync(user.Id))
                        {
                            ModelState.AddModelError("", string.Format("Your account has been locked. Please contact your administrator.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                            return View(model);
                        }
                        else
                        {
                            await SignInAsync(user, model.RememberMe);
                        }

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        var user1 = UserManager.FindByName(model.UserName);
                        if (user1 != null)
                        {
                            if ((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                            {
                                if (await UserManager.IsLockedOutAsync(user1.Id))
                                {
                                    ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                }
                                // if user is subject to lockouts and the credentials are invalid  // record the failure and check if user is lockedout and display message, otherwise,
                                // display the number of attempts remaining before lockout
                                else if (await UserManager.GetLockoutEnabledAsync(user1.Id))
                                {
                                    // Record the failure which also may cause the user to be locked out
                                    await UserManager.AccessFailedAsync(user1.Id);
                                    string message;
                                    if (await UserManager.IsLockedOutAsync(user1.Id))
                                        message = string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value);
                                    else
                                    {
                                        int accessFailedCount = await UserManager.GetAccessFailedCountAsync(user1.Id);
                                        int attemptsLeft = Convert.ToInt32(db.AppSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value) - accessFailedCount;
                                        message = string.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);
                                    }
                                    ModelState.AddModelError("", message);
                                }
                                else
                                    ModelState.AddModelError("", "Invalid username or password.");
                            }
                            else
                            {
                                if (await UserManager.IsLockedOutAsync(user1.Id))
                                    ModelState.AddModelError("", string.Format("Your account has been locked out. Please contact your administrator.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                else
                                    ModelState.AddModelError("", "Invalid username or password.");
                            }
                        }
                        else
                            ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
            }
            if (Request.AcceptTypes.Contains("text/html"))
            {
                return View(model);
            }
            else if (Request.AcceptTypes.Contains("application/json"))
            {
                return RedirectToLocal(returnUrl);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
		
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            if (((CustomPrincipal)User).CanAddAdminFeature("User"))
            {
                var role = Identitydb.Roles;
                ViewBag.RoleList = role.ToList().OrderBy(p => p.Name);
                return View();
            }
            else
                return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

		        /// <summary>
        /// Generate Captcha method for show Captcha image on Register page   
        /// </summary>
        [AllowAnonymous]
        public void generateCaptcha()
        {
            CaptchaImage cap = new CaptchaImage();
            string randomString = cap.CreateRandomText(6);
            Bitmap bitmap = new Bitmap(200, 40, PixelFormat.Format32bppArgb);
            Random random = new Random(System.DateTime.Now.Millisecond);
            // Create a graphics object for drawing.
            Graphics g = Graphics.FromImage(bitmap);
            g.PageUnit = GraphicsUnit.Pixel;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, 200, 40);
            // Fill in the background.
            HatchBrush hatchBrush = new HatchBrush(HatchStyle.Shingle, Color.LightGray, Color.White);
            g.FillRectangle(hatchBrush, rect);
            // Set up the text font.
            SizeF size;
            float fontSize = rect.Height + 1;
            Font font;
            // Adjust the font size until the text fits within the image.
            do
            {
                fontSize--;
                font = new Font("Arial", fontSize, GraphicsUnit.Pixel);
                size = g.MeasureString(randomString, font);
            } while (size.Width > rect.Width);
            // Set up the text format.
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            // Create a path using the text and warp it randomly.
            GraphicsPath path = new GraphicsPath();
            path.AddString(randomString, font.FontFamily, (int)font.Style, font.Size, rect, format);
            float v = 4F;
            PointF[] points =
			{
				new PointF(random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				new PointF(rect.Width - random.Next(rect.Width) / v, random.Next(rect.Height) / v),
				new PointF(random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v),
				new PointF(rect.Width - random.Next(rect.Width) / v, rect.Height - random.Next(rect.Height) / v)
			};
            Matrix matrix = new Matrix();
            matrix.Translate(0F, 0F);
            path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
            // Draw the text.
            hatchBrush = new HatchBrush(HatchStyle.Shingle, Color.LightGray, Color.DarkGray);
            g.FillPath(hatchBrush, path);
            // Add some random noise.
            int m = Math.Max(rect.Width, rect.Height);
            for (int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
            {
                int x = random.Next(rect.Width);
                int y = random.Next(rect.Height);
                int w = random.Next(m / 50);
                int h = random.Next(m / 50);
                g.FillEllipse(hatchBrush, x, y, w, h);
            }
            captchastring = randomString;
            //this.ControllerContext.Controller.ViewBag.captchastring = randomString;
            HttpResponseBase response = this.ControllerContext.HttpContext.Response;
            response.ContentType = "image/jpeg";
            bitmap.Save(response.OutputStream, ImageFormat.Jpeg);
            bitmap.Dispose();
            // Clean up.
            font.Dispose();
            hatchBrush.Dispose();
            g.Dispose();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string CaptchaText)
        {
            if (ModelState.IsValid)
            {
                var User = await UserManager.FindByEmailAsync(model.Email);
                if (User != null)
                {
                    return Json("emailExist", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                var user = model.GetUser();
                var userExist = await UserManager.FindByNameAsync(user.UserName);
                if (userExist == null)
                {
                    if (CaptchaText == captchastring)
                    {
                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            AssignDefaultRoleToNewUser(user.Id);
                            var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
                            SendEmail sendEmail = new SendEmail();
                            //var Db = new ApplicationDbContext();
                            var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                            if (EmailTemplate != null)
                            {
                                string mailbody = string.Empty;
                                string mailsubject = string.Empty;
                                if (!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                                {
                                    mailbody = EmailTemplate.EmailContent;
                                    mailbody = mailbody.Replace("###FullName###", model.FirstName + " " + model.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + appURL + "'>here</a>");
                                }

                                mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " :Your registration is successful!" : EmailTemplate.EmailSubject; ;

                                sendEmail.Notify(model.Email, mailbody, mailsubject);
                            }
                            return Json("Ok", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            string strError = "";
                            var lstError = result.Errors;
                            foreach (var errormsg in lstError)
                            {
                                foreach (var msg in errormsg.Split('.'))
                                {
                                    strError += msg + ".\r\n";
                                }
                            }
                            return Json(strError, "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("Captchaverification", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("UserExist", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AssignDefaultRoleToNewUser(string userId)
        {
            AdminSettingsRepository _adminSettings = new AdminSettingsRepository();
            IdentityManager idManager = new IdentityManager();
            if (!string.IsNullOrEmpty(_adminSettings.GetDefaultRoleForNewUser()) && _adminSettings.GetDefaultRoleForNewUser().ToUpper() != "NONE")
                idManager.AddUserToRole(LogggedInUser, userId, _adminSettings.GetDefaultRoleForNewUser());
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterUser(RegisterViewModel model, string selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var User = await UserManager.FindByEmailAsync(model.Email);
                string emailerror = "";
                if (User != null)
                {
                    emailerror = "E-mail is already taken";
                    var errors = new List<string>();
                    errors.Add(emailerror);
                    return Json(errors);
                }
                var user = model.GetUser();
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(selectedRoles))
                    {
                        selectedRoles = selectedRoles.TrimEnd(',');
                        var lstRoles = selectedRoles.Split(',');
                        //var Db = new ApplicationDbContext();
                        var currentUser = Identitydb.Users.First(u => u.UserName == model.UserName);
                        if (currentUser != null)
                        {
                            var idManager = new IdentityManager();
                            idManager.ClearUserRoles(LogggedInUser, currentUser.Id);
                            foreach (var role in lstRoles)
                            {
                                idManager.AddUserToRole(LogggedInUser, user.Id, role);
                            }
                        }
                    }
                    return Json(new { success = true });
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error + "\r\n");
                    }
                    return Json(errors);
                }
            }
            else
            {
                var errors = new List<string>();
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
                }
                return Json(errors);
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword()
        {
            return View();
        }
       [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model, bool? IsAddPop)
        {
            if (ModelState.IsValid)
            {
                var appURL = UrlforForgotpassword;
                SendEmail sendEmail = new SendEmail();
                //var Db = new ApplicationDbContext();
                ApplicationContext db = new ApplicationContext(new SystemUser());
                ApplicationUser User = new ApplicationUser();

                if (model.Username != null)
                    User = await UserManager.FindByNameAsync(model.Username);
                else
                    User = await UserManager.FindByEmailAsync(model.Email);

                if (User != null)
                {
                    int accessFailedCount = User.AccessFailedCount;
                    int attempts = Convert.ToInt32(db.AppSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value);
                    if (accessFailedCount >= attempts)
                        return Json("UserLoginAttempt", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                    if (User.LockoutEndDateUtc != null)
                        return Json("UserIslocked", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);

                    var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Forgot Password");
                    if (EmailTemplate != null)
                    {
                        string mailbody = string.Empty;
                        string mailsubject = string.Empty;
                        if (!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                        {
                            mailbody = EmailTemplate.EmailContent;
                            try
                            {
                                var code = await UserManager.GenerateEmailConfirmationTokenAsync(User.Id);
                                var callbackUrl = Url.Action("ResetPassword", "Account",
                                new { UserId = User.Id, code = System.Web.HttpUtility.UrlEncode(code) }, protocol: Request.Url.Scheme);
                                mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###Username###", User.UserName).Replace("###Password###", "").Replace("###URL###", " <a href='" + callbackUrl + "'>here</a>").Replace("###AppName###", CommonFunction.Instance.AppName());
                            }
                            catch
                            {
                            }
                        }
                        mailsubject = string.IsNullOrEmpty(EmailTemplate.EmailSubject) ? CommonFunction.Instance.AppName() + " :Your password changed successfully!" : EmailTemplate.EmailSubject; ;
                        sendEmail.Notify(User.Email, mailbody, mailsubject);
                    }
                    return Json("Ok", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("UserNotExist", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            IdentityResult result;
            try
            {
                result = await UserManager.ConfirmEmailAsync(userId, HttpUtility.UrlDecode(code));
            }
            catch (InvalidOperationException ioe)
            {
                // ConfirmEmailAsync throws when the userId is not found.
                ViewBag.errorMessage = ioe.Message;
                return View("Error");
            }

            if (result.Succeeded)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(userId);
                return RedirectToAction("Manage", new { token = token, provider = userId });
            }
            // If we got this far, something failed.
            AddErrors(result);
            ViewBag.errorMessage = "Authentication failed";
            return RedirectToAction("ActivationLink", "Error");
        }
        [AllowAnonymous]
        public ActionResult CreateRole()
        {
            if (((CustomPrincipal)User).CanAddAdminFeature("Role"))
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
         [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(CreateRoleViewModel model)
        {
            model.Name = model.Name.Trim();
            if (((CustomPrincipal)User).CanAddAdminFeature("Role"))
                if (ModelState.IsValid)
                {
                    var role = model.GetRole();
                    var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
                    // Create Admin Role
                    string roleName = model.Name;
                    IdentityResult roleResult;
                    roleResult = RoleManager.Create(new IdentityRole(roleName));
                    if (roleResult.Succeeded)
                    {
						DoAuditEntry.AddJournalEntryCommon(LogggedInUser, Identitydb, roleName, "IdentityRole");
                        return Json(new { success = true });
                    }
                    else
                    {
                        var errors = new List<string>();
                        foreach (var error in roleResult.Errors)
                        {
                            errors.Add(error);
                        }
                        return Json(errors);
                    }
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var modelState in ViewData.ModelState.Values)
                    {
                        errors.AddRange(modelState.Errors.Select(error => error.ErrorMessage));
                    }
                    return Json(errors);
                }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Manage(ManageMessageId? message, string token, string provider)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            ViewBag.token = token;
            ViewBag.provider = provider;
            if (!string.IsNullOrEmpty(token))
                ViewBag.HasLocalPassword = true;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword || !string.IsNullOrEmpty(model.token))
            {
                if (ModelState.IsValid)
                {
                    ApplicationContext db = new ApplicationContext(new SystemUser());
                    var appSettings = db.AppSettings;
                    string applySecurityPolicy = appSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
                    if (applySecurityPolicy.ToLower() == "yes")
                    {
                        var pwdcount = Convert.ToInt32(appSettings.Where(p => p.Key == "OldPasswordGenerationCount").FirstOrDefault().Value);
                        if (string.IsNullOrEmpty(model.token))
                        {
                            if (isPreviousUsedPassword(model, pwdcount, User.Identity.GetUserId()))
                                return View(model);
                        }
                        else
                        {
                            if (isPreviousUsedPassword(model, pwdcount, model.provider))
                            {
                                ViewBag.HasLocalPassword = true;
                                ViewBag.token = model.token;
                                ViewBag.provider = model.provider;
                                return View(model);
                            }
                        }
                    }
                    //IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (string.IsNullOrEmpty(model.token))
                    {
                        IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

                        if (result.Succeeded)
                        {
                            SavePasswordHistory(User.Identity.GetUserId());
                            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                    else
                    {
                        ViewBag.HasLocalPassword = true;
                        ViewBag.token = model.token;
                        ViewBag.provider = model.provider;
                        IdentityResult result = await UserManager.ResetPasswordAsync(model.provider, model.token, model.NewPassword);
                        if (result.Succeeded)
                        {
                            SavePasswordHistory(model.provider);
                            return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess, token = model.token, provider = model.provider });
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public void SavePasswordHistory(string userID)
        {
            PasswordHistory history = new PasswordHistory();
            var user = UserManager.FindById(userID);
            history.Date = DateTime.Now;
            history.UserId = user.Id;
            history.HashedPassword = user.PasswordHash;
            Identitydb.PasswordHistorys.Add(history);
            Identitydb.SaveChanges();
        }
        public bool isPreviousUsedPassword(ManageUserViewModel model, int pwdcount, string userid)
        {
            if (string.IsNullOrEmpty(userid)) return false;
            ApplicationDbContext appDb = new ApplicationDbContext(true);
            var user = appDb.Users.Find(userid); //UserManager.FindById(userid);
            if (user == null) return false;
            var lstpwdhistory = appDb.PasswordHistorys.Where(p => p.UserId == user.Id).OrderByDescending(p => p.Date).Take(pwdcount).ToList();
            foreach (var pwd in lstpwdhistory)
            {
                var result = UserManager.PasswordHasher.VerifyHashedPassword(pwd.HashedPassword, model.NewPassword);
                if (result == PasswordVerificationResult.Success)
                {
                    ModelState.AddModelError("", "You can't use password from last 24 passwords.");
                    return true;
                }
            }
            return false;
        }
		public ActionResult LogOff(string UrlReferrer)
        {
			return RedirectToAction("Index", "Home");
			RemoveMultitenant(((CustomPrincipal)User).Name);
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
			RemoveMultitenant(((CustomPrincipal)User).Name);
			string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
			 HttpCookie userrole = new HttpCookie(AppUrl+"CurrentRole");
            HttpCookie userpage = new HttpCookie("PageId");
            userpage.Expires = DateTime.Now.AddDays(-1);
            userrole.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userpage);
            Response.Cookies.Add(userrole);
			Response.Cookies["fltCookie"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["fltCookieFltTabId"].Expires = DateTime.Now.AddDays(-1);
			Response.Cookies.Clear();
            Session.Clear();
			Response.Cookies[AppUrl + "CurrentRole"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["PageId"].Expires = DateTime.Now.AddDays(-1);
            //string[] myCookies = Request.Cookies.AllKeys;
            //foreach (string cookie in myCookies)
            //{
                //Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            //}
            return RedirectToAction("Index", "Home");
        }

		[HttpGet]
        public ActionResult BrowserClose()
        {
            AuthenticationManager.SignOut();
			RemoveMultitenant(((CustomPrincipal)User).Name);
			string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            HttpCookie userrole = new HttpCookie(AppUrl+"CurrentRole");
            HttpCookie userpage = new HttpCookie("PageId");
            userpage.Expires = DateTime.Now.AddDays(-1);
            userrole.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userpage);
            Response.Cookies.Add(userrole);
			Response.Cookies.Clear();
            Session.Clear();
			Response.Cookies[AppUrl + "CurrentRole"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["PageId"].Expires = DateTime.Now.AddDays(-1);
            //string[] myCookies = Request.Cookies.AllKeys;
            //foreach (string cookie in myCookies)
            //{
                //Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            //}
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? IsExport, bool? RenderPartial)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("User"))
                return RedirectToAction("Index", "Home");
            if (string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
            {
                isAsc = "ASC";
            }
            ViewBag.isAsc = isAsc;
            ViewBag.CurrentSort = sortBy;
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;
            List<ApplicationUser> users = null;
            users = Identitydb.Users.ToList();
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            var _model = from s in model
                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                _model = searchRecords(_model.AsQueryable(), searchString.ToUpper());
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                _model = sortRecords(_model.AsQueryable(), sortBy, isAsc);
            }
            else _model = _model.OrderByDescending(c => c.Id);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "Account"].Value);
                ViewBag.Pages = pageNumber;
            }
            //
            //

            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(_model.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("IndexPartial", _model.ToPagedList(pageNumber, pageSize));
        }
        private IQueryable<EditUserViewModel> searchRecords(IQueryable<EditUserViewModel> users, string searchString)
        {
            searchString = searchString.Trim();
            users = users.Where(s => (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.Email) && s.Email.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.FirstName) && s.FirstName.ToUpper().Contains(searchString)) || (!String.IsNullOrEmpty(s.LastName) && s.LastName.ToUpper().Contains(searchString)));
            return users;
        }
        private IQueryable<EditUserViewModel> sortRecords(IQueryable<EditUserViewModel> users, string sortBy, string isAsc)
        {
            string methodName = "";
            switch (isAsc.ToLower())
            {
                case "asc":
                    methodName = "OrderBy";
                    break;
                case "desc":
                    methodName = "OrderByDescending";
                    break;
            }
            ParameterExpression paramExpression = Expression.Parameter(typeof(EditUserViewModel), "ApplicationUser");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<EditUserViewModel>)users.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { users.ElementType, lambda.Body.Type },
                    users.Expression,
                    lambda));
        }
        [AllowAnonymous]
        public ActionResult RoleList()
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("Role"))
                return RedirectToAction("Index", "Home");
            // var Db = new ApplicationDbContext();
            var roles = Identitydb.Roles;
            var model = new List<EditRoleViewModel>();
            foreach (var role in roles)
            {
                var u = new EditRoleViewModel(role);
                model.Add(u);
            }
            AdminSettingsRepository _adminSettings = new AdminSettingsRepository();
            ViewBag.DefaultRoleForNewUser = _adminSettings.GetDefaultRoleForNewUser();
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("User"))
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("User"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                //  var Db = new ApplicationDbContext();
                var userInfo = Identitydb.Users;
                //var username = user.First(p => p.UserName == model.UserName);
                var userEmail = userInfo.FirstOrDefault(p => p.Email == model.Email);
                if (userEmail != null)
                {
                    var user = userInfo.First(p => p.UserName == model.UserName);
                    if (user.FirstName != model.FirstName || user.LastName != model.LastName)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        await Identitydb.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    else if (user.Email == userEmail.Email)
                    {
                        return RedirectToAction("Index");

                    }
                    //if (user.Email == model.Email)
                    //    return RedirectToAction("Index");
                    ViewBag.DuplicacyMessage = "E-Mail already exist. Please try another one.";
                    return View(model);
                }
                else
                {
                    var user = userInfo.First(p => p.UserName == model.UserName);
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    await Identitydb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult LockUnlockUser(string id, string lockuser)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("User"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                //var Db = new ApplicationDbContext();
                var user = Identitydb.Users.First(u => u.Id == id);

                if (Convert.ToBoolean(lockuser))
                {
                    user.LockoutEnabled = true;
                    user.LockoutEndDateUtc = DateTime.UtcNow.AddYears(100);
                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEndDateUtc = null;
                }


                Identitydb.Entry(user).State = System.Data.Entity.EntityState.Modified;
                Identitydb.SaveChanges();
                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult EditRole(string id, ManageMessageId? Message = null)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("Role"))
                return RedirectToAction("Index", "Home");
            var roles = Identitydb.Roles.First(u => u.Id == id);
            var model = new EditRoleViewModel(roles);
            ViewBag.MessageId = Message;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(EditRoleViewModel model)
        {
            model.Name = model.Name.Trim();
            model.OriginalName = model.OriginalName.Trim();
            if (!((CustomPrincipal)User).CanEditAdminFeature("Role"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                //var Db = new ApplicationDbContext();
                if (Identitydb.Roles.Where(u => u.Name == model.Name).Count() <= 0)
                {
                    var roles = Identitydb.Roles.First(u => u.Name == model.OriginalName);
                    roles.Name = model.Name;
                    Identitydb.Entry(roles).State = System.Data.Entity.EntityState.Modified;
                    await Identitydb.SaveChangesAsync();
                    PermissionContext db = new PermissionContext();
                    List<Permission> lstprm = db.Permissions.Where(q => q.RoleName == model.OriginalName).ToList();
                    lstprm.ForEach(p => p.RoleName = model.Name);
                    db.SaveChanges();
                    UserDefinePagesRoleContext dbUserPages = new UserDefinePagesRoleContext();
                    List<UserDefinePagesRole> lstUserPagesprm = dbUserPages.UserDefinePagesRoles.Where(q => q.RoleName == model.OriginalName).ToList();
                    lstUserPagesprm.ForEach(p => p.RoleName = model.Name);
                    dbUserPages.SaveChanges();
                    AdminSettingsRepository _adminSettingsRepository = new AdminSettingsRepository();
                    if (_adminSettingsRepository.GetDefaultRoleForNewUser() == model.OriginalName)
                    {
                        AdminSettings adminSettings = new AdminSettings();
                        adminSettings.DefaultRoleForNewUser = model.Name;
                        _adminSettingsRepository.EditAdminSettings(adminSettings);
                    }
                    return RedirectToAction("RoleList");
                }
                else
                    ModelState.AddModelError("", "Name " + model.Name + " is already taken");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Delete(string id = null)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("User"))
                return RedirectToAction("Index", "Home");
            // var Db = new ApplicationDbContext();
            var user = Identitydb.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(string id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("User"))
                return RedirectToAction("Index", "Home");
            var user = Identitydb.Users.First(u => u.Id == id);
            Identitydb.Users.Remove(user);
            Identitydb.SaveChanges();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult DeleteRole(string id = null)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("Role"))
                return RedirectToAction("Index", "Home");
            // var Db = new ApplicationDbContext();
            var roles = Identitydb.Roles.First(u => u.Id == id);
            var model = new EditRoleViewModel(roles);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult DeleteRoleConfirmed(string id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("Role"))
                return RedirectToAction("Index", "Home");
            var roles = Identitydb.Roles.First(u => u.Id == id);
            Identitydb.Roles.Remove(roles);
            Identitydb.SaveChanges();
            PermissionContext db = new PermissionContext();
            List<Permission> lstprm = db.Permissions.Where(q => q.RoleName == id).ToList();
            db.Permissions.RemoveRange(lstprm);
            db.SaveChanges();
            UserDefinePagesRoleContext dbUserPages = new UserDefinePagesRoleContext();
            List<UserDefinePagesRole> lstUserPagesprm = dbUserPages.UserDefinePagesRoles.Where(q => q.RoleName == id).ToList();
            dbUserPages.UserDefinePagesRoles.RemoveRange(lstUserPagesprm);
            dbUserPages.SaveChanges();
            AdminSettingsRepository _adminSettingsRepository = new AdminSettingsRepository();
            if (_adminSettingsRepository.GetDefaultRoleForNewUser() == roles.Name)
            {
                AdminSettings adminSettings = new AdminSettings();
                adminSettings.DefaultRoleForNewUser = "None";
                _adminSettingsRepository.EditAdminSettings(adminSettings);
            }
            return RedirectToAction("Rolelist");
        }
        [AllowAnonymous]
        public ActionResult UserRoles(string id)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("AssignUserRole"))
                return RedirectToAction("Index", "Home");
            var user = Identitydb.Users.First(u => u.Id == id);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("AssignUserRole"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                //    var Db = new ApplicationDbContext();
                var user = Identitydb.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(LogggedInUser, user.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(LogggedInUser, user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult UsersInRole(string id, string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? IsExport, bool? RenderPartial)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("AssignUserRole"))
                return RedirectToAction("Index", "Home");


            if (string.IsNullOrEmpty(isAsc) && !string.IsNullOrEmpty(sortBy))
            {
                isAsc = "ASC";
            }
            ViewBag.isAsc = isAsc;
            ViewBag.CurrentSort = sortBy;
            if (searchString != null)
                page = null;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;

            // var Db = new ApplicationDbContext();
            var role = Identitydb.Roles.First(u => u.Id == id);
            var model = new SelectUsersInRoleViewModel(role);
            ViewBag.RolesName = model.RoleName;
            ViewBag.Count = model.UserCount;

            var model1 = from s in model.Users.ToList()
                         select s;
            //model1 = model1.OrderBy(c => c.UserName);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Pages = page;
            if (itemsPerPage != null)
            {
                pageSize = (int)itemsPerPage;
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            //Cookies for pagesize 
            if (Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"] != null)
            {
                pageSize = Convert.ToInt32(Request.Cookies["pageSize" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"].Value);
                ViewBag.CurrentItemsPerPage = itemsPerPage;
            }
            pageSize = pageSize > 100 ? 100 : pageSize;
            //Cookies for pagination 
            if (Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"] != null)
            {
                pageNumber = Convert.ToInt32(Request.Cookies["pagination" + HttpUtility.UrlEncode(((CustomPrincipal)User).Name) + "UsersInRole"].Value);
                ViewBag.Pages = pageNumber;
            }

            //
            if (!String.IsNullOrEmpty(searchString))
            {
                model1 = searchRecordsRoles(model1.AsQueryable(), searchString.ToUpper());
            }
            if (!String.IsNullOrEmpty(sortBy) && !String.IsNullOrEmpty(isAsc))
            {
                model1 = sortRecordsRoles(model1.AsQueryable(), sortBy, isAsc);
            }
            if (pageNumber > 1)
            {
                var totalListCount = model1.Count();
                int quotient = totalListCount / pageSize;
                var remainder = totalListCount % pageSize;
                var maxpagenumber = quotient + (remainder > 0 ? 1 : 0);
                if (pageNumber > maxpagenumber)
                {
                    pageNumber = 1;
                }
            }

            ViewBag.PageSize = pageSize;

            if (!(RenderPartial == null ? false : RenderPartial.Value) && !Request.IsAjaxRequest())
                return View(model1.ToPagedList(pageNumber, pageSize));
            else
                return PartialView("UsersInRolePartial", model1.ToPagedList(pageNumber, pageSize));

            // return View(model);
        }

        private IQueryable<SelectUserEditorViewModel> searchRecordsRoles(IQueryable<SelectUserEditorViewModel> users, string searchString)
        {
            searchString = searchString.Trim();
            users = users.Where(s => (!String.IsNullOrEmpty(s.UserName) && s.UserName.ToUpper().Contains(searchString)));
            return users;
        }
        private IQueryable<SelectUserEditorViewModel> sortRecordsRoles(IQueryable<SelectUserEditorViewModel> users, string sortBy, string isAsc)
        {
            string methodName = "";
            switch (isAsc.ToLower())
            {
                case "asc":
                    methodName = "OrderBy";
                    break;
                case "desc":
                    methodName = "OrderByDescending";
                    break;
            }
            ParameterExpression paramExpression = Expression.Parameter(typeof(SelectUserEditorViewModel), "ApplicationUser");
            MemberExpression memExp = Expression.PropertyOrField(paramExpression, sortBy);
            LambdaExpression lambda = Expression.Lambda(memExp, paramExpression);
            return (IQueryable<SelectUserEditorViewModel>)users.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { users.ElementType, lambda.Body.Type },
                    users.Expression,
                    lambda));
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UsersInRole(List<SelectUserEditorViewModel> model, string id)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("AssignUserRole"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                foreach (var user in model)
                {
                    var userId = Identitydb.Users.First(p => p.UserName == user.UserName).Id;
                    if (user.Selected)
                        idManager.AddUserToRole(LogggedInUser, userId, id);
                    else
                        if (user.UserName != "Admin")
                            idManager.ClearUsersFromRole(LogggedInUser, userId, id);
                }
                return RedirectToAction("RoleList", "Account");
            }
            return View();
        }
        public ActionResult ReturnToUsersInRole(string urlReferrer)
        {
            return RedirectToAction("RoleList", "Account");
        }
		private void RemoveMultitenant(string username)
        {
            ApplicationDbContext ac = new ApplicationDbContext();
            var oldAccess = ac.MultiTenantLoginSelected.FirstOrDefault(p => p.T_User == username);
            if (oldAccess != null)
            {
                ac.MultiTenantLoginSelected.Remove(oldAccess);
                ac.SaveChanges();
            }
        }
		 [AllowAnonymous]
        public ActionResult ApplicationExtraSecurity()
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("MultiTenantExtraPrivileges"))
                return RedirectToAction("Index", "Home");
            ApplicationDbContext user = new ApplicationDbContext();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var userlist = user.Users.OrderBy(p => p.UserName);
            ViewBag.mainentitylist = new MultiSelectList(db.T_Organizations.OrderBy(p => p.DisplayValue).ToList(), "ID", "DisplayValue");
            List<MultiTenantExtraAccess> model = new List<MultiTenantExtraAccess>();
            ApplicationDbContext adb = new ApplicationDbContext();
            foreach (var item in userlist)
            {
                MultiTenantExtraAccess obj = new MultiTenantExtraAccess();
                obj.T_User = item.UserName;
                var security = adb.MultiTenantExtraAccess.Where(p => p.T_User == item.UserName);
                obj.T_MainEntity = security != null ? string.Join(",", security.Select(p => p.T_MainEntityID)) : "";// ViewBag.mainentitylist;
                model.Add(obj);
            }

            return View(model);
        }
        public ActionResult ApplicationExtraSecurityByMainEntity()
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("MultiTenantExtraPrivileges"))
                return RedirectToAction("Index", "Home");
            ApplicationDbContext user = new ApplicationDbContext();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var userlist = user.Users.OrderBy(p => p.UserName);
            var mainentitylist = db.T_Organizations.OrderBy(p => p.DisplayValue).ToList();
            ViewBag.mainentitylist = new MultiSelectList(userlist, "UserName", "UserName");
            List<MultiTenantExtraAccess> model = new List<MultiTenantExtraAccess>();
            ApplicationDbContext adb = new ApplicationDbContext();
            foreach (var item in mainentitylist)
            {
                MultiTenantExtraAccess obj = new MultiTenantExtraAccess();
                obj.T_MainEntityID = item.Id;
                obj.DisplayValue = item.DisplayValue;
                var security = adb.MultiTenantExtraAccess.Where(p => p.T_MainEntityID == item.Id);
                obj.T_MainEntity = security != null ? string.Join(",", security.Select(p => p.T_User)) : "";// ViewBag.mainentitylist;
                model.Add(obj);
            }

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ApplicationExtraSecurity(List<MultiTenantExtraAccess> model, string GroupBy)
        {
             if (!((CustomPrincipal)User).CanAddAdminFeature("MultiTenantExtraPrivileges"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                ApplicationDbContext adb = new ApplicationDbContext();
                foreach (var item in adb.MultiTenantExtraAccess.ToList())
                {
                    adb.MultiTenantExtraAccess.Remove(item);
                }
                adb.SaveChanges();
                
                foreach (var item in model)
                {
                    if (!string.IsNullOrEmpty(item.T_MainEntity))
                    {
                        foreach (var mainid in item.T_MainEntity.Split(",".ToCharArray()))
                        {
                            if (!string.IsNullOrEmpty(mainid))
                            {
                                MultiTenantExtraAccess obj = new MultiTenantExtraAccess();
                                if (GroupBy == "MainEntity")
                                {
                                    obj.T_MainEntityID = item.T_MainEntityID;
                                    obj.T_User = mainid;
                                }
                                else
                                {
                                    obj.T_User = item.T_User;
                                    obj.T_MainEntityID = Convert.ToInt64(mainid);
                                }
                                adb.MultiTenantExtraAccess.Add(obj);
                            }
                        }
                    }
                }
                adb.SaveChanges();
                if (GroupBy == "MainEntity")
                    return RedirectToAction("ApplicationExtraSecurityByMainEntity");
                else
                    return RedirectToAction("ApplicationExtraSecurity");
            }
            ApplicationDbContext user = new ApplicationDbContext();
            ApplicationContext db = new ApplicationContext(new SystemUser());
            var userlist = user.Users;
            ViewBag.mainentitylist = new MultiSelectList(db.T_Organizations.OrderBy(p => p.DisplayValue), "ID", "DisplayValue");
            List<MultiTenantExtraAccess> modela = new List<MultiTenantExtraAccess>();
            ApplicationDbContext adbc = new ApplicationDbContext();
            foreach (var item in userlist)
            {
                MultiTenantExtraAccess obj = new MultiTenantExtraAccess();
                obj.T_User = item.UserName;
                var security = adbc.MultiTenantExtraAccess.FirstOrDefault(p => p.T_User == item.UserName);
                obj.T_MainEntity = security != null ? security.T_MainEntity : "";// ViewBag.mainentitylist;
                model.Add(obj);
            }
            return View(modela);
        }
		[AcceptVerbs(HttpVerbs.Get)]
        [AllowAnonymous]
        public JsonResult GetAllValue(string caller, string key, string AssoNameWithParent, string AssociationID, string ExtraVal)
        {
            ApplicationContext db = new ApplicationContext(new SystemUser());
            IQueryable<T_Organization> list = db.T_Organizations;
            if (key != null && key.Length > 0)
            {
                if (ExtraVal != null && ExtraVal.Length > 0)
                {
                    long? val = Convert.ToInt64(ExtraVal);
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key) && p.Id != val).Take(9).Union(list.Where(p => p.Id == val)).OrderBy(q => q.DisplayValue).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = from x in list.Where(p => p.DisplayValue.Contains(key)).OrderBy(q => q.DisplayValue).Take(10).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                    var data = from x in list.OrderBy(q => q.DisplayValue).Take(10).ToList()
                               select new { Id = x.Id, Name = x.DisplayValue };
                    return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
#region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            ClearCookies();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                var strError = "";
                foreach (var msg in error.Split('.'))
                {
                    strError += msg + ".\r\n";
                }
                ModelState.AddModelError("", strError);
            }
        }
        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }
        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            string IsAdmin = "false";
            if (((CustomPrincipal)User).IsAdmin)
                IsAdmin = "true";
            else
                IsAdmin = "false";
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
        #region Helpers External Login ex google,yahoo etc
        //
        // POST: /Account/Disassociate
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            if (provider.ToLower() == "yahoo")
                OAuthWebSecurity.RequestAuthentication(provider, Url.Action("ExternalLoginCallbackYahoo", new { provider = provider, ReturnUrl = returnUrl }));
            else
            {
                ThirdPartyLoginRepository thirdPartyLoginRepository = new ThirdPartyLoginRepository();
                ThirdPartyLogin thirdPartyLogin = thirdPartyLoginRepository.GetThirdPartyLogin();
                if (provider.ToLower() == "facebook")
                {
                    if (!string.IsNullOrEmpty(thirdPartyLogin.FacebookId) && !string.IsNullOrEmpty(thirdPartyLogin.FacebookSecretKey) && thirdPartyLogin.FacebookId.ToUpper() != "NONE" && thirdPartyLogin.FacebookSecretKey.ToUpper() != "NONE")
                        return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { provider = provider, ReturnUrl = returnUrl }));
                    else
                        return RedirectToAction("Login", new RouteValueDictionary(new { returnUrl = "", ThirdPartyLoginError = "Facebook settings has not been done. Please contact your Administrator." }));
                }
                else if (provider.ToLower() == "googleplus")
                {
                    if (!string.IsNullOrEmpty(thirdPartyLogin.GooglePlusId) && !string.IsNullOrEmpty(thirdPartyLogin.GooglePlusSecretKey) && thirdPartyLogin.GooglePlusId.ToUpper() != "NONE" && thirdPartyLogin.GooglePlusSecretKey.ToUpper() != "NONE")
                        return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { provider = provider, ReturnUrl = returnUrl }));
                    else
                        return RedirectToAction("Login", new RouteValueDictionary(new { returnUrl = "", ThirdPartyLoginError = "GooglePlus settings has not been done. Please contact your Administrator." }));
                }
            }
            return View();
        }
        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string provider, string returnUrl)
        {
            // ApplicationDbContext localAppDB = new ApplicationDbContext();
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var userExist = await UserManager.FindByNameAsync(loginInfo.Email);
            if (userExist != null)
            {
                var localAppUser = Identitydb.Users.Where(p => p.UserName == userExist.UserName).ToList();
                if (localAppUser != null && localAppUser.Count > 0)
                {
                    ApplicationContext db = new ApplicationContext(new SystemUser());
                    string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;

                    if (localAppUser != null && localAppUser.Count > 0)
                    {
                        if ((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                        {
                            if (await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                return View("Login");
                            }
                        }
                        else
                        {
                            if (await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                            {
                                ModelState.AddModelError("", string.Format("Your account has been locked. Please contact your administrator. ", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                return View("Login");
                            }
                        }
                        await SignInAsync(localAppUser[0], isPersistent: false);
                        return RedirectToAction("Index", "Home", new { isThirdParty = true });
                    }
                }
            }
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                //return RedirectToLocal(returnUrl);
                return RedirectToAction("Index", "Home", new { isThirdParty = true });
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                RegisterViewModel usermodel = new RegisterViewModel();
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Manage");
                }
                if (ModelState.IsValid)
                {
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                    if (provider.ToLower() == "facebook")
                    {
                        usermodel.FirstName = info.Email;
                        usermodel.LastName = info.Email;
                    }
                    else
                    {
                        usermodel.FirstName = info.DefaultUserName;
                        usermodel.LastName = info.DefaultUserName;
                    }
                    usermodel.UserName = info.Email;
                    //password will genrate randomly
                    string randomPassword = Membership.GeneratePassword(8, 2);
                    usermodel.ConfirmPassword = randomPassword;
                    usermodel.Password = randomPassword;
                    //
                    usermodel.Email = info.Email;
                    var LogedInuser = usermodel.GetUser();
                    var localAppUser = Identitydb.Users.Where(p => p.UserName == LogedInuser.UserName).ToList();
                    if (localAppUser != null && localAppUser.Count > 0)
                    {
                        await SignInAsync(localAppUser[0], isPersistent: false);
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Home", new { isThirdParty = true });
                    }
                    var result = await UserManager.CreateAsync(LogedInuser, usermodel.Password);
                    if (result.Succeeded)
                    {
                        var idManager = new IdentityManager();
                        // var Db = new ApplicationDbContext();
                        idManager.ClearUserRoles(LogggedInUser, LogedInuser.Id);
                        AssignDefaultRoleToNewUser(LogedInuser.Id);
                        //idManager.AddUserToRole(LogedInuser.Id, "ReadOnly");
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");
                        var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
                        SendEmail sendEmail = new SendEmail();
                        var UserMail = await UserManager.FindByNameAsync(usermodel.UserName);
                        if (UserMail != null)
                        {
                            string EmailBody = "Dear " + UserMail.FirstName + ", <br/><br/>";
                            EmailBody += "User Name : " + UserMail.Email + "<br/>";
                            EmailBody += "Password  : " + randomPassword + "<br/>";
                            EmailBody += "Click <a href='" + appURL + "'>here</a> to login in <b>" + CommonFunction.Instance.AppName() + "</b>";
                            sendEmail.Notify(UserMail.Email, EmailBody, CommonFunction.Instance.AppName() + " : You have been registered successfully!");
                        }
                        var Registerlogin = await AuthenticationManager.GetExternalLoginInfoAsync();
                        var RegisteruserExist = await UserManager.FindByNameAsync(Registerlogin.Email);
                        if (RegisteruserExist != null)
                        {
                            var LogedInAppUser = Identitydb.Users.Where(p => p.UserName == RegisteruserExist.UserName).ToList();
                            if (LogedInAppUser != null && LogedInAppUser.Count > 0)
                            {
                                await SignInAsync(LogedInAppUser[0], isPersistent: false);
                                return RedirectToLocal(returnUrl);
                                return RedirectToAction("Index", "Home", new { isThirdParty = true });
                            }
                        }
                    }
                    AddErrors(result);
                }
                ViewBag.ReturnUrl = returnUrl;
                return View(usermodel);
            }
        }
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallbackYahoo(string provider, string returnUrl)
        {
            // ApplicationDbContext localAppDB = new ApplicationDbContext();
            var yahooResult = OAuthWebSecurity.VerifyAuthentication();
            if (yahooResult.IsSuccessful)
            {
                var userExist = await UserManager.FindByNameAsync(yahooResult.ExtraData["email"]);
                if (userExist != null)
                {
                    var localAppUser = Identitydb.Users.Where(p => p.UserName == userExist.UserName).ToList();
                    if (localAppUser != null && localAppUser.Count > 0)
                    {
                        ApplicationContext db = new ApplicationContext(new SystemUser());
                        string applySecurityPolicy = db.AppSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;

                        if (localAppUser != null && localAppUser.Count > 0)
                        {
                            if ((applySecurityPolicy.ToLower() == "yes") && !(((CustomPrincipal)User).Identity is System.Security.Principal.WindowsIdentity))
                            {
                                if (await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                                {
                                    ModelState.AddModelError("", string.Format("Your account has been locked out for {0} hours due to multiple failed login attempts.", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                    return View("Login");
                                }
                            }
                            else
                            {
                                if (await UserManager.IsLockedOutAsync(localAppUser[0].Id))
                                {
                                    ModelState.AddModelError("", string.Format("Your account has been locked. Please contact your administrator. ", db.AppSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                                    return View("Login");
                                }
                            }
                            await SignInAsync(localAppUser[0], isPersistent: false);
                            return RedirectToAction("Index", "Home", new { isThirdParty = true });
                        }
                    }
                }
                // Sign in the user with this external login provider if the user already has a login
                var user = await UserManager.FindByNameAsync(yahooResult.ExtraData["email"]);
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("Index", "Home", new { isThirdParty = true });
                }
                else
                {
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = yahooResult.Provider;
                    RegisterViewModel usermodel = new RegisterViewModel();
                    if (User.Identity.IsAuthenticated)
                    {
                        return RedirectToAction("Manage");
                    }
                    if (ModelState.IsValid)
                    {
                        var info = OAuthWebSecurity.VerifyAuthentication();
                        if (info == null)
                        {
                            return View("ExternalLoginFailure");
                        }
                        usermodel.FirstName = info.ExtraData["fullName"];
                        usermodel.LastName = info.ExtraData["fullName"];
                        usermodel.UserName = info.ExtraData["email"];
                        //password will genrate randomly
                        string randomPassword = Membership.GeneratePassword(8, 2);
                        usermodel.ConfirmPassword = randomPassword;
                        usermodel.Password = randomPassword;
                        //
                        usermodel.Email = info.UserName;
                        var LogedInuser = usermodel.GetUser();
                        var localAppUser = Identitydb.Users.Where(p => p.UserName == LogedInuser.UserName).ToList();
                        if (localAppUser != null && localAppUser.Count > 0)
                        {
                            await SignInAsync(localAppUser[0], isPersistent: false);
                            // return RedirectToLocal(returnUrl);
                            return RedirectToAction("Index", "Home", new { isThirdParty = true });
                        }
                        var result = await UserManager.CreateAsync(LogedInuser, usermodel.Password);
                        if (result.Succeeded)
                        {
                            var idManager = new IdentityManager();
                            //  var Db = new ApplicationDbContext();
                            idManager.ClearUserRoles(LogggedInUser, LogedInuser.Id);
                            AssignDefaultRoleToNewUser(LogedInuser.Id);
                            //idManager.AddUserToRole(LogedInuser.Id, "ReadOnly");
                            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");
                            var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
                            SendEmail sendEmail = new SendEmail();
                            var UserMail = await UserManager.FindByNameAsync(usermodel.UserName);
                            if (UserMail != null)
                            {
                                string EmailBody = "Dear " + UserMail.FirstName + ", <br/><br/>";
                                EmailBody += "User Name : " + UserMail.Email + "<br/>";
                                EmailBody += "Password  : " + randomPassword + "<br/>";
                                EmailBody += "Click <a href='" + appURL + "'>here</a> to login in <b>" + CommonFunction.Instance.AppName() + "</b>";
                                sendEmail.Notify(UserMail.Email, EmailBody, CommonFunction.Instance.AppName() + " : You have been registered successfully!");
                            }
                            var Registerlogin = OAuthWebSecurity.VerifyAuthentication();
                            var RegisteruserExist = await UserManager.FindByNameAsync(Registerlogin.ExtraData["email"]);
                            if (RegisteruserExist != null)
                            {
                                var LogedInAppUser = Identitydb.Users.Where(p => p.UserName == RegisteruserExist.UserName).ToList();
                                if (LogedInAppUser != null && LogedInAppUser.Count > 0)
                                {
                                    await SignInAsync(LogedInAppUser[0], isPersistent: false);
                                    //return RedirectToLocal(returnUrl);
                                    return RedirectToAction("Index", "Home", new { isThirdParty = true });
                                }
                            }
                        }
                        AddErrors(result);
                    }
                    ViewBag.ReturnUrl = returnUrl;
                    return View(usermodel);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }
        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }
        //[AllowAnonymous]
        //public ActionResult ExternalLoginConfirmation(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}
        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            RegisterViewModel usermodel = new RegisterViewModel();
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }
            if (ModelState.IsValid)
            {
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                usermodel.FirstName = info.DefaultUserName;
                usermodel.LastName = info.DefaultUserName;
                usermodel.UserName = info.DefaultUserName;
                usermodel.ConfirmPassword = "test123";
                usermodel.Password = "test123";
                usermodel.Email = info.Email;
                var user = usermodel.GetUser();
                var result = await UserManager.CreateAsync(user, usermodel.Password);
                if (result.Succeeded)
                {
                    //result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    //if (result.Succeeded)
                    //{
                    await SignInAsync(user, isPersistent: false);
                    var idManager = new IdentityManager();
                    // var Db = new ApplicationDbContext();
                    //var user = Db.Users.First(u => u.UserName == model.UserName);
                    idManager.ClearUserRoles(LogggedInUser, user.Id);
                    AssignDefaultRoleToNewUser(user.Id);
                    //idManager.AddUserToRole(user.Id, "ReadOnly");
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");
                    return RedirectToAction("Index", "Home");
                    //}
                }
                AddErrors(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(usermodel);
        }
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        private void ClearCookies()
        {
            string AppUrl = System.Configuration.ConfigurationManager.AppSettings["AppUrl"];
            HttpCookie userrole = new HttpCookie(AppUrl + "CurrentRole");
            HttpCookie userpage = new HttpCookie("PageId");
            userpage.Expires = DateTime.Now.AddDays(-1);
            userrole.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userpage);
            Response.Cookies.Add(userrole);
            Response.Cookies["fltCookie"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["fltCookieFltTabId"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Clear();
            Session.Clear();
            Response.Cookies[AppUrl + "CurrentRole"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["PageId"].Expires = DateTime.Now.AddDays(-1);
            //string[] myCookies = Request.Cookies.AllKeys;
            //foreach (string cookie in myCookies)
            //{
            //Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            //}
        }
        public static string CreateRandomPassword()
        {
            ApplicationContext dbcontext = new ApplicationContext(new SystemUser());
            int pwdlength = Convert.ToInt32(dbcontext.AppSettings.Where(p => p.Key == "PasswordMinimumLength").FirstOrDefault().Value);
            bool isalphanumeric = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireAlphaNumericCharacter").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool isdigit = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireDigit").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool isUpperCase = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireUpperCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;
            bool isLowerCase = dbcontext.AppSettings.Where(p => p.Key == "PasswordRequireLowerCase").FirstOrDefault().Value.ToLower() == "yes" ? true : false;

            char[] charUpper = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] charNumeric = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] charAlphaNumeric = { '!', '@', '$', '?', '_', '&', '#', '*' };
            char[] charLower = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            System.Text.StringBuilder randomString = new System.Text.StringBuilder();

            Random randomCharacter = new Random();
            int randomCharSelected;
            int stringlength = pwdlength;
            if (isUpperCase)
            {
                randomCharSelected = randomCharacter.Next(1, charUpper.Length);
                randomString.Append(charUpper[randomCharSelected]);
            }
            if (isalphanumeric)
            {
                randomCharSelected = randomCharacter.Next(1, charAlphaNumeric.Length);
                randomString.Append(charAlphaNumeric[randomCharSelected]);
            }
            if (isLowerCase)
            {
                for (uint i = 0; i < stringlength; i++)
                {
                    int randomCharSelected1 = randomCharacter.Next(1, (stringlength));
                    randomString.Append(charLower[randomCharSelected1]);
                }
            }
            if (isdigit)
            {
                randomCharSelected = randomCharacter.Next(1, charNumeric.Length);
                randomString.Append(charNumeric[randomCharSelected]);
            }
            return randomString.ToString();
        }
        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }
            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }
            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }
            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
