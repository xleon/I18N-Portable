namespace I18NPortable
{
	public static class Extensions
	{
		public static string Translate(this string key, params object[] args)
		{
			return I18N.Current.Translate(key, args);
		}
	}
}
