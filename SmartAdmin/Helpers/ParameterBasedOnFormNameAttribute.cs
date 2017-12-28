using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SmartAdmin.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterBasedOnFormNameAttribute : Attribute, IActionFilter
    {
        private readonly string _name;
        private readonly string _actionParameterName;

        public ParameterBasedOnFormNameAttribute()
        {
            this._name = "";
            this._actionParameterName = "";
        }
        public ParameterBasedOnFormNameAttribute(string name, string actionParameterName)
        {
            this._name = name;
            this._actionParameterName = actionParameterName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            


            //var formValue = filterContext.HttpContext.Request.Form[_name];
            //filterContext.ActionArguments[_actionParameterName] = !string.IsNullOrEmpty(formValue);
            //object data;
            //filterContext.ActionArguments.TryGetValue("ModifiedDate", out data);
            //data = System.DateTime.Now;


        }
    }
}
