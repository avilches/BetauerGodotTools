using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Betauer.Tools.Logging;

namespace Betauer.Application.Persistent;

public abstract class GameObjectLoader {
    public static readonly Logger Logger = LoggerFactory.GetLogger<GameObjectLoader>();
    
    public static ICryptoTransform CreateDecryptor(string seed) {
        using var aes = Aes.Create();
        var (key, iv) = GenerateKeyAndIV(aes, seed);
        return aes.CreateDecryptor(key, iv);
    }
        
    public static ICryptoTransform CreateEncryptor(string seed) {
        using var aes = Aes.Create();
        var (key, iv) = GenerateKeyAndIV(aes, seed);
        return aes.CreateEncryptor(key, iv);
    }

    public static (byte[] Key, byte[] IV) GenerateKeyAndIV(SymmetricAlgorithm algorithm, string seed) {
        var rng = new Random(seed.GetHashCode());
        var key = new byte[algorithm.KeySize / 8];
        var iv = new byte[algorithm.BlockSize / 8];
        rng.NextBytes(key);
        rng.NextBytes(iv);
        return (key, iv);
    }

    public static Stream Compress(Stream stream, bool compress) {
        return compress ? new GZipStream(stream, CompressionMode.Compress) : stream;
    }

    public static Stream Decompress(Stream stream, bool decompress) {
        return decompress ? new GZipStream(stream, CompressionMode.Decompress) : stream;
    }


}
