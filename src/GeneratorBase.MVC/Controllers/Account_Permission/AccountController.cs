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
namespace GeneratorBase.MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
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
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };

			ApplicationContext db = new ApplicationContext(new SystemUser());
            var appSettings = db.AppSettings;
            string applySecurityPolicy = appSettings.Where(p => p.Key == "ApplySecurityPolicy").FirstOrDefault().Value;
			UserManager.UserLockoutEnabledByDefault = true;
            if(applySecurityPolicy.ToLower() == "yes")
            {
                //UserManager.UserLockoutEnabledByDefault = Convert.ToBoolean(appSettings.Where(p => p.Key == "UserLockoutEnabledByDefault").FirstOrDefault().Value);
                UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromHours(Double.Parse(appSettings.Where(p => p.Key == "DefaultAccountLockoutTimeSpan").FirstOrDefault().Value));
                UserManager.MaxFailedAccessAttemptsBeforeLockout = Convert.ToInt32(appSettings.Where(p => p.Key == "MaxFailedAccessAttemptsBeforeLockout").FirstOrDefault().Value);
            }
        }
        public UserManager<ApplicationUser> UserManager { get; private set; }
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
                            ApplicationDbContext localAppDB = new ApplicationDbContext();
                            var localAppUser = localAppDB.Users.Where(p => p.UserName == model.UserName).ToList();
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
		 // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult CreateUser()
        {
            if (((CustomPrincipal)User).IsAdmin())
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
        // [Authorize(Roles = "Admin")]
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
         [HttpPost, ValidateInput(false)]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string CaptchaText)
        {
			ViewBag.DuplicacyMessage = "";
			if (ModelState.IsValid)
            {
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
                            var Db = new ApplicationDbContext();
                            var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Registration");
                            if (EmailTemplate != null)
                            {
                                string mailbody = string.Empty;
                                if (!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                                {
                                    mailbody = EmailTemplate.EmailContent;
                                    mailbody = mailbody.Replace("###FullName###", model.FirstName + " " + model.LastName).Replace("###AppName###", CommonFunction.Instance.AppName()).Replace("###URL###", " <a href='" + appURL + "'>here</a>");
                                }
                                sendEmail.Notify(model.Email, mailbody, CommonFunction.Instance.AppName() + " :Your registration is successful!");
                            }
							//return RedirectToAction("Index", "Home");
							ViewBag.Success = "You are registered successfully, Email has been sent to registered email id.";
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Captcha verification failed!";
                    }
                }
                else
                {
                    ViewBag.DuplicacyMessage = "Username already exist. Please try another one.";
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
                idManager.AddUserToRole(userId, _adminSettings.GetDefaultRoleForNewUser());
        }
		[HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterUser(RegisterViewModel model)
        {
           if (ModelState.IsValid)
            {
                var user = model.GetUser();
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
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
                var appURL = "http://" + CommonFunction.Instance.Server() + "/" + CommonFunction.Instance.AppURL();
                SendEmail sendEmail = new SendEmail();
                var Db = new ApplicationDbContext();
                var User = await UserManager.FindByNameAsync(model.Username);  
                if (User != null)
                {
                    string randomPassword = Membership.GeneratePassword(8, 2);
                    var user1 = UserManager.RemovePassword(Convert.ToString(User.Id));
                    UserManager.AddPassword(Convert.ToString(User.Id), randomPassword);
				   var EmailTemplate = (new ApplicationContext(new SystemUser())).EmailTemplates.FirstOrDefault(e => e.associatedemailtemplatetype.DisplayValue == "User Forgot Password");
                    if (EmailTemplate != null)
                    {
                        string mailbody = string.Empty;
                        if (!string.IsNullOrEmpty(EmailTemplate.EmailContent))
                        {
                            mailbody = EmailTemplate.EmailContent;
                            mailbody = mailbody.Replace("###FullName###", User.FirstName + " " + User.LastName).Replace("###Password###", randomPassword).Replace("###URL###", " <a href='" + appURL + "'>here</a>").Replace("###AppName###", CommonFunction.Instance.AppName());
                        }
                        sendEmail.Notify(User.Email, mailbody, CommonFunction.Instance.AppName() + " :Your password changed successfully!");
                    }
					SavePasswordHistory(Convert.ToString(User.Id));
                    return Json("Ok", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                }
                else
	                return Json("Wrong Username", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
            }
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult CreateRole()
        {
            if (((CustomPrincipal)User).IsAdmin())
                return View();
            else
                return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRole(CreateRoleViewModel model)
        {
		 if (((CustomPrincipal)User).IsAdmin())
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
        //  [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
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
            ApplicationDbContext appDb = new ApplicationDbContext();
            PasswordHistory history = new PasswordHistory();
            var Db = new ApplicationDbContext();
            var user = UserManager.FindById(userID);
            history.Date = DateTime.Now;
            history.UserId = user.Id;
            history.HashedPassword = user.PasswordHash;
            appDb.PasswordHistorys.Add(history);
            appDb.SaveChanges();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
			 HttpCookie userrole = new HttpCookie("CurrentRole");
            HttpCookie userpage = new HttpCookie("PageId");
            userpage.Expires = DateTime.Now.AddDays(-1);
            userrole.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userpage);
            Response.Cookies.Add(userrole);
            return RedirectToAction("Index", "Home");
        }

		[HttpGet]
        public ActionResult BrowserClose()
        {
            AuthenticationManager.SignOut();
            HttpCookie userrole = new HttpCookie("CurrentRole");
            HttpCookie userpage = new HttpCookie("PageId");
            userpage.Expires = DateTime.Now.AddDays(-1);
            userrole.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userpage);
            Response.Cookies.Add(userrole);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult Index(string currentFilter, string searchString, string sortBy, string isAsc, int? page, int? itemsPerPage, bool? IsExport, bool? RenderPartial)
        {
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

            var Db = new ApplicationDbContext();
            List<ApplicationUser> users = null;
            users = Db.Users.ToList();
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            var _model =from s in model
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
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult RoleList()
        {
			if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var roles = Db.Roles;
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
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
			if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
			if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
		[AllowAnonymous]
        public ActionResult LockUnlockUser(string id, string lockuser)
        {
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.Id == id);

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


                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index");
        }
		
	 // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult EditRole(string id, ManageMessageId? Message = null)
        {
			if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var roles = Db.Roles.First(u => u.Name == id);
            var model = new EditRoleViewModel(roles);
            ViewBag.MessageId = Message;
            return View(model);
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(EditRoleViewModel model)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                if (Db.Roles.Where(u => u.Name == model.Name).Count() <= 0)
                {
                    var roles = Db.Roles.First(u => u.Name == model.OriginalName);
                    roles.Name = model.Name;
                    Db.Entry(roles).State = System.Data.Entity.EntityState.Modified;
                    await Db.SaveChangesAsync();
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
        //  [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult Delete(string id = null)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult DeleteConfirmed(string id)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            Db.Users.Remove(user);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
		//  [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult DeleteRole(string id = null)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var roles = Db.Roles.First(u => u.Name == id);
            var model = new EditRoleViewModel(roles);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult DeleteRoleConfirmed(string id)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var roles = Db.Roles.First(u => u.Name == id);
            Db.Roles.Remove(roles);
            Db.SaveChanges();
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
        //  [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult UserRoles(string id)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var user = Db.Users.First(u => u.Id == id);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
		if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }
		//  [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public ActionResult UsersInRole(string id, string searchkey)
        {
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var role = Db.Roles.First(u => u.Name == id);
            var model = new SelectUsersInRoleViewModel(role, searchkey);
            return View(model);
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult UsersInRole(SelectUsersInRoleViewModel model, string searchkey)
        {
            if (!((CustomPrincipal)User).IsAdmin())
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ApplicationDbContext();
                var role = Db.Roles.First(u => u.Name == model.RoleName);
                foreach (var user in model.Users)
                {
                    var userId = Db.Users.First(p => p.UserName == user.UserName).Id;
                    if (user.Selected)
                        idManager.AddUserToRole(userId, role.Name);
                    else
                        if (user.UserName != "Admin")
                            idManager.ClearUsersFromRole(userId, role.Name);
                }
                return RedirectToAction("RoleList", "Account");
            }
            return View();
        }
        public ActionResult ReturnToUsersInRole(string urlReferrer)
        {
            return RedirectToAction("RoleList", "Account");
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
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
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
            if (((CustomPrincipal)User).IsAdmin())
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
                    if (!string.IsNullOrEmpty(thirdPartyLogin.FacebookId)&& !string.IsNullOrEmpty(thirdPartyLogin.FacebookSecretKey) && thirdPartyLogin.FacebookId.ToUpper() != "NONE" && thirdPartyLogin.FacebookSecretKey.ToUpper() != "NONE")
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
            ApplicationDbContext localAppDB = new ApplicationDbContext();
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var userExist = await UserManager.FindByNameAsync(loginInfo.Email);
            if (userExist != null)
            {
                var localAppUser = localAppDB.Users.Where(p => p.UserName == userExist.UserName).ToList();
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
                    var localAppUser = localAppDB.Users.Where(p => p.UserName == LogedInuser.UserName).ToList();
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
                        var Db = new ApplicationDbContext();
                        idManager.ClearUserRoles(LogedInuser.Id);
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
                            var LogedInAppUser = localAppDB.Users.Where(p => p.UserName == RegisteruserExist.UserName).ToList();
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
            ApplicationDbContext localAppDB = new ApplicationDbContext();
            var yahooResult = OAuthWebSecurity.VerifyAuthentication();
            if (yahooResult.IsSuccessful)
            {
                var userExist = await UserManager.FindByNameAsync(yahooResult.ExtraData["email"]);
                if (userExist != null)
                {
                    var localAppUser = localAppDB.Users.Where(p => p.UserName == userExist.UserName).ToList();
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
                        var localAppUser = localAppDB.Users.Where(p => p.UserName == LogedInuser.UserName).ToList();
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
                            var Db = new ApplicationDbContext();
                            idManager.ClearUserRoles(LogedInuser.Id);
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
                                var LogedInAppUser = localAppDB.Users.Where(p => p.UserName == RegisteruserExist.UserName).ToList();
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
                    var Db = new ApplicationDbContext();
                    //var user = Db.Users.First(u => u.UserName == model.UserName);
                    idManager.ClearUserRoles(user.Id);
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
