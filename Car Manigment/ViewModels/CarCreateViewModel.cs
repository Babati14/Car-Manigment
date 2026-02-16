using System.ComponentModel.DataAnnotations;
using static Car_Manigment.Common.ValidationConstants;  

namespace Car_Manigment.ViewModels.Cars
{
    public class CarCreateViewModel
    {
        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(CarBrandMaxLength, MinimumLength = CarBrandMinLength, ErrorMessage = InvalidLengthError)]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(CarModelMaxLength, MinimumLength = CarModelMinLength, ErrorMessage = InvalidLengthError)]
        public string Model { get; set; } = string.Empty;

        [Range(CarMinYear, CarMaxYear, ErrorMessage = InvalidRangeError)]
        public int Year { get; set; }

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(VinLength, MinimumLength = VinLength, ErrorMessage = InvalidVinError)]
        public string VinNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(OwnerNameMaxLength, MinimumLength = OwnerNameMinLength, ErrorMessage = InvalidLengthError)]
        public string OwnerName { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredFieldError)]
        [StringLength(PhoneNumberMaxLength, MinimumLength = PhoneNumberMinLength, ErrorMessage = InvalidLengthError)]
        public string OwnerPhone { get; set; } = string.Empty;

    }
}
