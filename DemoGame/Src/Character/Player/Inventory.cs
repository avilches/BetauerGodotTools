using System;
using System.Collections.Generic;
using Betauer.Core;
using Veronenger.Persistent;

namespace Veronenger.Character.Player;

public class Inventory {
    public event Action<PickableGameObject>? OnEquip;
    public event Action<PickableGameObject>? OnUnequip;
    public event Action<PickableGameObject>? OnSlotAmountUpdate;
    public event Action<PlayerInventoryEvent>? OnUpdateInventory;
    public event Action<PlayerInventorySlotEvent>? OnUpdateInventorySlot;

    public readonly List<PickableGameObject> Items = new();

    private readonly PlayerInventoryEvent _playerInventoryEventCached;

    public Inventory() {
        _playerInventoryEventCached = new PlayerInventoryEvent(this);
    }

    public int Selected = -1;

    public WeaponGameObject? WeaponEquipped { get; private set; }
    public WeaponRangeGameObject? WeaponRangeEquipped => WeaponEquipped as WeaponRangeGameObject;
    public WeaponMeleeGameObject? WeaponMeleeEquipped => WeaponEquipped as WeaponMeleeGameObject;

    public PickableGameObject? GetCurrent() {
        return Items.Count == 0 ? null : Items[Selected];
    }

    public void UpdateWeaponRangeAmmo(WeaponRangeGameObject gameObject, int amountChange) {
        gameObject.Ammo += amountChange;
        OnSlotAmountUpdate?.Invoke(gameObject);
    }

    public void Pick(PickableGameObject gameObject) {
        Items.Add(gameObject);
        TriggerPickUp(gameObject);
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

    private void _Unequip(PickableGameObject gameObject) {
        WeaponEquipped = null;
        OnUnequip?.Invoke(gameObject);
    }

    private void _Equip() {
        var item = GetCurrent();
        if (item is WeaponGameObject weapon) {
            WeaponEquipped = weapon;
            TriggerEquipItem(item);
        }
    }

    private void TriggerPickUp(PickableGameObject gameObject) =>
        OnUpdateInventorySlot?.Invoke(new PlayerInventorySlotEvent(this, PlayerInventoryEventType.PickUp, gameObject));

    private void TriggerDropItem(PickableGameObject drop) =>
        OnUpdateInventorySlot?.Invoke(new PlayerInventorySlotEvent(this, PlayerInventoryEventType.Drop, drop));

    private void TriggerEquipItem(PickableGameObject gameObject) {
        OnEquip?.Invoke(gameObject);
        OnUpdateInventorySlot?.Invoke(new PlayerInventorySlotEvent(this, PlayerInventoryEventType.Equip, gameObject));
    }

    public void TriggerRefresh() =>
        OnUpdateInventory?.Invoke(_playerInventoryEventCached);
}