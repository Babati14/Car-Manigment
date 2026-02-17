using System;
using System.ComponentModel.DataAnnotations;
using static CarManigment.Common.ValidationConstants;
using Microsoft.AspNetCore.Identity;

namespace Car_Manigment.Models
{
    public class ServiceOrder
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(
            ServiceDescriptionMaxLength,
            MinimumLength = ServiceTitleMinLength,
            ErrorMessage = InvalidLengthError)]
        public string Description { get; set; } = null!;

        [Range(0.01, double.MaxValue, ErrorMessage = InvalidRangeError)]
        [DataType(DataType.Currency)]
        public decimal EstimatedPrice { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

        [Required(ErrorMessage = RequiredFieldError)]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        public string? CreatedById { get; set; }
        public IdentityUser? CreatedBy { get; set; }
    }
}