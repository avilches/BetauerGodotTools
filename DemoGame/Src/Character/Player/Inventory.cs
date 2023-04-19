using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class Inventory {
    public event Action<PickableItem>? OnEquip;
    public event Action? OnUpdateInventory;

    public readonly List<PickableItem> Items = new();
    public int Selected { get; private set; } = -1;
    public WeaponItem? WeaponEquipped { get; private set; }
    public WeaponRangeItem? WeaponRangeEquipped => WeaponEquipped as WeaponRangeItem;
    public WeaponMeleeItem? WeaponMeleeEquipped => WeaponEquipped as WeaponMeleeItem;

    public PickableItem? GetCurrent() {
        return Items.Count == 0 ? null : Items[Selected];
    }

    public void Pick(PickableItem item) {
        Items.Add(item);
        if (Items.Count == 1) Selected = 0;
        OnUpdateInventory?.Invoke();
    }

    public void Drop(PickableItem item) {
        Items.Remove(item);
        if (Selected == Items.Count) Selected--;
        OnUpdateInventory?.Invoke();
    }

    public void PrevItem() {
        if (Items.Count == 0) return;
        Selected = (Selected - 1).Mod(Items.Count);
        OnUpdateInventory?.Invoke();
    }

    public void NextItem() {
        if (Items.Count == 0) return;
        Selected = (Selected + 1).Mod(Items.Count);
        OnUpdateInventory?.Invoke();
    }

    public void EquipSelected() {
        var worldItem = GetCurrent();
        if (worldItem == null) return;
        if (worldItem is WeaponItem weapon) {
            WeaponEquipped = weapon;
        }
        OnEquip?.Invoke(worldItem);
        OnUpdateInventory?.Invoke();
    }

    public void DropSelected() {
        if (Items.Count == 0) return;
        Items.RemoveAt(Selected);
        if (Selected == Items.Count) Selected--;
        OnUpdateInventory?.Invoke();
    }
}