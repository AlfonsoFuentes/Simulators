﻿using Simulator.Shared.Models.HCs.PlannedSKUs;
using Web.Infrastructure.Managers.Generic;

namespace Web.Infrastructure.Validators.FinishinLines.PlannedSKUs
{

    public class PlannedSKUValidator : AbstractValidator<PlannedSKUDTO>
    {
        private readonly IGenericService Service;

        public PlannedSKUValidator(IGenericService service)
        {
            Service = service;

            RuleFor(x => x.PlannedCases).NotEqual(0).WithMessage("Planned cases must be defined!");
          
            RuleFor(x => x.SKU).NotNull().WithMessage("SKU must be defined!");
          
            RuleFor(x => x.TimeToChangeSKUValue).NotEqual(0).WithMessage("Time to Scahnge Sku must be defined!");

        }

       
    }
}
