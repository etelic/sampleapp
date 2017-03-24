using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace GeneratorBase.MVC.Models
{
    internal interface IModelDocumentationProvider
    {
        IDictionary<string, TypeDocumentation> GetModelDocumentation(HttpActionDescriptor actionDescriptor);
    }
}
