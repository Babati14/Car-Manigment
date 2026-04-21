using System.Text.RegularExpressions;

namespace CarManigment.Common
{
    public static class SecurityHelpers
    {
        public static string SanitizeInput(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var noHtml = Regex.Replace(input, @"<[^>]*>", string.Empty);

            var sqlPatterns = new[]
            {
                @"('|(--)|;|\/\*|\*\/|xp_|sp_|exec|execute|union|select|insert|update|delete|drop|create|alter|declare|cast)",
            };

            var sanitized = noHtml;
            foreach (var pattern in sqlPatterns)
            {
                sanitized = Regex.Replace(sanitized, pattern, string.Empty, RegexOptions.IgnoreCase);
            }

            return sanitized.Trim();
        }

        public static bool IsValidInput(string? input, int maxLength = 500)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            if (input.Length > maxLength)
                return false;

            return Regex.IsMatch(input, @"^[\w\s\-.,()@]+$");
        }

        public static bool IsValidNumericInput(int? value, int min = int.MinValue, int max = int.MaxValue)
        {
            if (!value.HasValue)
                return true;

            return value.Value >= min && value.Value <= max;
        }
    }
}
