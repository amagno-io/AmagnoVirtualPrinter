using System.Text;

namespace AmagnoVirtualPrinter.Utils
{
    public static class ErrorHandling
    {
        public static string ObjectToString(this object obj)
        {
            if (obj == null) return string.Empty;

            var type = obj.GetType();
            var properties = type.GetProperties();
            var fields = type.GetFields();

            var sb = new StringBuilder();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj) ?? "null";
                sb.AppendLine($"{property.Name}: {value}");
            }

            foreach (var field in fields)
            {
                var value = field.GetValue(obj) ?? "null";
                sb.AppendLine($"{field.Name}: {value}");
            }

            return sb.ToString();
        }
    }
}