﻿using fw_secure_notes_api.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace fw_secure_notes_api.Conventions;

public class RouteConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _centralPrefix;

    public RouteConvention()
    {
        _centralPrefix = new AttributeRouteModel(new RouteAttribute("{title}/{pin}"));
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                if (action.Attributes.OfType<NoParameters>().Any())
                {
                    continue;
                }

                foreach (var selector in action.Selectors)
                {
                    selector.AttributeRouteModel = (selector.AttributeRouteModel == null)
                        ? _centralPrefix
                        : AttributeRouteModel
                            .CombineAttributeRouteModel(_centralPrefix, selector.AttributeRouteModel);
                }
            }
        }
    }
}
