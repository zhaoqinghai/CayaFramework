using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.BussinessData
{
    public class HelloModel
    {
        [Required]
        [StringLength(5, ErrorMessage = "最大长度5")]
        public string Name { get; set; }
    }

    public class HelloModelValidator : AbstractValidator<HelloModel>
    {
        public HelloModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("12").MaximumLength(5).WithMessage("34");
        }
    }
}
