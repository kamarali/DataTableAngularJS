using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FileHelpers;

namespace Iata.IS.Model.Fdr.Base
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public class ExchangeRates
    {
        [FieldFixedLength(1)]
        [FieldConverter(ConverterKind.Int32)]
        public int TypeIdentifier;

        [FieldFixedLength(20)]
        public string CountryName;

        [FieldFixedLength(15)]
        public string CurrencyName;

        [FieldFixedLength(3)]
        [FieldConverter(ConverterKind.Int32)]
        public int NumericCurrencyCode;

        [FieldFixedLength(3)]
        public string CurrencyCode;

        [FieldFixedLength(7)]
        [FieldConverter(ConverterKind.Int32)]
        public int EuroToCurrencyCodeExchangeRateInteger;

        [FieldFixedLength(5)]
        [FieldConverter(typeof(NumericConverter))]
        public string EuroToCurrencyCodeExchangeRateFraction;

        [FieldFixedLength(7)]
        [FieldConverter(ConverterKind.Int32)]
        public int PoundSterlingToCurrencyCodeExchangeRateInteger;

        [FieldFixedLength(5)]
        [FieldConverter(typeof(NumericConverter))]
        public string PoundSterlingToCurrencyCodeExchangeRateFraction;

        [FieldFixedLength(7)]
        [FieldConverter(ConverterKind.Int32)]
        public int UsDollarToCurrencyCodeExchangeRateInteger;

        [FieldFixedLength(5)]
        [FieldConverter(typeof(NumericConverter))]
        public string UsDollarToCurrencyCodeExchangeRateFraction;
    }

   public class ErrorExchangeRates
   {
     public int NumericCurrencyCode { get; set;}
     public string CurrencyCode { get; set; }
     public int EuroToCurrencyCodeExchangeRateInteger { get; set; }
     public string EuroToCurrencyCodeExchangeRateFraction { get; set; }
     public int PoundSterlingToCurrencyCodeExchangeRateInteger { get; set; }
     public string PoundSterlingToCurrencyCodeExchangeRateFraction { get; set; }
     public int UsDollarToCurrencyCodeExchangeRateInteger { get; set; }
     public string UsDollarToCurrencyCodeExchangeRateFraction { get; set; }

   }

  public class NumericConverter : ConverterBase 
{
    private int number = 0;
	    public override object StringToField(string from) 
	    {
        Regex isnumber = new Regex("[^0-9]");
        if (!IsItNumber(from))
         {
           ThrowConvertException(from,"Not a valid number.");
         }
        return from; 
	    } 
	 
	 
	    public override string FieldToString(object fieldValue) 
	    {
        return fieldValue as string; 
	         
	        // a more elegant option that also works 
	        // return Convert.ToInt32(Convert.ToDecimal(fieldValue) * (10 ^ mDecimals)).ToString();  
	    }

      private bool IsItNumber(string inputvalue)
      {
        Regex isnumber = new Regex("[^0-9]");
        return !isnumber.IsMatch(inputvalue);
      }
	     
	} 
}
