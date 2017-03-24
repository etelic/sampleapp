using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC.Controllers
{
    public class AdminController : BaseController
    {
        private IAdminSettingsRepository _repository;
        public AdminController()
            : this(new AdminSettingsRepository())
        {
        }
        public AdminController(IAdminSettingsRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            if (!((CustomPrincipal)User).HasAdminPrivileges())
                return RedirectToAction("Index", "Home");

            var fileList = Directory.GetFiles(Server.MapPath(@"~/Templates/"), "*.docx").Where(file => Regex.IsMatch(Path.GetFileName(file), "^[0-9]+"))
         .Select(f =>
         {
             int id = Int32.Parse(Path.GetFileNameWithoutExtension(f.ToLower()).Replace(".docx", ""));
             string Name = documentName(id).ToLower().Replace(".docx", "");
             return new { id, Name };
         }).ToList();
            if (fileList.Count > 0)
                ViewBag.FileList = fileList;

            return View();
        }

        private string documentName(int id)
        {
            string docName = string.Empty;
            switch (id)
            {
                case 1:
                    docName = "Common User's Guide.docx";
                    break;
                case 2:
                    docName = "General Deployment Guide.docx";
                    break;
                case 3:
                    docName = "Security Guide.docx";
                    break;
                case 4:
                    docName = "Configuring Business Rules.docx";
                    break;
                case 5:
                    docName = "Database Design Document.docx";
                    break;
                case 9:
                    docName = "Troubleshooting Guide.docx";
                    break;
                case 10:
                    docName = "Turanto User's Guide.docx";
                    break;
                case 7:
                    docName = "Current Security Settings.docx";
                    break;
                case 8:
                    docName = "Current Business Rules.docx";
                    break;
                default:
                    break;
            }
            return docName;
        }

        [HttpPost]
        public ActionResult Edit(AdminSettings adminSettings, HttpPostedFileBase logo)
        {
            if (((CustomPrincipal)User).IsAdmin)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _repository.EditAdminSettings(adminSettings);
                        return RedirectToAction("Index", "Admin");
                    }
                    catch (Exception ex)
                    {
                        //error msg for failed edit in XML file
                        ModelState.AddModelError("", "Error editing record. " + ex.Message);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult SetDefaultRole(string RoleName)
        {
            if (((CustomPrincipal)User).IsAdmin)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        AdminSettings adminSettings = new AdminSettings();
                        adminSettings.DefaultRoleForNewUser = RoleName;
                        _repository.EditAdminSettings(adminSettings);
                        return RedirectToAction("RoleList", "Account");
                    }
                    catch (Exception ex)
                    {
                        //error msg for failed edit in XML file
                        ModelState.AddModelError("", "Error editing record. " + ex.Message);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public FileResult DownloadTestScript()
        {
            string zipsolutionPath = Server.MapPath("~/SeleniumTestScript");
            FileZipper fz;
            fz = new FileZipper();
            fz.CopyZip(zipsolutionPath, "TestScript.rar");
            string solutionPath = zipsolutionPath;
            solutionPath = zipsolutionPath + "\\TestScript.rar";
            if (System.IO.File.Exists(solutionPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(solutionPath);
                string fileName = "TestScript.rar";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            return null;
        }
    }
}