using System.Text;

namespace Core.Tools
{
    public static class LongExtension
    {
        static readonly char[] URL_SAFE_ALPHABET = [
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2',
            '3', '4', '5', '6', '7', '8', '9', '0', '-', '_'];
        public static string ToEncodedString(this long userId)
        {
            StringBuilder ret = new();
            long offset = userId % URL_SAFE_ALPHABET.Length;
            while (userId > 0)
            {
                ret.Append(URL_SAFE_ALPHABET[offset + userId % 10]);
                userId /= 10;
            }
            return ret.ToString();
        }
    }
}
