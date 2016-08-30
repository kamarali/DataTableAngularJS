using System;
using System.Text.RegularExpressions;

namespace Iata.IS.Core
{
  public static class Validators
  {
    /// <summary>
    /// Function to test for Positive Integers with zero inclusive.
    /// </summary>
    /// <param name="number">String to be validated as a number.</param>
    /// <returns>True if whole number, false otherwise.</returns>
    public static bool IsWholeNumber(string number)
    {
      if (String.IsNullOrEmpty(number))
      {
        return false;
      }

      var pattern = new Regex(@"(^\d*\.?\d*[0-9]+\d*$)|(^[0-9]+\d*\.\d*$)");
      return pattern.IsMatch(number);
    }

    public static bool IsAlphaNumeric(string value)
    {
      if (String.IsNullOrEmpty(value))
      {
        return false;
      }

      var pattern = new Regex(@"(^[a-zA-Z0-9]*$)");
      return pattern.IsMatch(value);
    }

    /// <summary>
    /// Checks whether the argument is a valid email address.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns>True if valid, false otherwise.</returns>
    public static bool IsValidEmailId(string emailAddress)
    {
      if (String.IsNullOrEmpty(emailAddress))
      {
        return false;
      }

      const string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        
      var expression = new Regex(pattern);
      return expression.IsMatch(emailAddress);
    }

    /// <summary>
    /// Determines whether the specified input value is zero.
    /// </summary>
    /// <param name="inputValue">The input value.</param>
    /// <returns>
    /// 	<c>true</c> if the specified input value is zero; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsZero(string inputValue)
    {
      if (String.IsNullOrEmpty(inputValue))
      {
        return false;
      }

      var integerValue = new Regex("^[0-0]*$");

      return integerValue.IsMatch(inputValue);
    }
  }
}