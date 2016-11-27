using System;
using System.Reflection;

namespace I18NPortable
{
	public static class Extensions
	{
		public static string Translate(this string key, params object[] args) 
            => I18N.Current.Translate(key, args);

		public static string TranslateOrNull(this string key, params object[] args) 
            => I18N.Current.TranslateOrNull(key, args);

		public static string CapitalizeFirstCharacter(this string s)
		{
			if (string.IsNullOrEmpty(s))
                return s;

			if (s.Length == 1)
                return s.ToUpper();

			return s.Remove(1).ToUpper() + s.Substring(1);
		}

	    public static string UnescapeLineBreaks(this string str)
	        => str
                .Replace("\\r\\n", Environment.NewLine)
                .Replace("\\n", Environment.NewLine);

	    public static string Translate(this Enum value)
	    {
	        var fieldInfo = value.GetType().GetRuntimeField(value.ToString());
	        var fieldName = fieldInfo.FieldType.Name;

	        return $"{fieldName}.{value}".Translate();
	    }

	    public static string Translate(this object instance)
	    {
	        var nameTranslation = instance.GetType().Name.TranslateOrNull();
	        if (nameTranslation != null)
	            return nameTranslation;

            var fullNameTranslation = instance.GetType().FullName.TranslateOrNull();
	        return fullNameTranslation ?? instance.GetType().Name.Translate();
	    }
	}
}