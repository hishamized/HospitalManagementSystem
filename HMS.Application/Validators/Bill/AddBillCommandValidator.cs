using FluentValidation;
using HMS.Application.DTO.Bill;

namespace HMS.Application.Validators.Bill
{
    public class AddBillValidator : AbstractValidator<AddBillDto>
    {
        public AddBillValidator()
        {
            RuleFor(x => x.PatientId)
                .GreaterThan(0)
                .WithMessage("Patient ID is required.");

            RuleFor(x => x.VisitId)
                .GreaterThan(0)
                .WithMessage("Visit ID is required.");

            RuleFor(x => x.VisitType)
                .NotEmpty()
                .WithMessage("Visit type is required.")
                .Must(vt => vt.Equals("Inpatient", StringComparison.OrdinalIgnoreCase) ||
                            vt.Equals("Outpatient", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Visit type must be either 'Inpatient' or 'Outpatient'.");

            RuleFor(x => x.BillDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Bill date cannot be in the future.");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Total amount must be greater than or equal to 0.");

            RuleFor(x => x.NetAmount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.NetAmount.HasValue)
                .WithMessage("Net amount must be greater than or equal to 0.");

            RuleFor(x => x.PaymentStatus)
                .NotEmpty()
                .WithMessage("Payment status is required.");

            RuleFor(x => x.PaymentMode)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.PaymentMode))
                .WithMessage("Payment mode cannot exceed 50 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Notes))
                .WithMessage("Notes cannot exceed 500 characters.");
        }
    }
}
