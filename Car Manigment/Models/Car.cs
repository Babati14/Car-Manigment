using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Car_Manigment.Common;

namespace Car_Manigment.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.CarBrandMaxLength,
            MinimumLength = ValidationConstants.CarBrandMinLength,
            ErrorMessage = ValidationConstants.InvalidLengthError)]
        public string Brand { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.CarModelMaxLength,
            MinimumLength = ValidationConstants.CarModelMinLength,
            ErrorMessage = ValidationConstants.InvalidLengthError)]
        public string Model { get; set; }

        [Range(
            ValidationConstants.CarMinYear,
            ValidationConstants.CarMaxYear,
            ErrorMessage = ValidationConstants.InvalidRangeError)]
        public int Year { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.VinLength,
            MinimumLength = ValidationConstants.VinLength,
            ErrorMessage = ValidationConstants.InvalidVinError)]
        public string VinNumber { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.OwnerNameMaxLength,
            MinimumLength = ValidationConstants.OwnerNameMinLength,
            ErrorMessage = ValidationConstants.InvalidLengthError)]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = ValidationConstants.RequiredFieldError)]
        [StringLength(
            ValidationConstants.PhoneNumberMaxLength,
            MinimumLength = ValidationConstants.PhoneNumberMinLength,
            ErrorMessage = ValidationConstants.InvalidLengthError)]
        public string OwnerPhone { get; set; }

        public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();
    }
}