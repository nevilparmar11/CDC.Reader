using System.Linq;

namespace CDC.Reader.Core.Extensions
{
    public static class Extensions
    {
        public static string ConvertToString(this byte[] LSN)
        {
            if (LSN == null) return null;
            return "0x" + string.Join(string.Empty, LSN.Select(x => string.Format("{0:X2}", x)));
        }
    }
}
