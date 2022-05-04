Obfuscator v1.24.2 Copyright (c) 2015-2017 Beebyte Limited. All Rights Reserved


Usage
=====

The obfuscator is designed to work out of the box. When you build your project it automatically replaces the Assembly-CSharp.dll with an obfuscated version.

The default settings provide a good level of obfuscation, but it's worth looking at Options to enable or disable extra features (such as string literal obfuscation).

See https://youtu.be/Hk-iE9bpYLo for a video example.


Options
=======

From within Unity, select the ObfuscatorOptions asset in the Assets/Editor/Beebyte/Obfuscator directory.

From the Inspector window, you can now see the Obfuscation options available along with descriptions where relevant. The default settings provide a solid configuration that obfuscates the majority of your code, but here you have general control over what is obfuscated.


Code Attributes
===============

Methods often need to be left unobfuscated so that they can be referenced from an external plugin via reflection, or for some other reason. Or maybe you just want a field called "password" to appear as "versionId" when viewed by a decompiler.

You can achieve this by adding Attributes to your code. Have a look at Assets/Editor/Beebyte/Obfuscator/ObfuscatorExample.cs to see how this is done.

The standard System.Reflection.Obfuscation attribute is supported.

The following Beebyte specific attributes are supported:

[SkipRename]                - The obfuscator will not rename this class/method/field, but will continue to obfuscate its contents (if relevant).
[Skip]                      - The obfuscator will not rename this class/method/field, nor will it obfuscate its contents.
[Rename("MyRenameExample")] - The obfuscator will rename this class/method/field to the specified name. Class renames can accept a namespace rename, i.e. [Rename("MyNamespace.MyClass")]
[ReplaceLiteralsWithName]   - If the target is obfuscated, then any literals with the exact original name will be converted to use the new obfuscated name. e.g. if method MyMethod is renamed to AAABBB, then any string literals of exactly "MyMethod" anywhere in your code will be adjusted to be "AAABBB" instead.
[ObfuscateLiterals]         - Any string literals found within this method will be instructed to be obfuscated. This is a partial replacement to the clunky way of surrounding strings with '^' characters.
[DoNotFake]                 - Fake code will not be generated for this class/method.


Troubleshooting F.A.Q
=====================

Q. After obfuscating, my 3rd party plugin has stopped working! It has scripts that aren't in the Plugins folder.

A. The simplest way to fix this is to look at the plugin's script to see what namespace they use. Then, towards the bottom of the inspector window in ObfuscatorOptions.asset there is an array called "Skip Namespaces". Add the plugin's namespace to this array and the obfuscator will ignore any matching namespaces. Occassionally a plugin will forget to use namespaces for its scripts, in which case you have three choices: Either move them into a Plugins folder, or annotate each class with [Beebyte.Obfuscator.Skip], or add each class name to the "Skip Classes" array.


Q. After obfuscating, my 3rd party plugin has stopped working! It only has scripts in the Plugins folder.

A. The obfuscator won't have touched files that live in the Plugins folder, however it's likely that the plugin at some point required you to create a specifically named class and/or method. You'll need to add a [SkipRename] attribute to the class and/or method you created in your code.


Q. Button clicks don't work anymore!

A. Check your Options and see if you enabled the "Include public mono methods". If you did, then make sure you've added a [SkipRename] attribute to the button click method.


Q. How do I get string literal encryption to work, my secret field is still showing as plaintext in a decompiler?

A. You need to take the following steps:
     - Enable "Obfuscate Literals" in the ObfuscatorOptions asset.
     - Either leave the default Unicode to 94 (the ^ character), or change it as required.
     - In your code, surround the string with the chosen character, e.g. "secretpass" becomes "^secretpass^";

   Alternatively, if the string is within a method you have another option:
     - Enable "Obfuscate Literals" in the ObfuscatorOptions asset.
     - Decorate the method with [ObfuscateLiterals]       (using Beebyte.Obfuscator;)


Q. It's not working for a certain platform.

A. Regardless of the platform, send me an email (beebyte.limited@gmail.com) with the error and I'll see what I can do, but remember that it's only officially tested for Standalone, WebPlayer, and Android platforms.


Q. How can we run obfuscation later in the build process?

A. You can control this in the Assets/Editor/Beebyte/Obfuscator/Postbuild.cs script. The PostProcessScene attribute on the Obfuscate method has an index number that you can freely change to enable other scripts to be called first.


