using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace BillsManagementWebApp.Shared
{
    public class PasswordHasher
    {
        public static string GenerateHashForUser(string strPassword)
        {
            byte[] arrBytesSalt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(arrBytesSalt);
            Rfc2898DeriveBytes objRfc2898DeriveBytes = new Rfc2898DeriveBytes(strPassword, arrBytesSalt, 10000);
            byte[] arrBytesHashed = objRfc2898DeriveBytes.GetBytes(20);
            byte[] arrBytesWholeHash = new byte[36];
            Array.Copy(arrBytesSalt, 0, arrBytesWholeHash, 0, 16);
            Array.Copy(arrBytesHashed, 0, arrBytesWholeHash, 16, 20);
            return Convert.ToBase64String(arrBytesWholeHash);
        }

        public static bool CompareHashes(string strPassword, string strHashedPassword)
        {               
            byte[] arrBytesWholeHash = Convert.FromBase64String(strHashedPassword);
            byte[] arrBytesSalt = new byte[16];
            Array.Copy(arrBytesWholeHash, 0, arrBytesSalt, 0, 16);
            Rfc2898DeriveBytes objRfc2898DeriveBytes = new Rfc2898DeriveBytes(strPassword, arrBytesSalt, 10000);
            byte[] arrBytesHash = objRfc2898DeriveBytes.GetBytes(20);
            for (int i = 0; i < 20; i++)
            {
                if (arrBytesWholeHash[i + 16] != arrBytesHash[i])
                {
                    return false;
                }
            }

            return true;
        }


    }
}