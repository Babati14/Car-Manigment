using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CarManigment.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ValidateRouteParameterAttribute : ActionFilterAttribute
    {
        private readonly string _routeParameterName;
        private readonly string _formFieldName;
        public ValidateRouteParameterAttribute(string routeParameterName, string formFieldName)
        {
            _routeParameterName = routeParameterName ?? throw new ArgumentNullException(nameof(routeParameterName));
            _formFieldName = formFieldName ?? throw new ArgumentNullException(nameof(formFieldName));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.RouteData.Values.TryGetValue(_routeParameterName, out var routeValue))
            {
                context.Result = new BadRequestObjectResult($"Route parameter '{_routeParameterName}' not found.");
                return;
            }

            object? formModel = null;
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg != null)
                {
                    var property = arg.GetType().GetProperty(_formFieldName);
                    if (property != null)
                    {
                        formModel = arg;
                        break;
                    }
                }
            }

            if (formModel == null)
            {
                context.Result = new BadRequestObjectResult($"Form model with property '{_formFieldName}' not found.");
                return;
            }

            var propertyInfo = formModel.GetType().GetProperty(_formFieldName);
            if (propertyInfo == null)
            {
                context.Result = new BadRequestObjectResult($"Property '{_formFieldName}' not found in model.");
                return;
            }

            var formValue = propertyInfo.GetValue(formModel);

            var routeValueString = routeValue.ToString();
            var formValueString = formValue?.ToString();

            if (!string.Equals(routeValueString, formValueString, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new BadRequestObjectResult(
                    $"Parameter tampering detected: Route parameter '{_routeParameterName}' " +
                    $"({routeValueString}) does not match form field '{_formFieldName}' ({formValueString}).");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
