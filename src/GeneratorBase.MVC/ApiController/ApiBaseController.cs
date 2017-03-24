using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using GeneratorBase.MVC.Controllers;
using System.Configuration;
using System.Web.Http;

using GeneratorBase.MVC.Models;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;


namespace GeneratorBase.MVC.Controllers
{
    [NoCache]
    public class ApiBaseController : ApiController
    {

        private const string Token = "Token";
        public ApplicationContext db { get; private set; } //removed static for race condition
        private ApplicationDbContext UserContext = new ApplicationDbContext();
        public new IUser User { get; private set; }//removed static for race condition
        //
        private const string Origin = "Origin";
        private const string AccessControlRequestMethod = "Access-Control-Request-Method";
        private const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        private const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        private const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        //
        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            //var controllerName = controllerContext.ControllerDescriptor.ControllerName;
            bool isCorsRequest = controllerContext.Request.Headers.Contains(Origin);
            bool isPreflightRequest = controllerContext.Request.Method == HttpMethod.Options;
            TokenServicesController provider = new TokenServicesController();
            if (controllerContext.Request.Headers.Contains(Token))
            {
                var tokenValue = controllerContext.Request.Headers.GetValues(Token).FirstOrDefault();
                if (tokenValue == null || (provider != null && !provider.ValidateToken(tokenValue)))
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Invalid Request" };
                    controllerContext.Request.CreateResponse(responseMessage);
                }
                else
                {
                    ApplicationContext db1 = new ApplicationContext(new SystemUser());
                    var _tokenInfo = db1.ApiTokens.FirstOrDefault(p => p.T_AuthToken == tokenValue);
                    var _userId = _tokenInfo.T_UsersID;
                    ApplicationDbContext userdb = new ApplicationDbContext();
                    var _userInfo = userdb.Users.FirstOrDefault(p => p.Id == _userId);
                    ApiUser _apiuser = new ApiUser(_userInfo.UserName);

                    //
                    var roles = _apiuser.GetRoles();
                    var isAdmin = _apiuser.IsAdminUser();
                    _apiuser.IsAdmin = isAdmin;
                    List<Permission> permissions = new List<Permission>();
                    using (var pc = new PermissionContext())
                    {
                        // so we only make one database call instead of one per entity?
                        var rolePermissions = pc.Permissions.Where(p => roles.Contains(p.RoleName)).ToList();
                        foreach (var entity in GeneratorBase.MVC.ModelReflector.Entities)
                        {
                            var calculated = new Permission();
                            var raw = rolePermissions.Where(p => p.EntityName == entity.Name);
                            calculated.EntityName = entity.Name;
                            calculated.CanEdit = isAdmin || raw.Any(p => p.CanEdit);
                            calculated.CanDelete = isAdmin || raw.Any(p => p.CanDelete);
                            calculated.CanAdd = isAdmin || raw.Any(p => p.CanAdd);
                            calculated.CanView = isAdmin || raw.Any(p => p.CanView);
                            calculated.IsOwner = raw.Any(p => p.IsOwner != null && p.IsOwner.Value);
                            if (!isAdmin)
                                calculated.SelfRegistration = raw.Any(p => p.SelfRegistration != null && p.SelfRegistration.Value);
                            else calculated.SelfRegistration = false;
                            if (calculated.IsOwner != null && calculated.IsOwner.Value)
                                calculated.UserAssociation = raw.FirstOrDefault().UserAssociation;
                            else
                                calculated.UserAssociation = string.Empty;


                            //FLS
                            if (!isAdmin)
                            {
                                var listEdit = raw.Select(p => p.NoEdit).ToList();
                                var listView = raw.Select(p => p.NoView).ToList();
                                var resultEdit = "";
                                var resultView = "";
                                foreach (var str in listEdit)
                                {
                                    if (str != null)
                                        resultEdit += str;
                                }
                                foreach (var str in listView)
                                {
                                    if (str != null)
                                        resultView += str;
                                }
                                calculated.NoEdit = resultEdit;
                                calculated.NoView = resultView;
                            }
                            //
                            permissions.Add(calculated);
                        }
                    }
                    _apiuser.permissions = permissions;
                    List<BusinessRule> businessrules = new List<BusinessRule>();
                    using (var br = new BusinessRuleContext())
                    {
                        var rolebr = br.BusinessRules.Where(p => p.Roles != null && p.Roles.Length > 0 && !p.Disable && p.AssociatedBusinessRuleTypeID != 5).ToList();
                        foreach (var rules in rolebr)
                        {
                            if (_apiuser.IsInRole(rules.Roles.Split(",".ToCharArray())))
                            {
                                businessrules.Add(rules);
                            }
                        }
                    }
                    _apiuser.businessrules = businessrules.ToList();
                    db = new ApplicationContext(_apiuser);
                    if (isCorsRequest)
                    {
                        if (isPreflightRequest)
                        {
                            var response = new HttpResponseMessage(HttpStatusCode.OK);
                            response.Headers.Add(AccessControlAllowOrigin, (controllerContext.Request.Headers.GetValues(Origin).First()));

                            string accessControlRequestMethod = controllerContext.Request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                            if (accessControlRequestMethod != null)
                            {
                                response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                            }

                            string requestedHeaders = string.Join(", ", controllerContext.Request.Headers.GetValues(AccessControlRequestHeaders));
                            if (!string.IsNullOrEmpty(requestedHeaders))
                            {
                                response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                            }

                            var tcs = new TaskCompletionSource<HttpResponseMessage>();
                            tcs.SetResult(response);
                            return tcs.Task;

                        }

                        return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
                        {
                            HttpResponseMessage resp = t.Result;
                            resp.Headers.Add(Token, controllerContext.Request.Headers.GetValues(Token).First());
                            return resp;
                        });
                        //
                    }
                }
                return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
                {
                    HttpResponseMessage resp = t.Result;
                    resp.Headers.Add(Token, controllerContext.Request.Headers.GetValues(Token).First());
                    return resp;
                });
            }
            else
            {
                return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(t =>
                {
                    HttpResponseMessage resp = t.Result;
                    resp.StatusCode = HttpStatusCode.NotFound;
                    resp.ReasonPhrase = "Unauthorized access !";
                    //resp.Headers.Add(Token, "UnAuthorized");
                    return resp;
                });
            }
        }
    }
}