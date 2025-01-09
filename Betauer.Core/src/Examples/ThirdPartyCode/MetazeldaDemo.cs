using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Examples.ThirdPartyCode.Metazelda;
using Betauer.Core.Examples.ThirdPartyCode.Metazelda.constraints;
using Betauer.Core.Examples.ThirdPartyCode.Metazelda.generators;
using Godot;

namespace Betauer.Core.Examples.ThirdPartyCode;

/**
* If you just want to use the Metazelda library, this is the only file you need to modify / replace.
*/
public class MetazeldaDemo {
    public static int boxWidth = 20;
    public static int boxHeight = 4;

    public static void Main() {
        MZSpaceMap spaceMap = new MZSpaceMap();
        int width = 5;
        int height = 5;

        for (var y = 0; y < width; y++) {
            for (var x = 0; x < height; x++) {
                spaceMap.Set(new Vector2I(x, y), true);
            }
        }
        SpaceConstraints constraints = new SpaceConstraints(spaceMap);
        constraints.SetMaxKeys(3);
        constraints.SetMaxSwitches(2);
        constraints.SetMaxSpaces(25);

        var generator = new MZDungeonGenerator(5, constraints);
        generator.OnAdd = () => {
            PrintMaze(width, height, generator);
        };
        generator.Generate();

        PrintMaze(width, height, generator);
    }

    private static void PrintMaze(int width, int height, MZDungeonGenerator generator) {
        var dungeon = generator.GetMZDungeon();

        var map = new Dictionary<Vector2I, MZRoom>();
        dungeon.GetRooms().ForEach(room => map[room.GetCenter()] = room);

        var str = new TextCanvas();
        for (var y = 0; y < width; y++) {
            for (var x = 0; x < height; x++) {
                var r = PrintRoom(map.ContainsKey(new Vector2I(x, y)) ? map[new Vector2I(x, y)] : null);
                str.Write(x * boxWidth, y * boxHeight, r);
            }
        }

        Console.WriteLine(str.GetText().Trim());
    }

    public static String PrintRoom(MZRoom? room) {
        if (room == null) {
            var l = ". . . . . . . . . . . . . . . . . . . .".Substring(0, boxWidth);
            return Enumerable.Repeat(l, boxHeight).Aggregate((a, b) => a + "\n" + b);
        }

        char path = '█';
        var str = new TextCanvas($"""
                                  {"".PadRight(boxWidth - 1, '\u2500')}+
                                  {"".PadRight(boxWidth - 1, ' ')}│
                                  {"".PadRight(boxWidth - 1, ' ')}│
                                  {"".PadRight(boxWidth - 1, ' ')}│
                                  """);
        
        var up = room.GetEdge(Vector2I.Up);
        var left = room.GetEdge(Vector2I.Left);
        var right = room.GetEdge(Vector2I.Right);
        var down = room.GetEdge(Vector2I.Down);
        if (down != null) {
            str.WriteCentered(boxWidth / 2, 2, ""+path);
            str.WriteCentered(boxWidth / 2, 3, ""+path);
        }

        if (left != null) {
            str.WriteCentered(boxWidth / 2, 2, ""+path);
            if (left.HasSymbol()) {
                // str.Write(0, 1, " ");
                str.Write(0, 2, (left.Symbol.ToString() + "?").PadRight(boxWidth / 2, path));
                // str.Write(0, 3, " ");
            } else {
                // str.Write(0, 1, " ");
                str.Write(0, 2, new string(path, boxWidth / 2));
                // str.Write(0, 3, " ");
            }
        }

        if (right != null) {
            str.WriteCentered(boxWidth / 2, 2, ""+path);
            // str.Write(boxWidth - 1, 1, " ");
            str.WriteEnd(boxWidth, 2,  new string(path, boxWidth / 2));
            // str.Write(boxWidth - 1, 3, " ");
        }

        if (up != null) {
            if (up.HasSymbol()) {
                str.WriteCentered(boxWidth / 2, 0, "|" + (up.Symbol.ToString() + "?").PadCenter(6) + "|");
            } else {
                str.WriteCentered(boxWidth / 2, 0, "│  █  │");
            }
            str.WriteCentered(boxWidth / 2, 1, ""+path);
        }
        var item = room.GetItem();
        if (item != null) {
            var c = "";
            if (item.IsKey()) {
                c = "(" + room.GetItem() + ")";
            } else {
                c = " " + room.GetItem() + " ";
            }
            str.WriteCentered(boxWidth / 2, 2, c);
        }
        
        str.Write(0, 1, room.GetPrecond().GetKeyLevel().ToString());

        return str.GetText();
    }

    public static void PrintRoomChildren(MZRoom room, int indentLevel = 0) {
        string indent = new string(' ', indentLevel * 2);
        // if (room.G)        

        Console.WriteLine($"{indent}{room.id}" + (room.GetItem() != null ? $" {room.GetItem()}" : "") + " (" + room.GetPrecond().GetKeyLevel() + ")");

        foreach (var child in room.GetChildren()) {
            PrintRoomChildren(child, indentLevel + 1);
        }
    }
}


