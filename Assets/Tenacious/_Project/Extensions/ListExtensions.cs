using System.Collections.Generic;
using System.Text;

namespace Tenacious.Extensions
{
    public static class ListExtensions
    {
        public static string ToString<T>(this List<T> list, bool pretty = false)
        {
            if (!pretty)
                return list.ToString();
            else
            {
                StringBuilder strBuilder = new StringBuilder();

                strBuilder.Append("{[ ");
                int count = 0;
                foreach (T item in list)
                {
                    strBuilder.Append((count == 0 ? "" : ", ") + "\n\t" + count + " : " + item);
                    count++;
                }
                strBuilder.Append("\n]}");

                return strBuilder.ToString();
            }
        }
    }
}
