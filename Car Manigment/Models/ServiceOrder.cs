using System;
using System.ComponentModel.DataAnnotations;
using Car_Manigment.Common;

namespace Car_Manigment.Models
{
    public class ServiceOrder
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.ServiceDescriptionMaxLength,
            MinimumLength = ValidationConstants.ServiceTitleMinLength,
            ErrorMessage = ValidationConstants.InvalidLengthError)]
        public string Description { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = ValidationConstants.InvalidRangeError)]
        [DataType(DataType.Currency)]
        public decimal EstimatedPrice { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        public int CarId { get; set; }
        public Car Car { get; set; }
    }
}