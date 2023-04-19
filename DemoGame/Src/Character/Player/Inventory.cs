using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class Inventory {
    public event Action<PickableItem>? OnEquip;
    public event Action<PickableItem>? OnUnequip;
    public event Action<PlayerInventoryEvent>? OnUpdateInventory;
    public event Action<PlayerInventorySlotEvent>? OnUpdateInventorySlot;

    public readonly List<PickableItem> Items = new();

    private readonly PlayerInventoryEvent _playerInventoryEventCached;

    public Inventory() {
        _playerInventoryEventCached = new PlayerInventoryEvent(this);
    }

    public int Selected = -1;

    public WeaponItem? WeaponEquipped { get; private set; }
    public WeaponRangeItem? WeaponRangeEquipped => WeaponEquipped as WeaponRangeItem;
    public WeaponMeleeItem? WeaponMeleeEquipped => WeaponEquipped as WeaponMeleeItem;

    public PickableItem? GetCurrent() {
        return Items.Count == 0 ? null : Items[Selected];
    }

    public void Pick(PickableItem item) {
        Items.Add(item);
        TriggerPickUp(item);
        if (Items.Count == 1) {
            Selected = 0;
            _Equip();
        }
        TriggerRefresh();
    }

    public void Drop() {
        if (Items.Count == 0) return;
        var item = GetCurrent()!; 
        Items.RemoveAt(Selected);
        if (Selected == Items.Count) Selected--;
        TriggerDropItem(item);
        if (Items.Count == 0) {
            _Unequip(item);
        } else {
            _Equip();
        }
        TriggerRefresh();
    }

    public void EquipPrevItem() {
        if (Items.Count == 0) return;
        Selected = (Selected - 1).Mod(Items.Count);
        _Equip();
        TriggerRefresh();
    }

    public void EquipNextItem() {
        if (Items.Count == 0) return;
        Selected = (Selected + 1).Mod(Items.Count);
        _Equip();
        TriggerRefresh();
    }

    private void _Unequip(PickableItem item) {
        WeaponEquipped = null;
        OnUnequip?.Invoke(item);
    }

    private void _Equip() {
        var item = GetCurrent();
        if (item is WeaponItem weapon) {
            WeaponEquipped = weapon;
            TriggerEquipItem(item);
        }
    }

    private void TriggerPickUp(PickableItem item) =>
        OnUpdateInventorySlot?.Invoke(new PlayerInventorySlotEvent(this, PlayerInventoryEventType.PickUp, item));

    private void TriggerDropItem(PickableItem drop) =>
        OnUpdateInventorySlot?.Invoke(new PlayerInventorySlotEvent(this, PlayerInventoryEventType.Drop, drop));

    private void TriggerEquipItem(PickableItem item) {
        OnEquip?.Invoke(item);
        OnUpdateInventorySlot?.Invoke(new PlayerInventorySlotEvent(this, PlayerInventoryEventType.Equip, item));
    }

    public void TriggerRefresh() =>
        OnUpdateInventory?.Invoke(_playerInventoryEventCached);
}