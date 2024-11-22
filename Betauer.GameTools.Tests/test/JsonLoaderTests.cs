using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Betauer.Application.Persistent;
using Betauer.Application.Persistent.Json;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.GameTools.Tests;

[TestFixture]
public class JsonLoaderTests {
    public string SaveName1 = "a";
    public string SaveName2 = "b";

    [TearDown]
    [OneTimeSetUp]
    public void TearDown() {
        File.Delete($"{SaveName1}.metadata");
        File.Delete($"{SaveName1}.data");
        File.Delete($"{SaveName2}.metadata");
        File.Delete($"{SaveName2}.data");
    }


    public string key = new Guid().ToString();

    [Test]
    public async Task ListNoDataTest() {
        var loader = new MyJsonGameLoader();
        var list = await loader.ListMetadatas();
        Assert.That(list, Is.Empty);
    }

    public class MyMetadata : Metadata {
        [JsonInclude] public string MyString { get; set; } = "Hello World";
    }

    [Test(Description = "Error, no metadata")]
    public async Task LoadMissingMetadataAndDataTest() {
        await ThrowsAsync<FileNotFoundException>(async () => await new MyJsonGameLoader().LoadMetadata(SaveName1));
    }

    [Test(Description = "Error, no data")]
    public async Task LoadMissingDataTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>());
        new FileInfo(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".data")).Delete();
        await ThrowsAsync<FileNotFoundException>(async () => await loader.LoadMetadata(SaveName1));
    }

    [Test(Description = "Reading metadata: key")]
    public async Task LoadMetadataKeyTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata() {
            MyString = "XXX"
        }, new List<SaveObject>(), null, key);
        var savegame = await loader.LoadMetadata(SaveName1, key);
        Assert.That(savegame.MyString, Is.EqualTo("XXX"));
        Assert.That(savegame.Name, Is.EqualTo(SaveName1));
    }

    [Test(Description = "Reading metadata: no key")]
    public async Task LoadMetadataNoTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata() {
            MyString = "ZZZ"
        }, new List<SaveObject>(), null);
        var savegame = await loader.LoadMetadata(SaveName1);
        Assert.That(savegame.MyString, Is.EqualTo("ZZZ"));
        Assert.That(savegame.Name, Is.EqualTo(SaveName1));
    }

    [Test(Description = "Error reading metadata, missing key")]
    public async Task LoadMissingKeyTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, key);
        await ThrowsAsync<JsonException>(async () => await loader.LoadMetadata(SaveName1));
    }

    private async Task ThrowsAsync<T>(Func<Task> func) where T : Exception {
        try {
            await func();
        } catch (Exception e) {
            Assert.That(e, Is.TypeOf<T>());
        }
    }

    [Test(Description = "Error reading metadata, wrong key")]
    public async Task LoadWrongKeyTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, key);
        await ThrowsAsync<CryptographicException>(async () => await loader.LoadMetadata(SaveName1, key + key));
    }

    [Test(Description = "Error reading plain metadata with key")]
    public async Task LoadNoKeyWithKeyTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>());
        await ThrowsAsync<CryptographicException>(async () => await loader.LoadMetadata(SaveName1, key));
    }


    [Test(Description = "Error reading metadata, corrupted")]
    public async Task LoadCorruptedDataTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, key);
        File.WriteAllLines(Path.Combine(loader.GetSavegameFolder(), SaveName1 + ".metadata"), new[] { "corrupted data" });
        await ThrowsAsync<CryptographicException>(async () => await loader.LoadMetadata(SaveName1, key));
    }

    public class MyJsonGameLoader : JsonGameLoader<MyMetadata> {
        public MyJsonGameLoader() {
            Scan<ISaveObject>();
        }

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
            return "X.1";
        }

        public override bool Equivalent(Comp other) {
            return other.GetHashCode() != GetHashCode() && other is X x && x.MyX == MyX;
        }
        // public override int Hash() => System.HashCode.Combine(MyX);
    }

    public class Y : Comp {
        public string MyY { get; set; } = "Hello World";

        public Y() {
        }

        public Y(string myY) {
            MyY = myY;
        }

        public override string Discriminator() {
            return "Y.2";
        }

        public override bool Equivalent(Comp other) {
            return other.GetHashCode() != GetHashCode() && other is Y x && x.MyY == MyY;
        }
        // public override int Hash() => System.HashCode.Combine(MyY);
    }

    [Test(Description = "Read and write twice to ensure the file is not locked")]
    public async Task BasicSaveLoadTest() {
        var loader = new MyJsonGameLoader();
        var data = new List<SaveObject> { new WithData() };
        for (int i = 0; i < 10; i++) {
            data.AddRange(data.ToArray());
        }
        await loader.Save(SaveName1, new MyMetadata(), data, null, null, false);
        await loader.Load(SaveName1, null, null, false);
        await loader.Save(SaveName1, new MyMetadata(), data, null, null, false);
        await loader.Load(SaveName1, null, null, false);
        var plainTextSize = new FileInfo(SaveName1 + ".data").Length;
        Console.WriteLine("Plain text size: " + plainTextSize + " bytes");

        await loader.Save(SaveName1, new MyMetadata(), data, null, null, true);
        await loader.Load(SaveName1, null, null, true);
        await loader.Save(SaveName1, new MyMetadata(), data, null, null, true);
        await loader.Load(SaveName1, null, null, true);
        var compressedPlainSize = new FileInfo(SaveName1 + ".data").Length;
        Console.WriteLine("Compressed plain text size: " + compressedPlainSize + " bytes");

        await loader.Save(SaveName1, new MyMetadata(), data, null, "a", false);
        await loader.Load(SaveName1, null, "a", false);
        await loader.Save(SaveName1, new MyMetadata(), data, null, "a", false);
        await loader.Load(SaveName1, null, "a", false);
        var cypherSize = new FileInfo(SaveName1 + ".data").Length;
        Console.WriteLine("Cypher size: " + cypherSize + " bytes"); 

        await loader.Save(SaveName1, new MyMetadata(), data, null, "a", true);
        await loader.Load(SaveName1, null, "a", true);
        await loader.Save(SaveName1, new MyMetadata(), data, null, "a", true);
        await loader.Load(SaveName1, null, "a", true);
        var compressedCypherSize = new FileInfo(SaveName1 + ".data").Length;
        Console.WriteLine("Compressed cypher size: " + compressedCypherSize + " bytes");

        Assert.That(compressedPlainSize, Is.LessThan(plainTextSize / 100));
        Assert.That(cypherSize, Is.InRange(plainTextSize, plainTextSize + 80)); // 80 bytes for the compression header
        Assert.That(compressedCypherSize, Is.InRange(compressedPlainSize, compressedPlainSize + 80)); // 80 bytes for the compression header
    }

    [Test]
    public async Task NoCompressTests() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, null, false);
        Assert.That(await File.ReadAllTextAsync(SaveName1 + ".data"), Is.EqualTo("[]"));
    }

    [Test]
    public async Task CypherTests() {
        var loader = new MyJsonGameLoader();
        var seed = "hola";
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, seed, false);

        using var transform = GameObjectLoader.CreateDecryptor(seed);
        using var stringReader = new StreamReader(new CryptoStream(File.OpenRead(SaveName1 + ".data"), transform, CryptoStreamMode.Read));

        Assert.That(await stringReader.ReadToEndAsync(), Is.EqualTo("[]"));
    }

    [Test]
    public async Task CypherCompressedTests() {
        var loader = new MyJsonGameLoader();
        var seed = "hola";
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, seed, true);

        using var transform = GameObjectLoader.CreateDecryptor(seed);
        using var stringReader =
            new StreamReader(GameObjectLoader.Decompress(new CryptoStream(File.OpenRead(SaveName1 + ".data"), transform, CryptoStreamMode.Read), true));

        Assert.That(await stringReader.ReadToEndAsync(), Is.EqualTo("[]"));
    }

    [Test]
    public async Task CompressedTests() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata(), new List<SaveObject>(), null, null, true);
        using var stringReader = new StreamReader(new GZipStream(File.OpenRead(SaveName1 + ".data"), CompressionMode.Decompress));
        Assert.That(await stringReader.ReadToEndAsync(), Is.EqualTo("[]"));
    }

    [Test]
    public async Task LoadSaveGameWithMetadataTests() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MyMetadata {
            MyString = "a"
        };
        await loader.Save(SaveName1, saveGame, new List<SaveObject>(), null, key);
        var loadGame = await loader.Load(SaveName1, null, key);
        Assert.That(loadGame.Metadata.MyString, Is.EqualTo("a"));
        Assert.That(loadGame.Metadata.Name, Is.EqualTo(SaveName1));
        Assert.That(loadGame.Metadata.CreateDate, Is.Not.Null);
        Assert.That(loadGame.Metadata.UpdateDate, Is.Not.Null);
        Assert.That(loadGame.Metadata.ReadDate, Is.Not.Null);
        Assert.That(loadGame.GameObjects.Count, Is.EqualTo(0));
    }

    [Test]
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
        var saveGame = new MyMetadata {
            MyString = "a"
        };
        await loader.Save(SaveName1, saveGame, data, null, key);
        var loadGame = await loader.Load(SaveName1, null, key);
        Assert.That(loadGame.Metadata.MyString, Is.EqualTo("a"));
        Assert.That(loadGame.Metadata.Name, Is.EqualTo(SaveName1));
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
        // public override int Hash() => System.HashCode.Combine(Rect2Value, Vector2Value, Vector3Value, Rect2IValue, Vector2IValue, Vector3IValue, ColorValue);
    }

    [Test]
    public async Task ConverterTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MyMetadata();
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

        await loader.Save(SaveName1, saveGame, data, null, key);
        var loadGame = await loader.Load(SaveName1, null, key);
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

    [Test(Description = "List save games with key")]
    public async Task ListSeedOkTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata() {
            MyString = "yu1"
        }, new List<SaveObject> { new WithData() }, null, key);

        await loader.Save(SaveName2, new MyMetadata() {
            MyString = "yu2"
        }, new List<SaveObject> { new WithData() }, null, key);

        // Ignore cases
        Assert.That((await loader.ListMetadatas()).Count, Is.EqualTo(0)); // no key
        Assert.That((await loader.ListMetadatas("wrong")).Count, Is.EqualTo(0));
        Assert.That((await loader.GetMetadatas(null, "x", "y", SaveName1, SaveName2)).Count, Is.EqualTo(0));
        Assert.That((await loader.GetMetadatas("wrong", "x", "y", SaveName1, SaveName2)).Count, Is.EqualTo(0));

        Assert.That((await loader.GetMetadatas(key, "x", "y")).Count, Is.EqualTo(0));
        Assert.That((await loader.GetMetadatas(key, "x", "y", SaveName1, SaveName2)).Count, Is.EqualTo(2));

        var list = await loader.ListMetadatas(key);
        Assert.That(list.Count, Is.EqualTo(2));

        var mySaveGame1 = list.First(sg => sg.Name == SaveName1);
        Assert.That(mySaveGame1.MyString, Is.EqualTo("yu1"));

        var mySaveGame2 = list.First(sg => sg.Name == SaveName2);
        Assert.That(mySaveGame2.MyString, Is.EqualTo("yu2"));

        var get = await loader.GetMetadatas(key, SaveName1);
        Assert.That(get[0].MyString, Is.EqualTo("yu1"));
    }

    [Test(Description = "List save games with no key")]
    public async Task ListNoSeedOkTest() {
        var loader = new MyJsonGameLoader();
        await loader.Save(SaveName1, new MyMetadata() {
            MyString = "yu1"
        }, new List<SaveObject> { new WithData() }, null);

        await loader.Save(SaveName2, new MyMetadata() {
            MyString = "yu2"
        }, new List<SaveObject> { new WithData() }, null);

        // Ignore cases
        Assert.That((await loader.ListMetadatas("aaaa")).Count, Is.EqualTo(0));
        Assert.That((await loader.GetMetadatas("aaaa", SaveName1, SaveName2)).Count, Is.EqualTo(0));

        var list = await loader.ListMetadatas();
        Assert.That(list.Count, Is.EqualTo(2));

        var mySaveGame1 = list.First(sg => sg.Name == SaveName1);
        Assert.That(mySaveGame1.MyString, Is.EqualTo("yu1"));

        var mySaveGame2 = list.First(sg => sg.Name == SaveName2);
        Assert.That(mySaveGame2.MyString, Is.EqualTo("yu2"));

        var get = await loader.GetMetadatas(null, SaveName1);
        Assert.That(get[0].MyString, Is.EqualTo("yu1"));
    }

    [Test]
    public async Task SaveProgressTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MyMetadata();
        var wd = new WithData();
        var data = new List<SaveObject>() { wd, wd, wd, wd, wd, wd, wd, wd, wd, wd };

        var progress = new List<float>(11);
        await loader.Save(SaveName1, saveGame, data, (p) => {
            Console.WriteLine(p + "%");
            progress.Add(p);
        }, key);
        Assert.That(progress[0], Is.EqualTo(0f));
        Assert.That(progress[1], Is.EqualTo(0.1f));
        Assert.That(progress[2], Is.EqualTo(0.2f));
        Assert.That(progress[3], Is.EqualTo(0.3f));
        Assert.That(progress[4], Is.EqualTo(0.4f));
        Assert.That(progress[5], Is.EqualTo(0.5f));
        Assert.That(progress[6], Is.EqualTo(0.6f));
        Assert.That(progress[7], Is.EqualTo(0.7f));
        Assert.That(progress[8], Is.EqualTo(0.8f));
        Assert.That(progress[9], Is.EqualTo(0.9f));
        Assert.That(progress[10], Is.EqualTo(1f));
    }

    [Test]
    public async Task LoadProgressTest() {
        var loader = new MyJsonGameLoader();
        var saveGame = new MyMetadata();
        var wd = new WithData();
        var data = new List<SaveObject>();
        for (var i = 0; i < 25000; i++) {
            data.Add(wd);
        }
        var progress = new List<float>(11);
        await loader.Save(SaveName1, saveGame, data, null, key);
        await loader.Load(SaveName1, (p) => {
            progress.Add(p);
            Console.WriteLine(p + "%");
        }, key);

        Assert.That(progress.First(), Is.EqualTo(0f));
        Assert.That(progress.Last(), Is.EqualTo(1f));
        for (var i = 1; i < progress.Count; i++) {
            Assert.That(progress[i], Is.GreaterThanOrEqualTo(progress[i - 1]));
        }
        Assert.That(CalculateStandardDeviation(progress.ToArray()), Is.LessThan(0.1));
    }

    public double CalculateStandardDeviation(float[] array) {
        // Calcula las diferencias entre los elementos consecutivos
        var differences = new List<double>();
        for (int i = 1; i < array.Length; i++) {
            differences.Add(array[i] - array[i - 1]);
        }
        // Calcula la media de las diferencias
        double mean = differences.Average();
        // Calcula la varianza de las diferencias
        double sumOfSquaredDifferences = differences.Select(val => (val - mean) * (val - mean)).Sum();
        double variance = sumOfSquaredDifferences / differences.Count;
        // La desviación estándar es la raíz cuadrada de la varianza
        double standardDeviation = Math.Sqrt(variance);
        Console.WriteLine($"Standard deviation: {standardDeviation:0.00000}%");
        return standardDeviation;
    }
}