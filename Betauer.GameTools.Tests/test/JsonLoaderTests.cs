using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests;

[TestRunner.Test]
public class JsonLoaderTests {
    public string SaveName1 = "a";
    public string SaveName2 = "b";

    [TestRunner.TearDown]
    [TestRunner.SetUp]
    public void TearDown() {
        File.Delete($"{SaveName1}.metadata");
        File.Delete($"{SaveName1}.data");
        File.Delete($"{SaveName2}.metadata");
        File.Delete($"{SaveName2}.data");
    }

    
    [TestRunner.Test]
    public async Task ListNoDataTest() {
        var loader = new MyJsonGameLoader();
        var list = await loader.ListSaveGames();
        Assert.That(list, Is.Empty);
    }

    public class MySaveGame : SaveGame {
        [JsonInclude] public string MyString { get; set; } = "Hello World";
    }

    [TestRunner.Test(Description = "Error, no data neither metadata")]
    public async Task LoadMissingMetadataAndDataTest() {
        var loader = new MyJsonGameLoader();
        var mySaveGame = await loader.LoadMetadataFile(SaveName1);
        Assert.That(mySaveGame.MyString, Is.EqualTo("Hello World"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.SavegameNotFound));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(0));
        Assert.That(mySaveGame.GameObjects, Is.Null);
    }

    [TestRunner.Test(Description = "Error, metadata exists but no data")]
    public async Task LoadMissingDataTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MySaveGame() {
            MyString = "yu"
        };
        await loader.Save(saveGame, new List<SaveObject>(), SaveName1, null);
        
        new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Delete();

        var mySaveGame = await loader.LoadMetadataFile(SaveName1);
        Assert.That(mySaveGame.MyString, Is.EqualTo("yu"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.SavegameNotFound));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(0));
        Assert.That(mySaveGame.GameObjects, Is.Null);
        
        mySaveGame = await loader.Load(SaveName1, null);
        Assert.That(mySaveGame.MyString, Is.EqualTo("yu"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.SavegameNotFound));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(0));
        Assert.That(mySaveGame.GameObjects, Is.Null);

    }

    [TestRunner.Test(Description = "Error, metadata exists but corrupted data nothing happens, until the real data is read")]
    public async Task LoaCorruptedDataTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MySaveGame() {
            MyString = "yu"
        };
        await loader.Save(saveGame, new List<SaveObject>(), SaveName1, null);

        File.WriteAllLines(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data"), new[] { "corrupted data" });
        
        var mySaveGame = await loader.LoadMetadataFile(SaveName1);
        Assert.That(mySaveGame.MyString, Is.EqualTo("yu"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects, Is.Null);
        
        mySaveGame = await loader.Load(SaveName1, null);
        Assert.That(mySaveGame.MyString, Is.EqualTo("yu"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.SaveGameError));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects, Is.Null);

    }

    [TestRunner.Test(Description = "Missing metadata rebuild the metadata")]
    public async Task LoadMissingMetadataOnlyTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MySaveGame() {
            MyString = "yu"
        };
        await loader.Save(saveGame, new List<SaveObject>(), SaveName1, null);

        var metadataFile = new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".metadata"));
        metadataFile.Delete();
        Assert.That(metadataFile.Exists, Is.False);
        
        var mySaveGame = await loader.LoadMetadataFile(SaveName1);
        metadataFile.Refresh();
        Assert.That(metadataFile.Exists, Is.True);

        Assert.That(mySaveGame.MyString, Is.EqualTo("Hello World"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects, Is.Null);
        
        mySaveGame = await loader.Load(SaveName1, null);
        Assert.That(mySaveGame.MyString, Is.EqualTo("Hello World"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects.Count, Is.EqualTo(0));
    }

    
    [TestRunner.Test(Description = "Error, corrupted metadata rebuild the metadata")]
    public async Task LoadCorruptedDataTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MySaveGame() {
            MyString = "yu"
        };
        await loader.Save(saveGame, new List<SaveObject>(), SaveName1, null);
        
        File.WriteAllLines(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".metadata"), new []{"corrupted data"});

        var mySaveGame = await loader.LoadMetadataFile(SaveName1);
        Assert.That(mySaveGame.MyString, Is.EqualTo("Hello World"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects, Is.Null);
        
        mySaveGame = await loader.Load(SaveName1, null);
        Assert.That(mySaveGame.MyString, Is.EqualTo("Hello World"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects.Count, Is.EqualTo(0));
    }

    
    public class MyJsonGameLoader : JsonGameLoader<MySaveGame> {
        public override string GetSavegameFolder() {
            return ".";
        }
    }

    public abstract class Comp : SaveObject {
        public abstract bool Equivalent(Comp other);

    }

    public class X : Comp {
        public string MyX { get; set; } = "Hello World";

        public X() {
        }

        public X(string myX) {
            MyX = myX;
        }

        public override string Discriminator() {
            return "X";
        }
        
        public override bool Equivalent(Comp other) {
            return other.GetHashCode() != GetHashCode() && other is X x && x.MyX == MyX;
        }
    }

    public class Y : Comp {
        public string MyY { get; set; } = "Hello World";

        public Y() {
        }

        public Y(string myY) {
            MyY = myY;
        }

        public override string Discriminator() {
            return "Y";
        }
        public override bool Equivalent(Comp other) {
            return other.GetHashCode() != GetHashCode() && other is Y x && x.MyY == MyY;
        }
    }

    [TestRunner.Test]
    public async Task PolymorphicSaveAndLoad() {
        var loader = new MyJsonGameLoader();
        var data = new List<SaveObject> {
            new X("a") {
                Id = 1,
                Name = "aa",
                Alias = "aaa",
            },
            new Y("b") {
                Id = 2,
                Name = "bb",
                Alias = "bbb",
            },
            new X("c") {
                Id = 3,
                Name = "cc",
            },
            new Y("d") {
                Id = 4,
                Name = "dd",
            },
        };
        var saveGame = new MySaveGame {
            MyString = "a"
        };
        await loader.Save(saveGame, data, SaveName1, null);
        var loadGame = await loader.Load(SaveName1, null);
        Assert.That(saveGame.MyString, Is.EqualTo("a"));

        Assert.That(loadGame.GameObjects.Count, Is.EqualTo(4));
        Assert.That(((Comp)data[0]).Equivalent((Comp)loadGame.GameObjects[0]));
        Assert.That(((Comp)data[1]).Equivalent((Comp)loadGame.GameObjects[1]));
        Assert.That(((Comp)data[2]).Equivalent((Comp)loadGame.GameObjects[2]));
        Assert.That(((Comp)data[3]).Equivalent((Comp)loadGame.GameObjects[3]));
    }

    public class WithData : SaveObject {
        public Rect2 Rect2Value { get; set; }
        public Vector2 Vector2Value { get; set; }
        public Vector3 Vector3Value { get; set; }
        public Rect2I Rect2IValue { get; set; }
        public Vector2I Vector2IValue { get; set; }
        public Vector3I Vector3IValue { get; set; }
        public Color ColorValue { get; set; }
        public override string Discriminator() {
            return "WD";
        }
    }

    [TestRunner.Test]
    public async Task ConverterTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MySaveGame();
        var wd = new WithData {
            Id = 1,
            Name = "aa",
            Alias = "aaa",
            Rect2Value = new Rect2(1.3f, 2.2f, 3.2f, 4.5f),
            Vector2Value = new Vector2(1.2f, 2.4f),
            Vector3Value = new Vector3(1.3f, 2.3f, 3.3f),
            Rect2IValue = new Rect2I(1, 2, 3, 4),
            Vector2IValue = new Vector2I(2, 3),
            Vector3IValue = new Vector3I(2, 8, 10),
            ColorValue = new Color(0.23f, 0.4f, 0.53f, 0.3f),
        };
        var data = new List<SaveObject> { wd };
        
        await loader.Save(saveGame, data, SaveName1, null);
        var loadGame = await loader.Load(SaveName1, null);
        Assert.That(loadGame.GameObjects.Count, Is.EqualTo(1));
        var ld = loadGame.GameObjects.OfType<WithData>().First();
        Assert.That(wd.Id, Is.EqualTo(ld.Id));
        Assert.That(wd.Name, Is.EqualTo(ld.Name));
        Assert.That(wd.Alias, Is.EqualTo(ld.Alias));
        Assert.That(wd.Rect2Value, Is.EqualTo(ld.Rect2Value));
        Assert.That(wd.Vector2Value, Is.EqualTo(ld.Vector2Value));
        Assert.That(wd.Vector3Value, Is.EqualTo(ld.Vector3Value));
        Assert.That(wd.Rect2IValue, Is.EqualTo(ld.Rect2IValue));
        Assert.That(wd.Vector2IValue, Is.EqualTo(ld.Vector2IValue));
        Assert.That(wd.Vector3IValue, Is.EqualTo(ld.Vector3IValue));
        Assert.That(wd.ColorValue.ToHtml(true), Is.EqualTo(ld.ColorValue.ToHtml(true)));
    }

    [TestRunner.Test]
    public async Task ListDataTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(new MySaveGame() {
            MyString = "yu1"
        }, new List<SaveObject> { new WithData() }, SaveName1, null);


        var list = await loader.ListSaveGames();
        
        Assert.That(list.Count, Is.EqualTo(1));
        var mySaveGame = list[0];
        Assert.That(mySaveGame.MyString, Is.EqualTo("yu1"));
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects, Is.Null);
    }

    [TestRunner.Test]
    public async Task ListRebuildTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(new MySaveGame() {
            MyString = "yu1"
        }, new List<SaveObject> { new WithData() }, SaveName1, null);

        new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".metadata")).Delete();

        var list = await loader.ListSaveGames();
        
        Assert.That(list.Count, Is.EqualTo(1));
        var mySaveGame = list[0];
        Assert.That(mySaveGame.MyString, Is.EqualTo("Hello World")); // because the metadata file was deleted and rebuilt
        Assert.That(mySaveGame.LoadStatus, Is.EqualTo(LoadStatus.Ok));
        Assert.That(mySaveGame.SavegameFileName.EndsWith(SaveName1+".data"));
        Assert.That(mySaveGame.MetadataFileName.EndsWith(SaveName1+".metadata"));
        Assert.That(mySaveGame.Name, Is.EqualTo(SaveName1));
        Assert.That(mySaveGame.Size, Is.EqualTo(new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Length));
        Assert.That(mySaveGame.GameObjects, Is.Null);
    }
}