/*
public class MZUnityBehaviourExample {
public MZDungeonGenerator generator;

private readonly Color[] keyColors = { Colors.Blue, Colors.Yellow, Colors.Magenta, Colors.Cyan, Colors.Red, Colors.Green };

void Start() {
    float roomRatio = 0.6875f; // 256x176
    var random = new Random(1);

    // Use CountConstraints to make a truly random map.
    //CountConstraints constraints = new CountConstraints(50, 3, 3);

    // Use SpaceConstraints to make a map fitting to a shape.
    MZSpaceMap spaceMap = new MZSpaceMap();
    // tileMap = tileMapObjects[random.Range(0, tileMapObjects.Length)].GetComponentInChildren<Tilemap>();
    // foreach (Vector3I posWithZ in tileMap.cellBounds.allPositionsWithin.GetEnumerator()) {
    //     if (tileMap.HasTile(posWithZ)) {
    //         Vector2I pos = new Vector2I(posWithZ.X, posWithZ.Y);
    //         spaceMap.Set(pos, true);
    //     }
    // }
    SpaceConstraints constraints = new SpaceConstraints(spaceMap);

    generator = new MZDungeonGenerator(1, constraints);
    generator.Generate();
    IMZDungeon dungeon = generator.GetMZDungeon();
    foreach (MZRoom room in dungeon.GetRooms()) {
        MZSymbol item = room.GetItem();
        GameObject toInstantiate = normalRoom;
        Color roomColor = new Color((float)room.GetIntensity(), 1.0f - (float)room.GetIntensity(), 0.5f - (float)room.GetIntensity() / 2);
        if (item != null) {
            switch (item.GetValue()) {
                case (int)MZSymbol.MZSymbolValue.Start:
                    toInstantiate = entranceRoom;
                    roomColor = Colors.White;
                    break;

                case (int)MZSymbol.MZSymbolValue.Boss:
                    toInstantiate = bossRoom;
                    break;

                case (int)MZSymbol.MZSymbolValue.Goal:
                    toInstantiate = goalRoom;
                    roomColor = Colors.White;
                    break;

                default:
                    break;
            }

            if (item.GetValue() >= 0) {
                GameObject keyObjectInstance = Instantiate(key, new Vector3(room.GetCoords()[0].X, room.GetCoords()[0].Y * roomRatio, 0), Quaternion.identity, transform);
                keyObjectInstance.GetComponent<SpriteRenderer>().color = keyColors[item.GetValue()];
                keyObjectInstance.transform.localScale += new Vector3(2, 2, 2);
            } else if (item.GetValue() == (int)MZSymbol.MZSymbolValue.Switch) {
                GameObject keyObjectInstance = Instantiate(roomSwitch, new Vector3(room.GetCoords()[0].X, room.GetCoords()[0].Y * roomRatio, 0), Quaternion.identity, transform);
                keyObjectInstance.transform.localScale += new Vector3(2, 2, 2);
            }
        }

        GameObject roomObject = Instantiate(toInstantiate, new Vector3(room.GetCoords()[0].X, room.GetCoords()[0].Y * roomRatio, 0), Quaternion.identity, transform);
        roomObject.GetComponent<SpriteRenderer>().color = roomColor;

        foreach (MZEdge edge in room.GetEdges()) {
            MZRoom targetRoom = dungeon.Get(edge.GetTargetRoomId());
            Vector2I edgeDir = targetRoom.GetCoords()[0] - room.GetCoords()[0];

            toInstantiate = openDoor;
            GameObject keyObject = null;
            Color keyColor = Color.white;
            if (edge.GetSymbol() != null) {
                switch (edge.GetSymbol().GetValue()) {
                    case (int)MZSymbol.MZSymbolValue.SwitchOn:
                        toInstantiate = blockedDoor;
                        keyObject = roomSwitchOn;
                        break;

                    case (int)MZSymbol.MZSymbolValue.SwitchOff:
                        toInstantiate = blockedDoor;
                        keyObject = roomSwitchOff;
                        break;

                    default:
                        break;
                }

                if (edge.GetSymbol().GetValue() >= 0) {
                    toInstantiate = lockedDoor;
                    keyObject = key;
                    keyColor = keyColors[edge.GetSymbol().GetValue()];
                }
            }

            // This only works for identically sized rooms on a grid.
            Vector3 pos = Vector3.zero;
            if (edgeDir == Vector2I.right) {
                pos = new Vector3(room.GetCoords()[0].X + 0.5f, room.GetCoords()[0].Y * roomRatio, 0);
                GameObject doorObject = Instantiate(toInstantiate, pos, Quaternion.identity, transform);
                if (keyObject != null) {
                    GameObject keyObjectInstance = Instantiate(keyObject, pos, Quaternion.identity, transform);
                    keyObjectInstance.GetComponent<SpriteRenderer>().color = keyColor;
                    keyObjectInstance.transform.localScale += new Vector3(1, 1, 1);
                }
            } else if (edgeDir == Vector2I.down) {
                pos = new Vector3(room.GetCoords()[0].X, room.GetCoords()[0].Y * roomRatio - (roomRatio / 2), 0);
                var relativePos = Vector2I.zero + Vector2I.up;
                var angle = Mathf.Atan2(relativePos.Y, relativePos.X) * Mathf.Rad2Deg;
                var rotation = Quaternion.AngleAxis(angle, Vector3.forward); // 90 degrees
                GameObject doorObject = Instantiate(toInstantiate, pos, rotation, transform);
                if (keyObject != null) {
                    GameObject keyObjectInstance = Instantiate(keyObject, pos, Quaternion.identity, transform);
                    keyObjectInstance.GetComponent<SpriteRenderer>().color = keyColor;
                    keyObjectInstance.transform.localScale += new Vector3(1, 1, 1);
                }
            }
        }
    }
}

void Update() {
}
}
*/