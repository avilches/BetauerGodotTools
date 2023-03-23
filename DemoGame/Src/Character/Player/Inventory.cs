using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class Inventory {
    public event Action<Item>? OnEquip;

    public readonly List<Item> Items = new();
    public int Selected = 0;
    public WeaponMeleeItem WeaponMeleeEquipped { get; private set; }
    public WeaponRangeItem WeaponRangeEquipped { get; private set; }
    
    public void Pick(Item item) {
        Items.Add(item);
    }

    public void PickAndEquip(Item item) {
        Items.Add(item);
        Equip(Items.Count - 1);
    }

    public Item GetCurrent() => Items[Selected];
    
    public void Equip() {
        var worldItem = GetCurrent();
        OnEquip?.Invoke(worldItem);
        if (worldItem is WeaponMeleeItem melee) WeaponMeleeEquipped = melee;
        if (worldItem is WeaponRangeItem range) WeaponRangeEquipped = range;
    }

    public void Equip(int pos) {
        Selected = pos;
        Equip();
    }

    public void RemoveElement(Item item) {
        Items.Remove(item);
    }

    public void NextItem() => Selected = (Selected + 1).Mod(Items.Count);
    public void PrevItem() => Selected = (Selected - 1).Mod(Items.Count);
}