Q. Can I obfuscate externally created DLLs?

A. You can. To do this open Assets/Editor/Beebyte/Obfuscator/ObfuscatorMenuExample.cs. Uncomment this file and change the DLL filepath to point to your DLL. Now use the newly created menu option.


Q. How do I obfuscate local variables?

A. Local variable names are not stored anywhere, so there is nothing to obfuscate. A decompiler tries to guess a local variable's name based on the name of its class, or the method that instantiated it.


Q. I'm getting ArgumentException: The Assembly UnityEditor is referenced by obfuscator ('Assets/ThirdParty/Beebyte/Obfuscator/Plugins/obfuscator.dll'). But the dll is not allowed to be included or could not be found.

A. It's important to keep the directory structure of the obfuscator package. Specifically, the correct location should have an "Editor" folder somewhere on its path. Unity treats files within Editor folders differently. Assets/Editor/Beebyte will ensure that the obfuscator can correctly run and that the obfuscator tool itself won't be included in your production builds.


Q. I'm getting an AssemblyResolutionException:Failed to resolve assembly: 'System.Collections, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' when building for Windows Phone.

A. In the example given in the question, somewhere on your system would be a System.Collections.dll file of version 4.0.0.0. To fix the error, locate this file then add its location to the extraAssemblyDirectories array at the top of the Assets/Editor/Beebyte/Obfuscator/Postbuild.cs file.


Q. Is is possible to obfuscate certain public MonoBehaviour methods even when I've told it not to obfuscate them by default?

A. Yes, you override this by using the attribute [System.Reflection.Obfuscation(Exclude=false)] on the method.


Q. What can I do to make it more secure?

A. Try enabling obfuscation of public MonoBehaviour methods and obfuscation of MonoBehaviour class names. Enable string obfuscation and add [ObfuscateLiterals] to sensitive methods containing strings you'd prefer people not to see in a decompiler. Refactor your code to use smaller methods. Try using a different unicode starting character - instead of 65 (A) you could use 770 (Vertical text) - just be aware that makes stack traces harder to read.


Q. Something's still not working, how can I get in touch with you?

A. Please email me at beebyte.limited@gmail.com giving as much information about the problem as possible.



Notable 3rd Party Plugins (Beebyte is not affiliated with these products or companies)
======================================================================================

    * Anti-Cheat Toolkit (by Code Stage)
      This plugin focuses on preventing code tampering and memory manipulation and complements the Obfuscator well. No changes are needed and ACTK_EXCLUDE_OBFUSCATION does not need to be defined, so simply install and enjoy!

    * NGUI 2.7 (by Tasharen Entertainment)

        Default settings already handle this, but make sure you are deriving names from cryptographic hashes (in Naming Policies), then add the following method names to the "Replace Literals" section within ObfuscatorOptions.asset:

          OnHover
          OnSelect
          OnInput
          OnScroll
          OnKey
          OnPress
          OnDrag
          OnClick
          OnDoubleClick
          OnDrop
          OnTooltip

    * Behavior Designer (by Opsive)
      Shared variables need to have the same name, so you can either annotate these with [SkipRename] or [Rename("someCrypticName")], or alternatively use the cyptographic hash option which guarantees that all names map to a unique consistent value.
      Tasks you create should have their classes annotated with [SkipRename].
      If you use behaviorTree.GetVariable("MyVariable"), or the equivalent Set methods, then you either need to add [SkipRename] or [ReplaceLiteralsWithName] on the variable definition.

    * UFPS (by Opsive)
      Enable "Derive obfuscated names from cryptographic hash". Then inside Preserve Prefixes, add the UFPS reflection callbacks:
	OnMessage_
	OnValue_
	OnAttempt_
	CanStart_
	CanStop_
	OnStart_
	OnStop_
	OnFailStart_
	OnFailStop_

      Where you use vp_Timer.CancelAll("SomeMethod"), either add [SkipRename] or [ReplaceLiteralsWithName] on the SomeMethod definition.

      If you choose to exclude the core UFPS scripts from obfuscation, make sure you add [SkipRename] on method events that originate from the core UFPS, i.e. If you create a class and define a method OnStart_Reload, you probably want to use [SkipRename] on that method. Note that if the class you created explicitly inherits from the original UFPS class then this step is not required.

    * Photon Networking
      Photon uses ToString() often with its enums, so if you choose to obfuscate enums, make sure to skip the enums in PhotonNetwork/Enums.cs. For a more obfuscated approach you could annotate each enum value with [SkipRename] instead.

