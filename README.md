### Rebuild
1. Delete these folders:
   - `/.mono`
   - `/Tests/bin`
   - `/Tests/obj`
2. Click on rebuild from Godot_mono. Godot will create them again.

### Organization

- `/Game`: C# scripts.
- `/Game/Autoload`: scripts loaded in the [autoload] section in `project.godot`
- `/Game/Controller`: scripts attached to Godot game objects.

Scripts placed in other folders than /Games/Autoload or /GamesController can be renamed or moved without any problem.

- `/Tests`: additional project where units tests are located.
