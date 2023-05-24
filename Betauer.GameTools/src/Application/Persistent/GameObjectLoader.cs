using System;
using System.Security.Cryptography;
using Betauer.Tools.Logging;

namespace Betauer.Application.Persistent;

public abstract class GameObjectLoader {
    public static readonly Logger Logger = LoggerFactory.GetLogger<GameObjectLoader>();
    
    public static (byte[] Key, byte[] IV) GenerateKeyAndIV(string seed) {
        using var aes = Aes.Create();
        var rng = new Random(seed.GetHashCode());
        var key = new byte[aes.KeySize / 8];
        var iv = new byte[aes.BlockSize / 8];
        rng.NextBytes(key);
        rng.NextBytes(iv);
        return (key, iv);
    }


}
