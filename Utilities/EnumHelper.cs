namespace Utilities
{
    #region

    using System;

    #endregion

    public static class EnumHelper
    {
        public static T ToEnum<T>(this int value)
        {
            return (T)Enum.Parse(typeof(T), value.ToString());
        }

        public static T FromString<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}