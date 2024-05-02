namespace PizzaApi
{
    using Microsoft.AspNetCore.Mvc.Controllers;
    using PizzaApi.Controllers;
    using System.Reflection;

    internal sealed class CustomerControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var isCustomController = !typeInfo.IsAbstract && typeof(PizzaOrdersController).IsAssignableFrom(typeInfo);
            return isCustomController || base.IsController(typeInfo);
        }
    }
}
