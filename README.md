### Rebuild
1. Delete these folders:
   - `/.mono`
   - `/Tests/bin`
   - `/Tests/obj`
   - `/Tools/bin`
   - `/Tools/obj`
2. Click on rebuild from Godot_mono. Godot will create them again.

### Godot dependencies

- `/Game/Autoload`: scripts loaded in the [autoload] section in `project.godot`
- `/Game/Controller`: scripts attached to Godot game objects.

Scripts placed in other folders than /Games/Autoload or /GamesController can be renamed or moved without any problem.
                                                             
### Projects
- `/Veronenger.csproj` is the main project, located in the root folder. Name should be equals to the solution in order to work with Godot.
The main project excludes `/Tests` and `/Tools` folder from compilation (it tries to compile everything in the root), but it still uses the `Tools` project as Project Reference.
- `/Tools/Tools.csproj`: Used by main project, but the code inside can't use the main project code.
- `/Tests/Tests.csproj`: Additional project where units tests are located. It uses NUnit. It can use main or tools projects.
