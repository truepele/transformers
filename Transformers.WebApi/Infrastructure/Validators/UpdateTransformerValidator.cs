using FluentValidation;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.WebApi.Dto;

namespace Transformers.WebApi.Infrastructure.Validators
{
    public class UpdateTransformerValidator : AbstractValidator<UpdateTransformerDto>
    {
        public UpdateTransformerValidator()
        {
            RuleFor(x => x.RowVersion).NotEmpty();
            RuleFor(x => x.Allegiance).IsInEnum().NotEqual(Allegiance.Undefined);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(Transformer.NameMaxLen);
            RuleFor(x => x.Courage).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Endurance).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Firepower).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Intelligence).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Rank).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Skill).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Speed).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
            RuleFor(x => x.Strength).GreaterThanOrEqualTo(1).LessThanOrEqualTo(10);
        }
    }
}
