using FluentValidation;
using HMS.Application.Commands.Bill;

namespace HMS.Application.Validators.Bill
{
    public class EditBillCommandValidator : AbstractValidator<EditBillCommand>
    {
        public EditBillCommandValidator()
        {
            RuleFor(x => x.Bill.Id)
                .GreaterThan(0).WithMessage("Bill Id is required.");

            RuleFor(x => x.Bill.PatientId)
                .GreaterThan(0).WithMessage("Patient Id is required.");

            RuleFor(x => x.Bill.BillDate)
                .NotEmpty().WithMessage("Bill date is required.");

            RuleFor(x => x.Bill.PaymentStatus)
                .NotEmpty().WithMessage("Payment status is required.")
                .Must(x => new[] { "Paid", "Unpaid", "Partial", "Pending", "Cancelled" }.Contains(x))
                .WithMessage("Invalid payment status.");

            RuleFor(x => x.Bill.PaymentMode)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "Cash", "Card", "Insurance" }.Contains(x))
                .WithMessage("Invalid payment mode.");

            RuleFor(x => x.Bill.TotalAmount)
                .NotNull().WithMessage("Total amount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative.");

            RuleFor(x => x.Bill.NetAmount)
                .NotNull().WithMessage("Net amount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("Net amount cannot be negative.");
        }
    }
}
