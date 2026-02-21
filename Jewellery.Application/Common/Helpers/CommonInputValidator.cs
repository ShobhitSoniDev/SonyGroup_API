using System;
using System.Linq;
using System.Text.RegularExpressions;
using Jewellery.Domain.Entities;

public static class CommonInputValidator
{
    public static ResponseModel Validate(
        string value,
        bool numeric = true,
        bool allowDecimal = true,
        int maxDecimalPlaces = 2,
        int minLength = 1,
        int maxLength = 50
    )
    {
        // null / empty not allowed
        if (string.IsNullOrWhiteSpace(value))
            return new ResponseModel
            {
                Code = 0,
                Data = null,
                Message = "Value cannot be empty or null"
            };

        // ❌ negative not allowed
        if (numeric && value.StartsWith("-"))
            return new ResponseModel
            {
                Code = 0,
                Data = null,
                Message = "Negative value not allowed"
            };

        /* =========================
           NUMERIC MODE
        ==========================*/
        if (numeric)
        {
            if (!decimal.TryParse(value, out var parsedValue))
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = "Invalid numeric value"
                };

            if (parsedValue == 0)
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = "Zero value not allowed"
                };

            var regex = allowDecimal
                ? new Regex(@"^[0-9.]+$")
                : new Regex(@"^[0-9]+$");

            if (!regex.IsMatch(value))
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = allowDecimal
                        ? "Only numbers and decimal allowed"
                        : "Only numbers allowed"
                };

            // only one decimal
            if (allowDecimal && value.Count(c => c == '.') > 1)
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = "Only one decimal allowed"
                };

            var parts = value.Split('.');
            var decPart = parts.Length > 1 ? parts[1] : null;

            // max digits (excluding decimal)
            var digitCount = value.Replace(".", "").Length;
            if (digitCount > maxLength)
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = $"Maximum {maxLength} digits allowed"
                };

            // max decimal places
            if (decPart != null && decPart.Length > maxDecimalPlaces)
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = $"Only {maxDecimalPlaces} decimal places allowed"
                };

            // starting zero not allowed
            if (value.StartsWith("0"))
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = "Starting zero not allowed"
                };
        }
        else
        {
            /* =========================
               ALPHANUMERIC MODE
            ==========================*/
            var charRegex = new Regex(@"^[a-zA-Z0-9 ]+$");
            if (!charRegex.IsMatch(value))
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = "Special characters are not allowed"
                };

            if (value.Length < minLength)
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = $"Minimum {minLength} characters required"
                };

            if (value.Length > maxLength)
                return new ResponseModel
                {
                    Code = 0,
                    Data = null,
                    Message = $"Maximum {maxLength} characters allowed"
                };
        }

        // ✅ Success
        return new ResponseModel
        {
            Code = 1,
            Data = value,
            Message = "Success"
        };
    }
}
