# Build
1. Delete these folders:
   - `/.mono`
   - `/Tools/bin`
   - `/Tools/obj`

You can also execute the `clean.groovy` script.

2. Click on rebuild from Godot_mono app. Godot will create these folders again.

# File organisation

## Godot dependencies

All .cs resources loaded by Godot are isolated in this two folders:

- `/Game/Managers/Autoload`: scripts loaded in the [autoload] section in `project.godot`
- `/Game/Controller`: scripts attached to Godot game objects.

So, the rest of the other C# classes placed in other folders than `/Games/Autoload` or `/GamesController` can be renamed or moved without any problem.
                                                             
## Projects
- `/Veronenger.csproj` is the main project, located in the root folder. Godot project name should be equals to the solution/project in order to work with Godot.
The main project exclude `/Tools` folder from compilation because it will use the `Tools` project as a Project Reference.
- `/Tools/Tools.csproj`: Used by main project, but the code inside can't use the main project code.

> The idea is: `Tools` can't see the main project, so they can be used in any other projects.

# Working features to document

- Player controller
- Staged camera
- Slope stairs
- Platforms
- Bus events
- Test runner
- Animations
- Resolution

