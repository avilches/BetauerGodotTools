using System;
using System.Collections.Generic;
using Betauer.Core;

namespace Veronenger.Character.Items;

public class Inventory {
    public event Action<WorldItem>? OnEquip;

    public readonly List<WorldItem> Items = new();
    public int Selected = 0;

    public void Pick(WorldItem worldItem) {
        Items.Add(worldItem);
    }

    public WorldItem GetCurrent() => Items[Selected];

    public void Equip() {
        OnEquip?.Invoke(GetCurrent());
    }

    public void Equip(int pos) {
        Selected = pos;
        Equip();
    }

    public void RemoveElement(WorldItem worldItem) {
        Items.Remove(worldItem);
    }

    public void NextItem() => Selected = (Selected + 1).Mod(Items.Count);
    public void PrevItem() => Selected = (Selected - 1).Mod(Items.Count);
}