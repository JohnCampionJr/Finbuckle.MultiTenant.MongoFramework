// https://github.com/Finbuckle/Finbuckle.MultiTenant/issues/297

using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Finbuckle.Utilities.AspNetCore
{
    public class MultiTenantPageRouteModelConvention : IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            foreach (var selector in model.Selectors)
            {
                selector.AttributeRouteModel.Template =
                    AttributeRouteModel.CombineTemplates("{__tenant__=}", selector.AttributeRouteModel.Template);
            }
        }
    }

}
