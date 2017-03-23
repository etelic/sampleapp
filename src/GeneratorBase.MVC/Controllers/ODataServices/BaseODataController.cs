using GeneratorBase.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.OData;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
namespace GeneratorBase.MVC.ODataControllers
{
    public class BaseODataController : ODataController
    {
        protected ApplicationContext db { get; private set; }
        protected new IUser User { get; private set; }
        private IODataServicesRepository _repository;
        public BaseODataController()
        {
            //this.User = base.User as IUser;
            this.User = new SystemUser();
            this.db = new ApplicationContext(base.User as IUser);
        }
        protected bool IsServiceEnabled()
        {
            _repository = new ODataServicesRepository();
            ODataServices oDataServices = _repository.GetODataServices();
            return oDataServices.EnableODataServices;
        }
    }
}