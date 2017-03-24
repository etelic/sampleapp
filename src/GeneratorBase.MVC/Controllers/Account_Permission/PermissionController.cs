using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GeneratorBase.MVC.Models;
using System.ComponentModel.DataAnnotations;

namespace GeneratorBase.MVC.Controllers
{
    public class PermissionController : IdentityBaseController
    {
        //private PermissionContext db = new PermissionContext(LogggedInUser);

        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveUBS(USB model)
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("UserBasedSecurity"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var Db = new UserBasedSecurityContext();
                Db.UserBasedSecurities.RemoveRange(Db.UserBasedSecurities);
                Db.SaveChanges();

                foreach (var _model in model.security)
                {
                    Db.UserBasedSecurities.Add(_model);
                }
                Db.SaveChanges();
            }
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ClearUBS()
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("UserBasedSecurity"))
                return RedirectToAction("Index", "Home");
            var Db = new UserBasedSecurityContext();
            Db.UserBasedSecurities.RemoveRange(Db.UserBasedSecurities);
            Db.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }


        public ActionResult UserBasedSecurity(string key)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("UserBasedSecurity"))
                return RedirectToAction("Index", "Home");
            string setEntity = key;
            var Db = new UserBasedSecurityContext();
            var Datalist = Db.UserBasedSecurities.ToList();
            var EntList = GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity && p.Associations.Where(q => q.Target == "IdentityUser").Count() > 0).ToList();
            // var EntList = GeneratorBase.MVC.ModelReflector.Entities.ToList();
            if (key == null && EntList.Count > 0)
            {
                if (Datalist != null && Datalist.Where(p => p.IsMainEntity).Count() > 0)
                {
                    setEntity = Datalist.FirstOrDefault(p => p.IsMainEntity).EntityName;
                    var Entity = ModelReflector.Entities.FirstOrDefault(p => p.Name == setEntity);
                   if(Entity != null) 
                       ViewData["AlreadySet"] = Entity.DisplayName;
                   else ViewData["AlreadySet"] = setEntity;
                }
                else
                    setEntity = EntList[0].Name;
            }

            List<UserBasedSecurity> Data = new List<Models.UserBasedSecurity>();
            if (Datalist != null && Datalist.Where(p => p.EntityName == setEntity && p.IsMainEntity).Count() > 0)
                Data = Datalist;
            else
            {
                List<string> entitiesAdded = new List<string>();
                entitiesAdded.Add(setEntity);
                Data = GetGridData(Data, setEntity, setEntity,entitiesAdded);
            }

            ViewBag.EntityList = new SelectList(EntList, "Name", "DisplayName", setEntity);
            var RoleList = (new GeneratorBase.MVC.Models.CustomRoleProvider()).GetAllRoles().ToList();
            RoleList.Add("All");
            var adminString = System.Configuration.ConfigurationManager.AppSettings["AdministratorRoles"]; //CommonFunction.Instance.AdministratorRoles(); 
            RoleList.Remove(adminString);
            ViewBag.Roles = new SelectList(RoleList, "", "");

            USB DataUSB = new USB(Data);


            return View(DataUSB);
        }

        private List<UserBasedSecurity> GetGridData(List<UserBasedSecurity> Data, string setEntity, string Entity, List<string> entitiesAdded)
        {
            //IEnumerable<string> SelectedEntity = Data.Select(op => op.EntityName);

            if(Data.Where(p=>p.EntityName == setEntity && p.TargetEntityName == Entity).Count()>0)
            //if (SelectedEntity.Contains(Entity))
                return Data;
            else
            {

                var Ent = ModelReflector.Entities.Where(p => p.Name == Entity).ToList()[0];
                
                 if (setEntity == Entity)
                 {
                     UserBasedSecurity UBS = new UserBasedSecurity();
                     UBS.EntityName = Entity;
                     UBS.IsMainEntity = true;
                     UBS.RolesToIgnore = "";
                     UBS.TargetEntityName = "User";
                     UBS.AssociationName = Ent.Associations.Where(p => p.Target == "IdentityUser").ToList()[0].AssociationProperty;
                     Data.Add(UBS);
                 }

               
                foreach(var _obj in ModelReflector.Entities)
                {
                    foreach(var Next in  _obj.Associations.Where(p => p.Target == Entity) )
                    {
                        UserBasedSecurity UBS = new UserBasedSecurity();
                        UBS.EntityName = _obj.Name;
                        UBS.IsMainEntity = false;
                        UBS.RolesToIgnore = "";
                        UBS.TargetEntityName = Entity;
                        UBS.AssociationName = Next.AssociationProperty;
                        UBS.Other1 = _obj.Name == Entity ? "false" : "";
                        Data.Add(UBS);
                        if (entitiesAdded.Contains(_obj.Name))
                            continue;
                        else
                        {
                            entitiesAdded.Add(_obj.Name);
                            GetGridData(Data, Entity, _obj.Name,entitiesAdded);
                            break;
                        }
                    }
                }
               
                
            }

            return Data;

           
        }

        // GET: /Permission/
        public ActionResult Index(string RoleName)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            var Db = new ApplicationDbContext();
            var roles = Db.Roles;
            var model = new SelectEntityRolesViewModel(RoleName);

            //var adminfeaturelist = new List<PermissionAdminPrivilege>();
            //foreach (var item in Enum.GetValues(typeof(AdminFeatures)))
            //{
            //    var obj = new PermissionAdminPrivilege();
            //    obj.RoleName = RoleName;
            //    obj.IsAllow = false;
            //    obj.AdminFeature = item.ToString();
            //    adminfeaturelist.Add(obj);
            //}
            
            //model.privileges = adminfeaturelist;
            return View(model);

            //return View(db.Permissions.ToList());
        }

        // GET: /Permission/
        public ActionResult Fls(string RoleName, string key)
        {
            if (!((CustomPrincipal)User).CanViewAdminFeature("FieldLevelSecurity"))
                return RedirectToAction("Index", "Home");
            string setEntity = key;
            if (key == null)
                setEntity = GeneratorBase.MVC.ModelReflector.Entities[0].Name;
            var model = new SelectFlsViewModel(RoleName, setEntity);
            ViewBag.EntityList = new SelectList(GeneratorBase.MVC.ModelReflector.Entities.Where(p => !p.IsAdminEntity), "Name", "DisplayName", setEntity);
            return View(model);
        }
        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFls(SelectFlsViewModel model)
        {
            PermissionContext db = new PermissionContext(LogggedInUser);
            if (!((CustomPrincipal)User).CanEditAdminFeature("FieldLevelSecurity"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                string EntName = "";
                if (model.Properties.Count() > 0)
                {
                    EntName = model.Properties[0].EntityName;
                    var list = db.Permissions.FirstOrDefault(q => q.RoleName == model.RoleName && EntName == q.EntityName);
                    Permission permission = (list != null ? list : new Permission());
                    string NoEdit = "";
                    string NoView = "";
                    permission.EntityName = EntName;
                    permission.RoleName = model.RoleName;
                    foreach (var ent in model.Properties)
                    {
                        if (ent.NoEdit)
                            NoEdit += ent.PropertyName + ",";
                        if (ent.NoView)
                            NoView += ent.PropertyName + ",";
                    }
                    //NoEdit = NoEdit.TrimEnd(",".ToCharArray());
                    //NoView = NoView.TrimEnd(",".ToCharArray());
                    permission.NoEdit = NoEdit;
                    permission.NoView = NoView;

                    permission.CanAdd = model.CanAdd;
                    permission.CanEdit = model.CanEdit;
                    permission.CanDelete = model.CanDelete;
                    permission.CanView = model.CanView;

                    if (list == null)
                        db.Permissions.Add(permission);
                    db.SaveChanges();
                }
                return RedirectToAction("Fls", "Permission", new { RoleName = model.RoleName, key = EntName });
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdminPrivilege(SelectEntityRolesViewModel model)
        {
            PermissionContext db = new PermissionContext(LogggedInUser);
             if (!((CustomPrincipal)User).IsAdmin)
                return RedirectToAction("Index", "Home");
             if (ModelState.IsValid)
             {
                 foreach (var ent in model.privileges)
                 {
                     var list = db.AdminPrivileges.FirstOrDefault(q => q.RoleName == model.RoleName && ent.AdminFeature == q.AdminFeature);
                     PermissionAdminPrivilege permission = (list != null ? list : new PermissionAdminPrivilege());
                     permission.IsAllow = ent.IsAllow;
                     permission.IsEdit = ent.IsEdit;
                     permission.IsAdd = ent.IsAdd;
                     permission.IsDelete = ent.IsDelete;
                     permission.AdminFeature = ent.AdminFeature;
                     permission.RoleName = ent.RoleName;
                     if (list == null)
                         db.AdminPrivileges.Add(permission);
                     db.SaveChanges();
                 }
             }
             return RedirectToAction("Index", new { RoleName =  model.RoleName});
            // return Json("FROMPAGE", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        // [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SavePermission(SelectEntityRolesViewModel model)
        {
            PermissionContext db = new PermissionContext(LogggedInUser);
            if (!((CustomPrincipal)User).CanEditAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                //var idManager = new IdentityManager();


                foreach (var ent in model.Entities)
                {
                    var list = db.Permissions.FirstOrDefault(q => q.RoleName == model.RoleName && ent.EntityName == q.EntityName);
                    //db.Permissions.RemoveRange(list);
                    Permission permission = (list != null ? list : new Permission());
                    permission.CanAdd = ent.CanAdd;
                    permission.CanDelete = ent.CanDelete;
                    permission.CanView = ent.CanView;
                    permission.CanEdit = ent.CanEdit;
                    permission.IsOwner = ent.IsOwner;
                    permission.SelfRegistration = ent.SelfRegistration;
                    permission.UserAssociation = ent.UserAssociation;
                    permission.EntityName = ent.EntityName;
                    permission.RoleName = model.RoleName;
                    //code for verb action security
                    permission.Verbs = ent.Verbs;
                    //
                    if (list == null)
                        db.Permissions.Add(permission);
                    db.SaveChanges();
                }
                return Json("FROMPAGE", "application/json", System.Text.Encoding.UTF8, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        // GET: /Permission/Details/5
        public ActionResult Details(long? id)
        {
            PermissionContext db = new PermissionContext(LogggedInUser);
            if (!((CustomPrincipal)User).CanViewAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // GET: /Permission/Create
        public ActionResult Create()
        {
            if (!((CustomPrincipal)User).CanAddAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: /Permission/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EntityName,RoleName,CanEdit,CanDelete,CanView,CanAdd")] Permission permission)
        {
            PermissionContext db = new PermissionContext(LogggedInUser);
            if (!((CustomPrincipal)User).CanAddAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Permissions.Add(permission);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(permission);
        }

        // GET: /Permission/Edit/5
        public ActionResult Edit(long? id)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PermissionContext db = new PermissionContext(LogggedInUser);
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: /Permission/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EntityName,RoleName,CanEdit,CanDelete,CanView,CanAdd")] Permission permission)
        {
            if (!((CustomPrincipal)User).CanEditAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                PermissionContext db = new PermissionContext(LogggedInUser);
                db.Entry(permission).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(permission);
        }

        // GET: /Permission/Delete/5
        public ActionResult Delete(long? id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } PermissionContext db = new PermissionContext(LogggedInUser);
            Permission permission = db.Permissions.Find(id);
            if (permission == null)
            {
                return HttpNotFound();
            }
            return View(permission);
        }

        // POST: /Permission/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (!((CustomPrincipal)User).CanDeleteAdminFeature("RoleEntityPermission"))
                return RedirectToAction("Index", "Home"); PermissionContext db = new PermissionContext(LogggedInUser);
            Permission permission = db.Permissions.Find(id);
            db.Permissions.Remove(permission);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
            base.Dispose(disposing);
        }



    }


}

