using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class Inventory {
    public event Action<Item>? OnEquip;

    public readonly List<Item> Items = new();
    public int Selected = 0;
    public WeaponMeleeItem? WeaponMeleeEquipped { get; private set; }
    public WeaponRangeItem? WeaponRangeEquipped { get; private set; }
    
    public void Pick(Item item) {
        Items.Add(item);
        Log();
    }

    public void PickAndEquip(Item item) {
        Items.Add(item);
        Equip(Items.Count - 1);
    }

    public Item? GetCurrent() {
        return Items.Count == 0 ? null : Items[Selected];
    }

    public void Equip() {
        var worldItem = GetCurrent();
        if (worldItem == null) return;
        OnEquip?.Invoke(worldItem);
        if (worldItem is WeaponMeleeItem melee) {
            WeaponRangeEquipped = null;
            WeaponMeleeEquipped = melee;
        }
        if (worldItem is WeaponRangeItem range) {
            WeaponRangeEquipped = range;
            WeaponMeleeEquipped = null;
        }
        Log();
    }

    public void Equip(int pos) {
        Selected = pos;
        Equip();
    }

    public void RemoveElement(Item item) {
        Items.Remove(item);
        Log();
    }

    // print the items, showing which on is equipped comparing it withe WeaponMeleeEquipped
    public void Log() {
        for (var i = 0; i < Items.Count; i++) {
            var item = Items[i];
            var itemName = i == Selected ? $"[{item.Name}]" : $" {item.Name}";
            Console.WriteLine(itemName+(item == WeaponMeleeEquipped || item == WeaponRangeEquipped ? " E" : ""));
        }
    }


    public void NextItem() {
        if (Items.Count == 0) return;
        Selected = (Selected + 1).Mod(Items.Count);
    }

    public void PrevItem() {
        if (Items.Count == 0) return;
        Selected = (Selected - 1).Mod(Items.Count);
    }
}