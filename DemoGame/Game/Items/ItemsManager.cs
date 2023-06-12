using Betauer.DI.Attributes;
using Veronenger.Game.Items.Config;

namespace Veronenger.Game.Items;

[Singleton]
public class ItemsManager  {
    [Inject("KnifeMelee")] public KnifeMelee Knife { get; private set; }
    [Inject("MetalbarMelee")] public MetalbarMelee Metalbar { get; private set; }
    [Inject("SlowGun")] public RangeSlowGun SlowGun { get; private set; }
    [Inject("Gun")] public RangeGun Gun { get; private set; }
    [Inject("Shotgun")] public RangeShotgun Shotgun { get; private set; }
    [Inject("MachineGun")] public RangeMachineGun MachineGun { get; private set; }
}

