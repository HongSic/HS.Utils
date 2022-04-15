using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HS.Utils
{
    public static class ArrayUtils
    {
        public static void PushAll<T>(this Stack<T> Stack, params T[] Params)
        {
            if (Params == null || Params.Length == 0) return;
            for (int i = 0; i < Params.Length; i++) Stack.Push(Params[i]);
        }
        public static void EnqueueAll<T>(this Queue<T> Queue, params T[] Params)
        {
            if (Params == null || Params.Length == 0) return;
            for (int i = 0; i < Params.Length; i++) Queue.Enqueue(Params[i]);
        }

        public static string HashMD5(this byte[] Data, bool UpperCase) { return Data == null ? null : HashMD5(Data, 0, Data.Length, UpperCase); }
        public static string HashMD5(this byte[] Data, int Offset, int Count, bool UpperCase)
        {
            if (Data == null) return null;

            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Data, Offset, Count);

                StringBuilder sb = new StringBuilder();
                //Convert the byte array to hexadecimal string
                for (int i = 0; i < hashBytes.Length; i++)
                    sb.Append(hashBytes[i].ToString(UpperCase ? "X2" : "x2"));
                return sb.ToString();
            }
        }
    }
}
