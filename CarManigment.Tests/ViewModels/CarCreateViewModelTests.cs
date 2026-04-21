using Xunit;
using System.ComponentModel.DataAnnotations;
using Car_Manigment.ViewModels.Cars;

namespace CarManigment.Tests.ViewModels
{
    public class CarCreateViewModelTests
    {
        #region Valid Model Tests

        [Fact]
        public void CarCreateViewModel_WithValidData_PassesValidation()
        {

            var model = new CarCreateViewModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Year = 2020,
                VinNumber = "1HGBH41JXMN109186",
                OwnerName = "John Doe",
                OwnerPhone = "123-456-7890"
            };

            var validationResults = ValidateModel(model);

            Assert.Empty(validationResults);
        }

        #endregion

        #region Brand Validation Tests

        [Fact]
        public void CarCreateViewModel_WithEmptyBrand_FailsValidation()
        {

            var model = CreateValidModel();
            model.Brand = string.Empty;

            var validationResults = ValidateModel(model);

            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Brand"));
        }

        [Fact]
        public void CarCreateViewModel_WithTooShortBrand_FailsValidation()
        {

            var model = CreateValidModel();
            model.Brand = "A";

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("Brand"));
        }

        [Fact]
        public void CarCreateViewModel_WithTooLongBrand_FailsValidation()
        {

            var model = CreateValidModel();
            model.Brand = new string('A', 51);

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("Brand"));
        }

        [Fact]
        public void CarCreateViewModel_WithValidBrandLengths_PassesValidation()
        {
            AssertValidBrandLength("AB");
            AssertValidBrandLength("Toyota");
            AssertValidBrandLength(new string('A', 50));
        }

        #endregion

        #region Model Validation Tests

        [Fact]
        public void CarCreateViewModel_WithEmptyModel_FailsValidation()
        {

            var model = CreateValidModel();
            model.Model = string.Empty;

            var validationResults = ValidateModel(model);

            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Model"));
        }

        [Fact]
        public void CarCreateViewModel_WithValidModelName_PassesValidation()
        {
            // Arrange & Act & Assert
            AssertValidModelName("Camry");
            AssertValidModelName("F-150");
            AssertValidModelName("Model 3");
        }

        #endregion

        #region Year Validation Tests

        [Theory]
        [InlineData(1979)]
        [InlineData(1900)]
        [InlineData(2101)]
        [InlineData(2200)]
        public void CarCreateViewModel_WithInvalidYear_FailsValidation(int year)
        {

            var model = CreateValidModel();
            model.Year = year;

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("Year"));
        }

        [Theory]
        [InlineData(1980)]
        [InlineData(2000)]
        [InlineData(2020)]
        [InlineData(2024)]
        [InlineData(2100)]
        public void CarCreateViewModel_WithValidYear_PassesValidation(int year)
        {

            var model = CreateValidModel();
            model.Year = year;

            var validationResults = ValidateModel(model);

            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Year"));
        }

        #endregion

        #region VIN Validation Tests

        [Fact]
        public void CarCreateViewModel_WithEmptyVIN_FailsValidation()
        {

            var model = CreateValidModel();
            model.VinNumber = string.Empty;

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("VinNumber"));
        }

        [Theory]
        [InlineData("SHORT")]
        [InlineData("123456789012345")]
        [InlineData("12345678901234567890")]
        public void CarCreateViewModel_WithInvalidVINLength_FailsValidation(string vin)
        {

            var model = CreateValidModel();
            model.VinNumber = vin;

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("VinNumber"));
        }

        [Fact]
        public void CarCreateViewModel_WithValidVIN_PassesValidation()
        {

            var model = CreateValidModel();
            model.VinNumber = "1HGBH41JXMN109186";

            var validationResults = ValidateModel(model);

            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("VinNumber"));
        }

        #endregion

        #region Owner Name Validation Tests

        [Fact]
        public void CarCreateViewModel_WithEmptyOwnerName_FailsValidation()
        {

            var model = CreateValidModel();
            model.OwnerName = string.Empty;

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("OwnerName"));
        }

        [Fact]
        public void CarCreateViewModel_WithValidOwnerName_PassesValidation()
        {

            var model = CreateValidModel();
            model.OwnerName = "John Doe";

            var validationResults = ValidateModel(model);

            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("OwnerName"));
        }

        #endregion

        #region Owner Phone Validation Tests

        [Fact]
        public void CarCreateViewModel_WithEmptyOwnerPhone_FailsValidation()
        {

            var model = CreateValidModel();
            model.OwnerPhone = string.Empty;

            var validationResults = ValidateModel(model);

            Assert.Contains(validationResults, v => v.MemberNames.Contains("OwnerPhone"));
        }

        [Theory]
        [InlineData("123-456-7890")]
        [InlineData("1234567890")]
        [InlineData("(123) 456-7890")]
        public void CarCreateViewModel_WithValidPhoneFormats_PassesValidation(string phone)
        {

            var model = CreateValidModel();
            model.OwnerPhone = phone;

            var validationResults = ValidateModel(model);

            Assert.True(validationResults.Count == 0 || 
                       !validationResults.Any(v => v.MemberNames.Contains("OwnerPhone")));
        }

        #endregion

        #region Helper Methods

        private CarCreateViewModel CreateValidModel()
        {
            return new CarCreateViewModel
            {
                Brand = "Toyota",
                Model = "Camry",
                Year = 2020,
                VinNumber = "1HGBH41JXMN109186",
                OwnerName = "John Doe",
                OwnerPhone = "123-456-7890"
            };
        }

        private List<ValidationResult> ValidateModel(CarCreateViewModel model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        private void AssertValidBrandLength(string brand)
        {
            var model = CreateValidModel();
            model.Brand = brand;
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Brand"));
        }

        private void AssertValidModelName(string modelName)
        {
            var model = CreateValidModel();
            model.Model = modelName;
            var validationResults = ValidateModel(model);
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains("Model"));
        }

        #endregion

        #region Multiple Field Validation Tests

        [Fact]
        public void CarCreateViewModel_WithMultipleInvalidFields_ReturnsMultipleErrors()
        {

            var model = new CarCreateViewModel
            {
                Brand = "", 
                Model = "", 
                Year = 1900, 
                VinNumber = "SHORT", 
                OwnerName = "", 
                OwnerPhone = "" 
            };

            var validationResults = ValidateModel(model);

            Assert.NotEmpty(validationResults);
            Assert.True(validationResults.Count >= 5);
        }

        [Fact]
        public void CarCreateViewModel_DefaultValues_FailsValidation()
        {

            var model = new CarCreateViewModel();

            var validationResults = ValidateModel(model);

            Assert.NotEmpty(validationResults);
        }

        #endregion
    }
}