Update History
==============

1.24.2 - 17th March 2017

    * Fixed a rare TypeLoadException.
    * Fixed a "redefinition of parameter" error in IL2CPP when a delegate's parameter names are inferred from default declaration i.e. delegate { .. } instead of an explicit delegate(bool a, bool b) { .. }.

1.24.1 - 12th February 2017

    * Fixed a bug where a coroutine invoked by name from a different class would fail to execute if the coroutine is not annotated with [SkipRename] or [ReplaceLiteralsWithName]. This also fixes coroutines launched from within coroutines.

1.24.0 - 27th January 2017

    * Multiple DLLs can be simultaneously obfuscated by declaring them within Config.cs. Endpoints in the referenced DLL are updated in the calling DLL. This new feature is experimental and has no interface.
    * Fixed a TypeLoadException that happened when a class extending an external class implements a custom interface that declares one of the external methods and where the implementing class doesn't override it.
    * Optimisation.

1.23.9 - 13th November 2016

    * Optimisation - Method obfuscation is now really quick compared to previous lengthy times seen with some projects that focused heavily on reflection (mostly NGUI).
    * Extra information added to the progress bar for method obfusation.
    * Forcing obfuscation on serialized fields with [System.Reflection.Obfuscation(Exclude=false)] was sometimes being ignored.

1.23.8 - 3rd November 2016

    * Fixed an ArgumentException that could happen when obfuscating literals using the simplified non-RSA algorithm.

1.23.7 - 31st October 2016

    * Added hidden option to provide canonical names of attributes to be searched for strings that add to the array of method names to be treated as [ReplaceLiteralsWithName].
    * Custom attributes now have their string arguments searched for when considering [ReplaceLiteralsWithName].
    * Added option to include fake code method names to the nameTranslation file.
    * Added new Attribute to exclude fake code being created from particular methods or classes.

1.23.6 - 14th September 2016

    * Fixed a compile NullReferenceException under certain conditions when cryptographic hashes are disabled.

1.23.5 - 4th September 2016

    * Fixed IL2CPP compile error that, for some methods, would occur when using both string obfuscation and fake code generation.
    * The "Use RSA" setting is no longer transient.

1.23.4 - 25th August 2016

    * Fixed IL2CPP compile error that could sometimes happen when fake code generation is enabled.
    * Fixed a NullReferenceException with RSA string obfuscation that could consistently appear under certain conditions.

1.23.3 - 12th August 2016

    * Fixed a "NotSupportedException: The invoked member is not supported in a dynamic module." exception when using dynamically created libraries.

1.23.2 - 9th August 2016

    * Fixed an IL2CPP compile error when fake code tries to clone extern methods.

1.23.1 - 8th August 2016

    * Improvements to Fake Code generation.

1.23.0 - 27th July 2016

    * Added new hidden options includeParametersOnPublicMethods and includeParametersOnProtectedMethods, set to true by default. Useful if you're giving an API to someone.
    * Improved scanning for Unity reflection methods.
    * Updated ObfuscatorMenuExample.cs
    * Added Photon defaults for when Obfuscating MonoBehaviour class names are enabled.

1.22.1 - 25th July 2016

    * Obfuscating MonoBehaviour class names occassionally didn't get reverted due to a Unity editor delegate being reset. A more stable approach is now taken where a temporary translation file is produced, then read back in at the earliest opportunity.

1.22.0.1 - 25th July 2016

    * Fixed an issue with 1.22.0 for Unity 5.2+ that expected ObfuscatorOptions to be in a fixed location. It can once again live in a non-standard location.

1.22.0 - 19th July 2016

    * Option to strip namespaces.
    * Experimental option to obfuscate MonoBehaviour class names.
    * Option to supply strings of method names that should have [ReplaceLiteralsWithName] applied.
    * Added defaults for UFPS (Preserve Prefixes) and NGUI (Replace Literals).
    * ObfuscatorOptions now installs in a temporary location and will ask to overwrite or keep existing settings when next run.

