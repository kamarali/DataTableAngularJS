using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;



namespace Iata.IS.Core
{
  public class Crypto
  {
    private const string CryptoKey = "ABCDEFGHIJKLMNOPRSTUVWXYZ0123456";
    private const string InitializationVector = "0123456701234567";

   
    public static string EncryptString(string plainText)
    {

      Contract.Requires(!String.IsNullOrEmpty(plainText));

      // Check arguments.);)
      if (plainText == null || plainText.Length <= 0)
      {
        throw new ArgumentNullException("plainText");
      }

      var encoding = new System.Text.UTF8Encoding();
      var key = encoding.GetBytes(CryptoKey);
      var vector = encoding.GetBytes(InitializationVector);

      // Declare the streams used to encrypt to an in memory array of bytes.
      MemoryStream msEncrypt = null;
      CryptoStream csEncrypt = null;
      StreamWriter swEncrypt = null;

      // Declare the AesCryptoServiceProvider object used to encrypt the data.
      AesCryptoServiceProvider aesManagedAlg = null;

      try
      {
        // Create an AesCryptoServiceProvider object with the specified key and IV.
        aesManagedAlg = new AesCryptoServiceProvider
        {
          Key = key,
          IV = vector
        };

        // Create a decryptor to perform the stream transform.
        var encryptor = aesManagedAlg.CreateEncryptor(aesManagedAlg.Key, aesManagedAlg.IV);

        // Create the streams used for encryption.
        msEncrypt = new MemoryStream();
        csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        swEncrypt = new StreamWriter(csEncrypt);

        // Write all data to the stream.
        swEncrypt.Write(plainText);
      }
      finally
      {
        // Clean up.

        // Close the streams.
        if (swEncrypt != null)
        {
          swEncrypt.Close();
        }
        else if (csEncrypt != null)
        {
          csEncrypt.Close();
        }
        else if (msEncrypt != null)
        {
          msEncrypt.Close();
        }

        // Clear the AesCryptoServiceProvider object.
        if (aesManagedAlg != null)
        {
          aesManagedAlg.Clear();
        }
      }

      // Return the encrypted bytes from the memory stream.
      return ToString(msEncrypt.ToArray());
    }

    public static string DecryptString(string encryptedText)
    {
      // Check arguments.
      if (encryptedText == null || encryptedText.Length <= 0)
      {
        throw new ArgumentNullException("encryptedText");
      }

      var encoding = new System.Text.UTF8Encoding();
      var key = encoding.GetBytes(CryptoKey);
      var vector = encoding.GetBytes(InitializationVector);

      // Declare the streams used to decrypt to an in memory array of bytes.
      MemoryStream msDecrypt = null;
      CryptoStream csDecrypt = null;
      StreamReader srDecrypt = null;

      // Declare the AesCryptoServiceProvider object used to decrypt the data.
      AesCryptoServiceProvider aesManagedAlg = null;

      // Declare the string used to hold the decrypted text.
      string plaintext;

      try
      {
        // Create an AesCryptoServiceProvider object
        // with the specified key and IV.
        aesManagedAlg = new AesCryptoServiceProvider
        {
          Key = key,
          IV = vector
        };

        // Create a decryptor to perform the stream transform.
        var decryptor = aesManagedAlg.CreateDecryptor(aesManagedAlg.Key, aesManagedAlg.IV);

        // Create the streams used for decryption.
        msDecrypt = new MemoryStream(ToByteArray(encryptedText));
        csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        srDecrypt = new StreamReader(csDecrypt);

        // Read the decrypted bytes from the decrypting stream and place them in a string.
        plaintext = srDecrypt.ReadToEnd();
      }
      finally
      {
        // Clean up.

        // Close the streams.
        if (srDecrypt != null)
        {
          srDecrypt.Close();
        }
        else if (csDecrypt != null)
        {
          csDecrypt.Close();
        }
        else if (msDecrypt != null)
        {
          msDecrypt.Close();
        }

        // Clear the AesCryptoServiceProvider object.
        if (aesManagedAlg != null)
        {
          aesManagedAlg.Clear();
        }
      }

      return plaintext;
    }

    private static byte[] ToByteArray(string value)
    {
      return Convert.FromBase64String(value);
    }

    private static string ToString(byte []value)
    {
      return Convert.ToBase64String(value);
    }

    
 
    
  }
}