using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beebyte.Obfuscator
{
	internal class Config
	{
		private static readonly string CSHARP_DLL = "Assembly-CSharp.dll";
		
//This pragma simply stops a warning message being produced about variables not being used
#pragma warning disable 414
		private static readonly string CSHARP_FIRSTPASS_DLL = "Assembly-CSharp-firstpass.dll";
		private static readonly string SOME_PLUGIN_DLL = "SomePluginName.dll";
#pragma warning restore 414

		internal static readonly string[] extraAssemblyDirectories = new string[]
		{
			/* 
			 * AssemblyResolutionException can sometimes be thrown when obfuscating.
			 * Although most of these have now been prevented, due to individual system configuartions it is sometimes
			 * necessary to add an assembly's directory here, so that it may be found.
			 */
			@"C:\Path\To\Assembly\Directory",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Runtime\4.0.20\lib\netcore50",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Collections\4.0.0\ref\netcore50",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Collections\4.0.10\lib\netcore50",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Threading\4.0.0\ref\netcore50",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Diagnostics.Debug\4.0.0\ref\netcore50",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Runtime.Extensions\4.0.0\ref\netcore50",
			@"C:\Program Files (x86)\Microsoft SDKs\NETCoreSDK\System.Runtime.Extensions\4.0.0\ref\dotnet"
		};

		/**
		 * DLLs that you want obfuscating should be declared here if they are created during the build process.
		 */
		internal static readonly string[] temporaryDLLs = new string[]
		{
			CSHARP_DLL
			//CSHARP_FIRSTPASS_DLL
		};

		/**
		 * DLLs that you want obfuscating should be declared here if they live within your Assets folder.
		 * These DLLs are first obfuscated in situ, then their original versions are restored at the end of the build process.
		 */
		internal static readonly string[] permanentDLLs = new string[]
		{
			//SOME_PLUGIN_DLL
		};
	}
}