1.21.2 - 16th June 2016

    * Fixed missing scripts errors in WebPlayer builds that occured when using Fake Code generation on methods that declare default arguments.

1.21.1 - 15th June 2016

    * Fixed extremely long obfuscation times when using fake code generation on huge methods. There is a new option to limit the size of methods to be faked. This should bring obfuscation times back down to seconds from potentially hours.

1.21.0 - 15th June 2016

    * Added default settings for Photon networking compatibility.

1.20.0 - 15th June 2016

    * Added a progress bar.

1.19.0 - 10th June 2016

    * Enum constants can now be skipped by default.

1.18.4 - 2nd June 2016

    * Fixed a bug where iOS builds could fail to compile in xcode if using the default String Literal Obfuscation method.

1.18.3 - 1st June 2016

    * Using [System.Reflection.Obfuscation(Exclude=false)] on a class will now cause it to be obfuscated even if its namespace is explicity skipped in Options.

1.18.2 - 30th May 2016

    * Fixed a bug with string literal obfuscation where a string of length greater than 1/8th the chosen RSA key length would appear garbled.

1.18.1 - 23rd May 2016

    * Changed the default Unicode start character to 65 (A) to work around an error with structs in iOS builds for certain unpatched versions of Unity.
    * Changed defaults to use hash name generation instead of order based.

1.18.0 - 18th May 2016

    * New "Preserve Prefixes" section that can keep part of a name, i.e. OnMessage_Victory() -> OnMessage_yzqor(). This is mostly for the reflection used by the UFPS plugin.
    * New option to reverse the line order of the nameTranslation.txt. This is now the default.
    * New option to surround every obfuscated name with a specified delimiter.
    * Parameter obfuscation is now included in the nameTranslation.txt.
    * New hidden option to include a simplified HASHES section in the nameTranslation.txt.

1.17.1 - 6th May 2016

    * Fixed an ArgumentOutOfRangeException that could occur when the minimum number of fake methods is set to a large value.

1.17.0 - 5th May 2016

    * Optimisation.
    * Option to derive obfuscated names from a cryptographically generated hash. This means names will be consistent throughout a project's lifecycle, removing the need to maintain up to date nameTranslation.txt files.
    * New attribute [ObfuscateLiterals] for methods that instructs all string literals within it to be obfuscated, without requiring delimiters within the literals.
    * New option to toggle whether attributes should be cloned for fake methods.

1.16.2 - 1st April 2016

    * ScriptableObject classes are now treated as Serializable by default (i.e. fields and properties are not renamed). This can be overriden by setting options.treatScriptableObjectsAsSerializable to false, or on a case-by-case basis by making use of [System.Reflection.Obfuscation] on each field, or [System.Reflection.Obfuscation(Exclude = false, ApplyToMembers = true)] on the class.
    * Fixed a TypeLoadException for methods such as public abstract T Method() where a deriving class creates a method replacing the generic placeholder, i.e. public override int Method().
    * Added hidden option for classes to inherit Beebyte attributes Skip and SkipRename that are on an ancestor class. To use, set options.inheritBeebyteAttributes = true prior to obfuscation.

1.16.1 - 23rd March 2016

    * Fixed an issue with 1.16.0 where internal methods would not obfuscate.

1.16.0 - 10th March 2016

    * New option to obfuscate Unity reflection methods instead of simply skipping them.
    * Methods replacing string literals that share the same name now also share the same obfuscated name, so that replaced literals correctly point to their intended method.
    * Faster obfuscation time for methods.
    * Fixed a TypeLoadException.
    * The name translation file now has a consistent order.

1.15.0 - 25th February 2016

    * Added option to include skipped classes and namespaces when searching for string literal replacement via [ReplaceLiteralsWithName] or through the RPC option.
    * Fixed a bug where classes within skipped namespaces could sometimes have their references broken if they link to obfuscated classes.

1.14.0 - 25th February 2016

    * Added option to search SkipNamespaces recursively (this was the default behaviour)
    * Added option to restrict obfuscation to user-specified Namespaces.

1.13.1 - 24th February 2016

    * Fixed a NullReferenceException that could sometimes (very rarely but consistent) occur during the write process.
    * Removed the dependance of the version of Mono.Cecil shipped with Unity by creating a custom library. This is necessary to avoid "The imported type `...` is defined multiple times" errors.

