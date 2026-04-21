using Xunit;
using CarManigment.Common;

namespace CarManigment.Tests.Common
{
    public class SecurityHelpersTests
    {
        #region SanitizeInput Tests

        [Fact]
        public void SanitizeInput_WithNull_ReturnsEmptyString()
        {
            string? input = null;

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SanitizeInput_WithEmptyString_ReturnsEmptyString()
        {
            var input = "";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SanitizeInput_WithWhitespace_ReturnsEmptyString()
        {
            var input = "   ";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void SanitizeInput_WithCleanText_ReturnsUnchangedText()
        {
            var input = "Toyota Camry 2020";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.Equal("Toyota Camry 2020", result);
        }

        [Fact]
        public void SanitizeInput_WithHtmlTags_RemovesHtmlTags()
        {
            var input = "<script>alert('XSS')</script>";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("<", result);
            Assert.DoesNotContain(">", result);
        }

        [Fact]
        public void SanitizeInput_WithMultipleHtmlTags_RemovesAllTags()
        {
            var input = "<div><p>Hello</p><span>World</span></div>";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.Equal("HelloWorld", result);
        }

        [Fact]
        public void SanitizeInput_WithSqlInjectionAttempt_RemovesSqlKeywords()
        {
            var input = "'; DROP TABLE Cars; --";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("DROP", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("--", result);
        }

        [Fact]
        public void SanitizeInput_WithUnionSelect_RemovesSqlKeywords()
        {
            var input = "1' UNION SELECT * FROM Users --";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("UNION", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("SELECT", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithInsertStatement_RemovesSqlKeywords()
        {
            var input = "'; INSERT INTO Users VALUES ('admin', 'pass');";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("INSERT", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithUpdateStatement_RemovesSqlKeywords()
        {
            var input = "'; UPDATE Users SET password='hacked';";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("UPDATE", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithDeleteStatement_RemovesSqlKeywords()
        {
            var input = "'; DELETE FROM Users;";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("DELETE", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithStoredProcedure_RemovesSqlKeywords()
        {
            var input = "'; EXEC sp_executesql;";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("EXEC", result, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("sp_", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithLeadingTrailingSpaces_TrimsSpaces()
        {
            var input = "   Toyota Camry   ";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.Equal("Toyota Camry", result);
        }

        [Fact]
        public void SanitizeInput_WithAlterStatement_RemovesSqlKeywords()
        {
            var input = "'; ALTER TABLE Cars;";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("ALTER", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithCreateStatement_RemovesSqlKeywords()
        {
            var input = "'; CREATE TABLE Hacked;";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("CREATE", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithDeclareStatement_RemovesSqlKeywords()
        {
            var input = "'; DECLARE @var;";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("DECLARE", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeInput_WithCastFunction_RemovesSqlKeywords()
        {
            var input = "'; CAST(1 AS INT);";

            var result = SecurityHelpers.SanitizeInput(input);

            Assert.DoesNotContain("CAST", result, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region IsValidInput Tests

        [Fact]
        public void IsValidInput_WithNull_ReturnsTrue()
        {
            string? input = null;

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithEmptyString_ReturnsTrue()
        {
            var input = "";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithWhitespace_ReturnsTrue()
        {
            var input = "   ";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithValidText_ReturnsTrue()
        {
            var input = "Toyota Camry 2020";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithSpecialCharactersAllowed_ReturnsTrue()
        {
            var input = "Test-Name_123 (2020) @email.com";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithHtmlTags_ReturnsFalse()
        {
            var input = "<script>alert('XSS')</script>";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.False(result);
        }

        [Fact]
        public void IsValidInput_WithSqlInjection_ReturnsFalse()
        {
            var input = "'; DROP TABLE Cars;";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.False(result);
        }

        [Fact]
        public void IsValidInput_ExceedingMaxLength_ReturnsFalse()
        {
            var input = new string('a', 501);

            var result = SecurityHelpers.IsValidInput(input, 500);

            Assert.False(result);
        }

        [Fact]
        public void IsValidInput_WithinMaxLength_ReturnsTrue()
        {
            var input = new string('a', 500);

            var result = SecurityHelpers.IsValidInput(input, 500);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithCustomMaxLength_ValidatesCorrectly()
        {
            var input = "Short";

            var result = SecurityHelpers.IsValidInput(input, 10);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithCustomMaxLengthExceeded_ReturnsFalse()
        {
            var input = "This is a longer string";

            var result = SecurityHelpers.IsValidInput(input, 10);

            Assert.False(result);
        }

        [Fact]
        public void IsValidInput_WithInvalidCharacters_ReturnsFalse()
        {
            var input = "Test$Invalid%Chars&";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.False(result);
        }

        [Fact]
        public void IsValidInput_WithNumbers_ReturnsTrue()
        {
            var input = "12345";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithDots_ReturnsTrue()
        {
            var input = "test.value";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithCommas_ReturnsTrue()
        {
            var input = "test,value";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithParentheses_ReturnsTrue()
        {
            var input = "test(value)";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithHyphen_ReturnsTrue()
        {
            var input = "test-value";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithUnderscore_ReturnsTrue()
        {
            var input = "test_value";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidInput_WithAtSymbol_ReturnsTrue()
        {
            var input = "test@value";

            var result = SecurityHelpers.IsValidInput(input);

            Assert.True(result);
        }

        #endregion

        #region IsValidNumericInput Tests

        [Fact]
        public void IsValidNumericInput_WithNull_ReturnsTrue()
        {
            int? input = null;

            var result = SecurityHelpers.IsValidNumericInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_WithinRange_ReturnsTrue()
        {
            int? input = 2020;

            var result = SecurityHelpers.IsValidNumericInput(input, 1900, 2100);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_BelowMin_ReturnsFalse()
        {
            int? input = 1800;

            var result = SecurityHelpers.IsValidNumericInput(input, 1900, 2100);

            Assert.False(result);
        }

        [Fact]
        public void IsValidNumericInput_AboveMax_ReturnsFalse()
        {
            int? input = 2200;

            var result = SecurityHelpers.IsValidNumericInput(input, 1900, 2100);

            Assert.False(result);
        }

        [Fact]
        public void IsValidNumericInput_AtMinBoundary_ReturnsTrue()
        {
            int? input = 1900;

            var result = SecurityHelpers.IsValidNumericInput(input, 1900, 2100);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_AtMaxBoundary_ReturnsTrue()
        {
            int? input = 2100;

            var result = SecurityHelpers.IsValidNumericInput(input, 1900, 2100);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_WithDefaultRange_ValidatesCorrectly()
        {
            int? input = 0;

            var result = SecurityHelpers.IsValidNumericInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_NegativeWithinRange_ReturnsTrue()
        {
            int? input = -50;

            var result = SecurityHelpers.IsValidNumericInput(input, -100, 100);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_NegativeBelowMin_ReturnsFalse()
        {
            int? input = -150;

            var result = SecurityHelpers.IsValidNumericInput(input, -100, 100);

            Assert.False(result);
        }

        [Fact]
        public void IsValidNumericInput_WithMaxInt_ValidatesCorrectly()
        {
            int? input = int.MaxValue;

            var result = SecurityHelpers.IsValidNumericInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_WithMinInt_ValidatesCorrectly()
        {
            int? input = int.MinValue;

            var result = SecurityHelpers.IsValidNumericInput(input);

            Assert.True(result);
        }

        [Fact]
        public void IsValidNumericInput_ZeroWithinRange_ReturnsTrue()
        {
            int? input = 0;

            var result = SecurityHelpers.IsValidNumericInput(input, -10, 10);

            Assert.True(result);
        }

        #endregion
    }
}
