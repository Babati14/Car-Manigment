using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Car_Manigment.Common.ValidationConstants;
using Microsoft.AspNetCore.Identity;
namespace Car_Manigment.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(
            CarBrandMaxLength,
            MinimumLength = CarBrandMinLength,
            ErrorMessage = InvalidLengthError)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(
            CarModelMaxLength,
            MinimumLength = CarModelMinLength,
            ErrorMessage = InvalidLengthError)]
        public string Model { get; set; } = string.Empty;

        [Range(
            CarMinYear,
            CarMaxYear,
            ErrorMessage = InvalidRangeError)]
        public int Year { get; set; }

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(
            VinLength,
            MinimumLength = VinLength,
            ErrorMessage = InvalidVinError)]
        public string VinNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(
            OwnerNameMaxLength,
            MinimumLength = OwnerNameMinLength,
            ErrorMessage = InvalidLengthError)]
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(
            PhoneNumberMaxLength,
            MinimumLength = PhoneNumberMinLength,
            ErrorMessage = InvalidLengthError)]
        public string OwnerPhone { get; set; } = string.Empty;

        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();

        // Link to the user who owns/created the car
        public string? OwnerId { get; set; }
        public IdentityUser? Owner { get; set; }
    }
}