using System;
using System.Web.Http.Controllers;

namespace System.Web.Http.Description
{
    // Summary:
    //     Defines the provider responsible for documenting the service.
    public interface IDocumentationProvider
    {
        // Summary:
        //     Gets the documentation based on System.Web.Http.Controllers.HttpActionDescriptor.
        //
        // Parameters:
        //   actionDescriptor:
        //     The action descriptor.
        //
        // Returns:
        //     The documentation for the controller.
        string GetDocumentation(HttpActionDescriptor actionDescriptor);
        //
        // Summary:
        //     Gets the documentation based on System.Web.Http.Controllers.HttpParameterDescriptor.
        //
        // Parameters:
        //   parameterDescriptor:
        //     The parameter descriptor.
        //
        // Returns:
        //     The documentation for the controller.
        string GetDocumentation(HttpParameterDescriptor parameterDescriptor);
    }
}