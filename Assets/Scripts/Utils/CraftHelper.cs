using System;
using System.Collections;
using System.Collections.Generic;

public class CraftHelper {
    public List<string> Recipie;
    public string[] InSlots => _inSlots;
    public string ItemName;

    private string[] _inSlots = new string[16];
    
    public CraftHelper(DragSlot[] craftSlots, List<string> recipie, string itemId) {
        Recipie = recipie;
        ItemName = itemId;

        for(int i = 0; i < craftSlots.Length; i++) {
            var draggable = craftSlots[i].LinkedDraggable;
            var linkedItem = draggable == null ? null : draggable.GetComponent<InventoryItem>();

            _inSlots[i] = linkedItem == null ? null : linkedItem.UniqName;
        }
    }

    public bool IsMatch() {
        var result = true;

        for (int i = 0; i < Recipie.Count; i++)
        {
            var inSlot = _inSlots[i];
            var inRecipie = Recipie[i];

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