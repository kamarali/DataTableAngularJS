using System;
using System.Text;
using System.Security.Cryptography;

namespace Iata.IS.Web.Util
{
  public class CryptoWrapper
  {


    public static string DecryptAes(string strTextToDecrypt, string strKey)
    {


      byte[] bytIn = null;
      SymmetricAlgorithm objCryptoService = null;

      var hashMd5 = new MD5CryptoServiceProvider();

      var encrypto = default(ICryptoTransform);
      
      try
      {

        objCryptoService = new AesManaged
                             {
                               Key = hashMd5.ComputeHash(Encoding.ASCII.GetBytes(strKey)),
                               Mode = CipherMode.ECB
                             };
        encrypto = objCryptoService.CreateDecryptor();
        //// Convert the strTextToEncode String to a byte array 
        bytIn = Convert.FromBase64String(strTextToDecrypt);

        //bytIn = Encoding.ASCII.GetBytes(strTextToDecrypt);
        // Transform and return the string. 

        return Encoding.ASCII.GetString(encrypto.TransformFinalBlock(bytIn, 0, bytIn.Length));
      }
      catch (Exception exception)
      {
        throw;
      }

    }

    public static string EncryptAes(string strTextToEncrypt, string strKey)
    {


      byte[] bytIn = null;
      SymmetricAlgorithm objCryptoService = null;

      ICryptoTransform encrypto = default(ICryptoTransform);

      var hashMd5 = new MD5CryptoServiceProvider();

      try
      {

        objCryptoService = new AesManaged
                             {
                               Key = hashMd5.ComputeHash(Encoding.ASCII.GetBytes(strKey)),
                               Mode = CipherMode.ECB
                             };
        encrypto = objCryptoService.CreateEncryptor();


        //// Convert the strTextToEncode String to a byte array 
        bytIn = Encoding.ASCII.GetBytes(strTextToEncrypt);

        return Convert.ToBase64String(encrypto.TransformFinalBlock(bytIn, 0, bytIn.Length));
      }


      catch (Exception exception)
      {
        throw;
      }

    }
    
  }
}
