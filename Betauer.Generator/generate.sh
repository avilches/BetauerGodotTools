msbuild  Generator.sln /restore /t:Build "/p:Configuration=Debug" /v:normal "/l:GodotTools.BuildLogger.GodotBuildLogger,/Applications/Godot_mono.app/Contents/Resources/GodotSharp/Tools/GodotTools.BuildLogger.dll;/Users/avilches/Library/Application Support/Godot/mono/build_logs/9a5fa814d0209cde832d8df526a70ad1_Debug" /p:GodotTargetPlatform=osx
/Applications/Godot_mono.app/Contents/MacOS/Godot --path "/Users/avilches/Godot/Betauer.Generator" -s GeneratorScript.cs --no-window