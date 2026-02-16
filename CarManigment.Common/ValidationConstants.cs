namespace CarManigment.Common
{
    public static class ValidationConstants
    {
        //Car
        public const int CarBrandMinLength = 2;
        public const int CarBrandMaxLength = 50;

        public const int CarModelMinLength = 1;
        public const int CarModelMaxLength = 50;

        public const int CarMinYear = 1980;
        public const int CarMaxYear = 2100;

        public const int VinLength = 17;

        public const int OwnerNameMinLength = 2;
        public const int OwnerNameMaxLength = 100;

        public const int PhoneNumberMinLength = 6;
        public const int PhoneNumberMaxLength = 20;

        //MaintenanceService
        public const int ServiceTitleMinLength = 5;
        public const int ServiceTitleMaxLength = 100;

        public const int ServiceDescriptionMaxLength = 500;

        public const int MinEstimatedDurationHours = 1;
        public const int MaxEstimatedDurationHours = 48;

        public const int MinDailyCapacity = 1;
        public const int MaxDailyCapacity = 50;

        //ServiceType
        public const int ServiceTypeNameMinLength = 3;
        public const int ServiceTypeNameMaxLength = 50;

        //ServiceBooking
        public const int CustomerNameMinLength = 2;
        public const int CustomerNameMaxLength = 100;

        //Common Error Messages
        public const string RequiredFieldError =
            "This field is required.";

        public const string InvalidLengthError =
            "The field {0} must be between {2} and {1} characters long.";

        public const string InvalidRangeError =
            "The field {0} must be between {1} and {2}.";

        public const string InvalidVinError =
            "VIN number must be exactly 17 characters.";

        public const string PastDateError =
            "The selected date cannot be in the past.";

        public const string CapacityExceededError =
            "No available slots for this service on the selected date.";
    }
}
