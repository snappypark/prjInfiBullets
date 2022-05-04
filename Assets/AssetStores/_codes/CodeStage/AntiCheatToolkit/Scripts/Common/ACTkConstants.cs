#if UNITY_EDITOR
// allows to use internal methods from the editor code (Prefs editor window)
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Assembly-CSharp-Editor-firstpass")] // thx Daniele! ;)
#endif

namespace CodeStage.AntiCheat.Common
{
	internal class ACTkConstants
	{
		internal const string VERSION = "1.5.7.0";
		internal const string LOG_PREFIX = "[ACTk] ";
		internal const string DOCS_ROOT_URL = "http://codestage.net/uas_files/actk/api/";
	}
}