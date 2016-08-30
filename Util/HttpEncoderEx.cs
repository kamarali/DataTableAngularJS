using System;
using System.IO;
using System.Web;
using System.Web.Util;
using Microsoft.Security.Application;
using Encoder = Microsoft.Security.Application.Encoder;

namespace Iata.IS.Web.Util
{
  /// <summary>
  /// This class overrides the default encoding of ASP.NET. It uses Anti-XSS library for HTML encoding.
  /// </summary>
  public class HttpEncoderEx : HttpEncoder
  {
    protected override void HtmlEncode(string value, TextWriter output)
    {
      output.Write(Encoder.HtmlEncode(value));
    }

    protected override void HtmlAttributeEncode(string value, TextWriter output)
    {
      output.Write(Encoder.HtmlAttributeEncode(value));
    }

    protected override byte[] UrlEncode(byte[] bytes, int offset, int count)
    {
      //Can't call AntiXss library because the AntiXss library works with Unicode strings.
      //This override works at a lower level with just a stream of bytes, independent of 
      //the original encoding.

      //
      //Internal ASP.NET implementation reproduced below.
      //
      int cSpaces = 0;
      int cUnsafe = 0;

      // count them first
      for (int i = 0; i < count; i++)
      {
        char ch = (char)bytes[offset + i];

        if (ch == ' ')
          cSpaces++;
        else if (!IsUrlSafeChar(ch))
          cUnsafe++;
      }

      // nothing to expand?
      if (cSpaces == 0 && cUnsafe == 0)
        return bytes;

      // expand not 'safe' characters into %XX, spaces to +s
      byte[] expandedBytes = new byte[count + cUnsafe * 2];
      int pos = 0;

      for (int i = 0; i < count; i++)
      {
        byte b = bytes[offset + i];
        char ch = (char)b;

        if (IsUrlSafeChar(ch))
        {
          expandedBytes[pos++] = b;
        }
        else if (ch == ' ')
        {
          expandedBytes[pos++] = (byte)'+';
        }
        else
        {
          expandedBytes[pos++] = (byte)'%';
          expandedBytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
          expandedBytes[pos++] = (byte)IntToHex(b & 0x0f);
        }
      }

      return expandedBytes;


    }

    protected override string UrlPathEncode(string value)
    {
      //AntiXss.UrlEncode is too "pessimistic" for how ASP.NET uses UrlPathEncode

      //ASP.NET's UrlPathEncode splits the query-string off, and then Url encodes
      //the Url path portion, encoding any parts that are non-ASCII, or that
      //are <= 0x20 or >=0x7F.

      //Additionally, it is expected that:
      //                       UrPathEncode(string) == UrlPathEncode(UrlPathEncode(string))
      //which is not the case for UrlEncode.

      //The Url needs to be separated into individual path segments, each of which
      //can then be Url encoded.
      string[] parts = value.Split("?".ToCharArray());
      string originalPath = parts[0];

      string originalQueryString = null;
      if (parts.Length == 2)
        originalQueryString = "?" + parts[1];

      string[] pathSegments = originalPath.Split("/".ToCharArray());

      for (int i = 0; i < pathSegments.Length; i++)
      {
        pathSegments[i] = Encoder.UrlEncode(pathSegments[i]);  //this step is currently too aggressive
      }

      return String.Join("/", pathSegments) + originalQueryString;
    }

    private bool IsUrlSafeChar(char ch)
    {
      if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9')
        return true;

      switch (ch)
      {

        //These are the characters ASP.NET considers safe by default
        //case '-':
        //case '_':
        //case '.':
        //case '!':
        //case '*':
        //case '\'':
        //case '(':
        //case ')':
        //    return true;

        //Modified list based on what AntiXss library allows from the ASCII character set
        case '-':
        case '_':
        case '.':
          return true;
      }

      return false;
    }

    private char IntToHex(int n)
    {
      if (n <= 9)
        return (char)(n + (int)'0');
      else
        return (char)(n - 10 + (int)'a');
    }

    public string GetSafeHtmlFragment(string content)
    {

        const string newlinemarker = "¥";

        string escapedHtml = content.Replace("\n", newlinemarker);
        //SCP475771 - ICH-unable to add E10 as an aggregator
        string sanitized = Sanitizer.GetSafeHtmlFragment(escapedHtml.Replace(" ", "||||||"));
        //Text should not contain newlines so if the sanitizer has added any they can be removed
        sanitized = sanitized.Replace("\n", "").Replace("||||||", " "); 

        //Put newline back
        sanitized = sanitized.Replace(newlinemarker, "\n");
        //SCP442258 - SRM: processing dashboard -member name
        // Html Decode : function GetSafeHtmlFragment applies the string encoding , resulting escap character get converted into its hex value .
        // for ex: double quote(") to &quote; To make the input string as per the database content, applied decoding.

        sanitized = HttpUtility.HtmlDecode(sanitized);

        return sanitized;

    }

    /// <summary>
    /// NII: review changes to sanitize Request.Form input into safe html content
    /// SIS_SCR_REPORT_23_jun-2016_2: Cross Site Scripting
    /// </summary>
    /// <param name="value">input value</param>
    /// <returns>Sanitized string</returns>
    public string GetHtmlEncodeValue(string value)
    {
        return Encoder.HtmlEncode(value);
    }
  }
}
