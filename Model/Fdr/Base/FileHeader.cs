using System;
using FileHelpers;

namespace Iata.IS.Model.Fdr.Base
{
    [FixedLengthRecord(FixedMode.ExactLength)]
    public class FdrFileHeader
    {
        [FieldFixedLength(1)] 
        [FieldConverter(ConverterKind.Int32)]
        public int TypeIdentifier;

        [FieldFixedLength(14)] 
        public string CopyRightInformation;

        [FieldFixedLength(8)]
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]  
        public DateTime FdrDate;

        [FieldFixedLength(9)]
        public string FdrVariation;
    }

}
