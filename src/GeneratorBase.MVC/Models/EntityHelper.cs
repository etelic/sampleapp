using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GeneratorBase.MVC.Models
{
    public static class EntityHelper
    {
        public static string GetName(this IEntity entity)
        {
            // TODO: make sure that we find the base class, sometimes we get Order_8383 instead of Order
            return entity.GetType().ToString();
        }
    }
}