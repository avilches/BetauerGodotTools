using System;
using System.Collections.Generic;

namespace Veronenger.Game.Dungeon.World;

public abstract record EnumConfig<TEnum, TConfig>(TEnum Type) where TEnum : struct, Enum
    where TConfig : EnumConfig<TEnum, TConfig> {

    private static readonly Dictionary<TEnum, TConfig> Configs = new();

    public static IEnumerable<TConfig> All => Configs.Values;

    // Método protegido para registrar configuraciones, solo accesible desde las clases derivadas
    public static void Register(TConfig config) {
        var type = config.Type;
        if (!Configs.TryAdd(type, config)) {
            throw new InvalidOperationException($"Config for {typeof(TEnum).Name}.{type} already registered");
        }
    }

    public static void RegisterAll(params TConfig[] configs) {
        Clear();
        foreach (var config in configs) {
            Register(config);
        }
        VerifyAll();
    }

    public static TConfig Get(TEnum type) {
        return Configs.TryGetValue(type, out var config)
            ? config
            : throw new InvalidOperationException($"No config found for {typeof(TEnum).Name}.{type}");
    }

    public static void VerifyAll() {
        foreach (var type in Enum.GetValues<TEnum>()) {
            if (!Configs.ContainsKey(type)) {
                throw new InvalidOperationException($"Missing config for {typeof(TEnum).Name}.{type}");
            }
        }
    }

    public static void Clear() => Configs.Clear();

    public void Deconstruct(out TEnum Type) {
        Type = this.Type;
    }
}