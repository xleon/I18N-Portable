namespace I18NPortable
{
	public static class Extensions
	{
		public static string Translate(this string key, params object[] args) => 
			I18N.Current.Translate(key, args);

		public static string CapitalizeFirstLetter(this string s)
		{
			if (string.IsNullOrEmpty(s)) return s;
			if (s.Length == 1) return s.ToUpper();
			return s.Remove(1).ToUpper() + s.Substring(1);
		}
	}
}