1.13.0 - 23rd February 2016

    * Added a master "enabled" option to easily turn obfuscation on/off through scripts or the GUI.
    * Add Fake Code is now available for WebPlayer builds.
    * Fixed a "UnityException: Failed assemblies stripper" exception that occurs when selecting both fake code and player preference stripping levels.
    * Improvements to Fake Code generation.
    * Obfuscation times can now be printed with a call to Obfuscator.SetPrintChronology(..).
    * Building a project with no C# scripts will no longer cause an error to occur.

1.12.0 - 12th February 2016

    * Added finer control to exclude public or protected classes, methods, properties, fields, events from being renamed. This might be useful to keep a DLLs public API unchanged, for it to then be used in another project.
    * Fixed a bug in the Options inspector that could revert some changes after an option's array's size is altered.

1.11.0 - 3rd February 2016

    * Added an option to specify annotations that should be treated in the same way as [RPC], to cater for third party RPC solutions.

1.10.0 - 28th January 2016

    * Previous obfuscation mappings can now be reused.
    * Unity Networking compatibility (old & new).
    * [RPC] annotated methods are no longer renamed unless an explicit [Rename("newName")] attribute is added, or if an option is enabled.
    * A new [ReplaceLiteralsWithName] attribute exists that can be applied to classes, methods, properties, fields, and events. It should be used with caution since it searches every string literal in the entire assembly replacing any instance of the old name with the obfuscated name. This is useful for certain situations such as replacing the parameters in nView.RPC("MyRPCMethod",..) method calls. It may also be useful for reflection, but note that only exact strings are replaced.

1.9.0 - 23rd January 2016

    * Added a new option "Skip Classes" that is equivalent to adding [Skip] to a class. It's a good long-term solution for 3rd party assets that place files outside of a Plugin directory in the default namespace.
    * Added a way to resolve any future AssemblyResolutionExceptions via the Postscript.cs file without requiring a bespoke fix from Beebyte.
    * Fixed a bug in Postscript.cs for Unity 4.7
    * Added a workaround for an unusual Unity bug where the strings within the Options asset would sometimes be turned into hexadecimals, most noticable when swapping build targets often.

1.8.4 - 7th January 2016

    * Fixed an AssemblyResolutionException for UnityEngine that could sometimes occur.

1.8.3 - 6th January 2016

    * Obfuscation attributes can now be applied to Properties.

1.8.2 - 6th January 2016

    * Serializable classes now retain their name, field names, and property names without requiring an explicit [Skip].
    * Fixed issues using generics that extend from MonoBehaviour.
    * Fixed an issue where two identically named methods in a type could cause obfuscation to fail if one is a generic, i.e. A() , A<T>().
    * Fixed an issue where fake code generation could sometimes result in a failed obfuscation when generic classes are involved.

1.8.1 - 1st January 2016

    * Fixed various issues using generics.
    * Fixed an AssemblyResolutionException for UnityEngine.UI when using interfaces from that assembly.

1.8.0 - 29th December 2015

    * Properties can now be obfuscated.

1.7.3 - 29th December 2015

    * Undocumented fix.

1.7.2 - 28th December 2015

    * Fixed a TypeLoadException error.
    * Fixed an issue with inheritence.
    * Undocumented fix.

1.7.1 - 27th December 2015

    * Fixed a TypeLoadException error.
    * Fixed a "Script behaviour has a different serialization layout when loading." error.
    * Private Fields marked with the [SerializeField] attribute are now preserved by default.
    * Classes extending from Unity classes that have MonoBehaviour as an ancestor were not being treated as such (i.e. UIBehaviour).

1.7.0.1 - 14th December 2015

    * Unity 5.3 compatibility.

1.7.0 - 11th December 2015

    * Improved the "Works first time" experience by searching for reflection methods referenced by certain Unity methods such as StartCoroutine. New option added to disable this feature.
    * WebPlayer support (string literal obfuscation, public field obfuscation, fake code are disabled for WebPlayer builds).
    * Added an option to strip string literal obfuscation markers from strings when choosing not to use string literal obfuscation.
    * ObfuscatorMenuExample.cs added showing how you can Obfuscate other dlls from within Unity.
    * Added protection against accidently obfuscating the same dll multiple times.
    * Added an advanced option to skip rename of public MonoBehaviour fields.

1.6.1 - 16th November 2015

    * First public release

