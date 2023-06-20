using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Betauer.Application.Persistent;
using Betauer.Core;
using Veronenger.Game.Platform.Items;

namespace Veronenger.Game.Platform.Character.Player;

public class InventoryGameObject : GameObject {
    public WeaponGameObject? WeaponEquipped { get; set; }
    public readonly List<PickableGameObject> Items = new();
    public int Selected { get; set; } = -1;

    public override void OnInitialize() {
    }
    
    public override void OnRemove() {
    }

    public override void OnLoad(SaveObject saveObject) {
        var save = (InventorySaveObject)saveObject;
        Selected = save.Selected;
        Items.Clear();
        Items.AddRange(save.Items.Select(id => GameObjectRepository.Get<PickableGameObject>(id)).ToList());
        WeaponEquipped = save.WeaponEquipped != -1 ? GameObjectRepository.Get<WeaponGameObject>(save.WeaponEquipped) : null;
        Selected = save.Selected;
    }

    public override SaveObject CreateSaveObject() => new InventorySaveObject(this);
}

public class InventorySaveObject : SaveObject<InventoryGameObject>, IPlatformSaveObject {
    [JsonInclude] public int Selected { get; set; }
    [JsonInclude] public List<int> Items { get; set; }
    [JsonInclude] public int WeaponEquipped { get; set; }

    public InventorySaveObject() {
    }

    public InventorySaveObject(InventoryGameObject gameObject) : base(gameObject) {
        Selected = gameObject.Selected;
        Items = gameObject.Items.Select(item => item.Id).ToList();
        WeaponEquipped = gameObject.WeaponEquipped?.Id ?? -1;
        Selected = gameObject.Selected;
    }

    public override string Discriminator() => "Player.Inventory";
}

public class Inventory {
    public InventoryGameObject InventoryGameObject { get; set; }
    private readonly PlayerInventoryEvent _playerInventoryEventCached;

    public event Action<PickableGameObject>? OnEquip;
    public event Action<PickableGameObject>? OnUnequip;
    public event Action<PickableGameObject>? OnSlotAmountUpdate;
    public event Action<PlayerInventoryEvent>? OnUpdateInventory;
    public event Action<PlayerInventorySlotEvent>? OnUpdateInventorySlot;

    public List<PickableGameObject> Items => InventoryGameObject.Items;

    public int Selected {
        get => InventoryGameObject.Selected;
        set => InventoryGameObject.Selected = value;
    }

    public WeaponGameObject? WeaponEquipped {
        get => InventoryGameObject.WeaponEquipped;
        set => InventoryGameObject.WeaponEquipped = value;
    }
    public WeaponRangeGameObject? WeaponRangeEquipped => WeaponEquipped as WeaponRangeGameObject;
    public WeaponMeleeGameObject? WeaponMeleeEquipped => WeaponEquipped as WeaponMeleeGameObject;

    public Inventory() {
        _playerInventoryEventCached = new PlayerInventoryEvent(this);
    }
    
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
            EquipCurrent();
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
            EquipCurrent();
        }
        TriggerRefresh();
    }

    public void EquipPrevItem() {
        if (Items.Count == 0) return;
        Selected = (Selected - 1).Mod(Items.Count);
        EquipCurrent();
        TriggerRefresh();
    }

    public void EquipNextItem() {
        if (Items.Count == 0) return;
        Selected = (Selected + 1).Mod(Items.Count);
        EquipCurrent();
        TriggerRefresh();
    }

    private void _Unequip(PickableGameObject gameObject) {
        WeaponEquipped = null;
        OnUnequip?.Invoke(gameObject);
    }

    public void EquipCurrent() {
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