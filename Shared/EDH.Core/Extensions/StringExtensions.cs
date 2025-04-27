using System.Globalization;

namespace EDH.Core.Extensions;

public static class StringExtensions
{
	/// <summary>
	/// Parses a string to decimal using invariant culture, accepting both '.' and ',' as decimal separators.
	/// </summary>
	/// <param name="value">The string to parse.</param>
	/// <returns>The parsed decimal value.</returns>
	public static decimal ToDecimal(this string value)
	{
		if (String.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

		string normalized = value.Replace(',', '.');

		return Decimal.Parse(normalized, CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Tries to parse a string to decimal using invariant culture, accepting both '.' and ',' as decimal separators.
	/// </summary>
	/// <param name="value">The string to parse.</param>
	/// <param name="result">When this method returns, contains the parsed decimal value if successful; otherwise, 0.</param>
	/// <returns>true if the parsing was successful; otherwise, false.</returns>
	public static bool TryToDecimal(this string value, out decimal result)
	{
		if (String.IsNullOrWhiteSpace(value))
		{
			result = 0;
			return false;
		}

		string normalized = value.Replace(',', '.');

		return Decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
	}
}