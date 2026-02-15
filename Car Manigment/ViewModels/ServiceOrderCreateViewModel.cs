using System.ComponentModel.DataAnnotations;
using Car_Manigment.Common;

namespace Car_Manigment.ViewModels.ServiceOrders
{
    public class ServiceOrderCreateViewModel
    {
        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.ServiceDescriptionMaxLength,
            MinimumLength = ValidationConstants.ServiceTitleMinLength,
            ErrorMessage = ValidationConstants.InvalidLengthError)]
        public string Description { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = ValidationConstants.InvalidRangeError)]
        public decimal EstimatedPrice { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        public int CarId { get; set; }
    }
}