using System.Text.RegularExpressions;

namespace CarManigment.Common
{
    public static class SecurityHelpers
    {
        /// <summary>
        /// Sanitizes input by removing potentially dangerous characters while preserving valid input
        /// </summary>
        public static string SanitizeInput(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove HTML tags
            var noHtml = Regex.Replace(input, @"<[^>]*>", string.Empty);

            // Remove SQL keywords and special characters commonly used in SQL injection
            var sqlPatterns = new[]
            {
                @"('|(--)|;|\/\*|\*\/|xp_|sp_|exec|execute|union|select|insert|update|delete|drop|create|alter|declare|cast)",
            };

            var sanitized = noHtml;
            foreach (var pattern in sqlPatterns)
            {
                sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.IgnoreCase);
            }

            // Trim and limit length
            return sanitized.Trim();
        }

        /// <summary>
        /// Validates that a string contains only alphanumeric characters, spaces, and basic punctuation
        /// </summary>
        public static bool IsValidInput(string? input, int maxLength = 500)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            if (input.Length > maxLength)
                return false;

            // Allow letters, numbers, spaces, and common punctuation
            return Regex.IsMatch(input, @"^[\w\s\-.,()@]+$");
        }

        /// <summary>
        /// Validates numeric input is within safe range
        /// </summary>
        public static bool IsValidNumericInput(int? value, int min = int.MinValue, int max = int.MaxValue)
        {
            if (!value.HasValue)
                return true;

            return value.Value >= min && value.Value <= max;
        }
    }
}
