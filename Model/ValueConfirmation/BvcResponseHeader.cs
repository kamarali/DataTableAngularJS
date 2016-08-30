using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Iata.IS.Model.ValueConfirmation
{
  [FixedLengthRecord] 
  public class BvcResponseHeader
  {
    //file header field
    [FieldFixedLength(3)]
    public string Apid;
  
    [FieldFixedLength(3)]
    public string Sour;
    [FieldFixedLength(100)]
    public string Filler;
    [FieldFixedLength(8)]
    public string Trad;
    [FieldFixedLength(10)]
    public string Recc;
    [FieldFixedLength(50)]
    public string Vcfk;
    [FieldFixedLength(226)]
    public string Filler2;
  
  }
}
