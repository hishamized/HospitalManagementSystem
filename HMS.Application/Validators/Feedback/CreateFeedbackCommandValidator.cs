using FluentValidation;
using HMS.Application.Commands.Feedback;

namespace HMS.Application.Validators.Feedback
{
    public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
    {
        public CreateFeedbackCommandValidator()
        {
            RuleFor(x => x.Dto.DoctorId)
                .GreaterThan(0)
                .WithMessage("Doctor selection is required.");

            RuleFor(x => x.Dto.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Dto.Comments)
                .MaximumLength(1000)
                .WithMessage("Comments cannot exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Dto.Comments));

            RuleFor(x => x.Dto.SubmittedFromIP)
                .NotEmpty()
                .WithMessage("IP address is required.");

            RuleFor(x => x.Dto.SubmittedFromDevice)
                .NotEmpty()
                .WithMessage("Device information is required.");
        }
    }
}
