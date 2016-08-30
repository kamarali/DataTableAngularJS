using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Iata.IS.Core.Configuration;
using System.Xml.Serialization;
using System.Xml;

namespace Iata.IS.Core
{
  public static class ConvertUtil
  {
    /// <summary>
    /// Converts the GUID to string.
    /// </summary>
    /// <param name="guid">The GUID.</param>
    public static string ConvertGuidToString(Guid guid)
    {
      return ConvertByteToString(guid.ToByteArray());
    }

    /// <summary>
    /// To convert byte array into string
    /// </summary>
    /// <param name="byteArray"></param>
    public static string ConvertByteToString(byte[] byteArray)
    {
      if (byteArray == null)
      {
        throw new ArgumentNullException("byteArray");
      }

      // initiate string builder
      var sb = new StringBuilder();

      // iterate byte[]
      foreach (var t in byteArray)
      {
        // if bit value less then 16, append "0"
        if (t < 16)
        {
          sb.Append("0");
        }

        // append value from byte[]
        sb.Append(String.Format("{0:x}", t));
      }

      return sb.ToString().ToUpper();
    }

    /// <summary>
    /// Converts bytes to KB with 0 decimal places.
    /// </summary>
    /// <param name="numberOfBytes"></param>
    /// <returns></returns>
    public static long ConvertBytesToKB(long numberOfBytes)
    {
      return (long)Round((double)(numberOfBytes / 1024), 0);
    }

    /// <summary>
    /// Converts bytes to KB with 0 decimal places.
    /// </summary>
    /// <param name="numberOfBytes"></param>
    /// <returns></returns>
    public static long ConvertBytesToMB(long numberOfBytes)
    {
      return (long) Round((double) ((numberOfBytes/1024)/1024), 0);
    }

    /// <summary>
    /// This method will convert the Record Id in string format to Guid
    /// </summary>
    /// <param name="recordId">recordId</param>
    /// <returns>Guid</returns>
    public static Guid ConvertStringtoGuid(string recordId)
    {
      return new Guid(Enumerable.Range(0, recordId.Length).
                                 Where(x => 0 == x % 2).
                                 Select(x => Convert.ToByte(recordId.Substring(x, 2), 16)).
                                 ToArray());

    }

    public static MemoryStream GetMemoryStreamForMessage(string message)
    {
      var errorContent = Encoding.ASCII.GetBytes(message);
      var memoryStream = new MemoryStream();
      memoryStream.Write(errorContent, 0, errorContent.Length);
      memoryStream.Position = 0;
      return memoryStream;
    }

    /// <summary>
    /// This method rounds a decimal using MidpointRounding.AwayFromZero.
    /// </summary>
    /// <param name="valueToRound">A decimal number to be rounded.</param>
    /// <param name="decimals">The number of decimal places in the return value.</param>
    /// <returns></returns>
    public static decimal Round(decimal valueToRound, int decimals)
    {
      return Math.Round(valueToRound, decimals, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// This method rounds a double using MidpointRounding.AwayFromZero.
    /// </summary>
    /// <param name="valueToRound">A double-precision floating-point number to be rounded.</param>
    /// <param name="digits">The number of fractional digits in the return value.</param>
    /// <returns></returns>
    public static double Round(double valueToRound, int digits)
    {
      return Convert.ToDouble(Math.Round(Convert.ToDecimal(valueToRound), digits, MidpointRounding.AwayFromZero));
    }
   

    public static List<MailAddress> ConvertToMailAddresses(string emailAddresses, char addressSeperator = ';')
    {
      var mailAddressesList = new List<MailAddress>();

      if (!string.IsNullOrEmpty(emailAddresses))
      {
        emailAddresses = emailAddresses.Replace(",", ";");

        // Remove last character if present to avoid null email address
        if(emailAddresses.Substring(emailAddresses.Length-1, 1) ==";")
        {
          emailAddresses = emailAddresses.Substring(0, emailAddresses.Length - 1);
        }
        var emailIds = emailAddresses.Split(addressSeperator);

        if (emailIds.Length > 0)
        {
          mailAddressesList.AddRange(emailIds.Select(emailId => new MailAddress(emailId)));
        }
      }

      return mailAddressesList;
    }

    public static decimal TruncateToTwoDecimal(decimal fieldValue)
    {
      return (Math.Truncate(fieldValue * 100) / 100);
    }

    /// <summary>
    /// Converts string(i.e. FileId) to GUID and converts it to ByteArray.
    /// Iterates through ByteArray, convert it to Hexadecimal equivalent and appends each hex values to 
    /// create a string(i.e. FileId in Oracle GUID format)
    /// </summary>
    /// <param name="oracleGuid">fileId i.e. .net GUID Format</param>
    /// <returns>fileId string i.e. Oracle GUID format</returns>
    public static string ConvertNetGuidToOracleGuid(string oracleGuid)
    {
      // Convert string to Guid
      Guid netGuid = new Guid(oracleGuid);
      // Convert Guid to ByteArray
      byte[] guidBytes = netGuid.ToByteArray();
      // Create StringBuilder
      var oracleGuidBuilder = new StringBuilder();
      // Iterate through ByteArray, get Hex equivalent of each byte and append it to string
      foreach (var singleByte in guidBytes)
      {
        // Get Hex equivalent of each byte
        var hexEqui = singleByte.ToString("X");
        // Append each Hex equivalent to construct Guid.(Pad '0' to single byte)
        oracleGuidBuilder.Append(hexEqui.ToString().PadLeft(2, '0'));
      }// end foreach()

      // Return Guid string in Oracle format
      return oracleGuidBuilder.ToString();
    }// end ConvertNetGuidToOracleGuid()

    //CMP508:Audit Trail Download with Supporting Documents
    public static string SerializeXml(Object obj, Type objType)
    {
        
        // Assuming obj is an instance of an object
        var xmlSerializer = new XmlSerializer(objType);
        
        // Create a MemoryStream here, we are just working exclusively in memory
        System.IO.Stream stream = new System.IO.MemoryStream();

        // The XmlTextWriter takes a stream and encoding as one of its constructors
        System.Xml.XmlTextWriter xtWriter = new System.Xml.XmlTextWriter(stream, Encoding.UTF8);
        xmlSerializer.Serialize(xtWriter, obj);
        xtWriter.Flush();
        // Go back to the beginning of the Stream to read its contents
        stream.Seek(0, System.IO.SeekOrigin.Begin);
        // Read back the contents of the stream and supply the encoding
        System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
        string result = reader.ReadToEnd();
        reader.Close();
        //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
        xtWriter.Close();
        // Return Xml string
        return result;
    }

    public static object DeSerializeXml(string strXml, Type objType)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(strXml);
        //Assuming doc is an XML document containing a serialized object and objType is a System.Type set to the type of the object.
        XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement);
        XmlSerializer ser = new XmlSerializer(objType);
        object obj = ser.Deserialize(reader);
        // Then you just need to cast obj into whatever type it is eg:
        return obj;
    }

  }
}
