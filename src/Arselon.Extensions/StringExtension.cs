namespace Arselon.Extensions
{
    public static class StringExtension
    {
        public static void CopyTo(this string s, byte[] destination)
        {
            CopyTo(s, destination, 0);
        }

        public static void CopyTo(this string s, byte[] destination,
            int index)
        {
            int l = s.Length;
            for (int i = 0; i < l; i++)
                destination[index + i] = (byte)s[i];
        }

        public static byte[] ToByteArray(this string s)
        {
            var data = new byte[s.Length];
            CopyTo(s, data, 0);
            return data;
        }


    }
}
