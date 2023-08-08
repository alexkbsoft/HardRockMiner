using System;
using System.Collections;
using System.Collections.Generic;

public class CraftHelper {
    public List<string> Recipie => _recipie;
    public string[] InSlots => _inSlots;
    public string ItemName => _itemName;

    private string[] _inSlots = new string[16];
    private List<string> _recipie;
    private string _itemName;
    public CraftHelper(List<string> recipie, DragSlot[] craftSlots, string itemId) {
        _recipie = recipie;
        _itemName = itemId;

        for(int i = 0; i < craftSlots.Length; i++) {
            var draggable = craftSlots[i].LinkedDraggable;
            var linkedItem = draggable == null ? null : draggable.GetComponent<InventoryItem>();

            _inSlots[i] = linkedItem == null ? null : linkedItem.UniqName;
        }
    }

    public bool IsMatch() {
        var result = true;

        for (int i = 0; i < _recipie.Count; i++)
        {
            var inSlot = _inSlots[i];
            var inRecipie = _recipie[i];

            if (string.IsNullOrEmpty(inSlot) && string.IsNullOrEmpty(inRecipie)) {
                continue;
            }

            if (inSlot == inRecipie) {
                continue;
            }

            result = false;

            break;
        }

        return result;
    }


}