using System.Linq;
using System.Text;

namespace TestSysplan.Core
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, params object[] value)
        {
            return condition ? value.Aggregate(sb, (acc, curr) => acc.Append(curr)).AppendLine() : sb;
        }

        public static StringBuilder AppendIf(this StringBuilder sb, bool condition, params object[] value)
        {
            return condition ? value.Aggregate(sb, (acc, curr) => acc.Append(curr)) : sb;
        }

    }
}
