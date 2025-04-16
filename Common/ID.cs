using System.Reflection;

namespace FinalSnack.Common
{
    public class ShaderID
    {
        // IDs

        public static readonly byte Outline = 0;


        // Extra ID stuff

        private static FieldInfo[] fields;
        public static string Search(byte id)
        {
            fields ??= typeof(ShaderID).GetFields(BindingFlags.Static | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.GetValue(null) is byte validID && validID == id)
                    return field.Name;
            }

            return null;
        }
    }
}
