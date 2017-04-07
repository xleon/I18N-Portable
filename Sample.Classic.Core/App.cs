using System.Reflection;
using I18NPortable;

namespace Sample.Classic.Core
{
	public class App
	{
		public App()
		{
		    I18N
                .Current
                .Init(GetType().GetTypeInfo().Assembly);
		}
	}
}
