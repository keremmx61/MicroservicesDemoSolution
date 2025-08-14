using FluentValidation;
using Microsoft.Extensions.Localization;
using ProductService.Localization;
using ProductService.Models;

namespace ProductService.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator(IStringLocalizer<SharedResources> localizer)
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(p => localizer["ProductNameRequired"]);

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage(p => localizer["PriceGreaterThanZero"]);

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage(p => localizer["ProductDescriptionRequired"]);

            RuleFor(p => p.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage(p => localizer["StockCannotBeNegative"]);
        }
    }
}