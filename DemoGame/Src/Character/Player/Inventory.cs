using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Character.Items;

namespace Veronenger.Character.Player;

public class Inventory {
    public event Action<Item>? OnEquip;

    public readonly List<Item> Items = new();
    public int Selected = 0;
    public WeaponMeleeItem? WeaponEquipped = null;
    
    public void Pick(Item item) {
        Items.Add(item);
    }

    public Item GetCurrent() => Items[Selected];
    
    public void Equip() {
        var worldItem = GetCurrent();
        OnEquip?.Invoke(worldItem);
        if (worldItem is WeaponMeleeItem weaponItem) WeaponEquipped = weaponItem;
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