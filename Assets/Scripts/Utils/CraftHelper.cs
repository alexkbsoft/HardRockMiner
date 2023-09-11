using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


public class CraftHelper
{
    public List<string> Recipie;
    public string[] InSlots => _inSlots;
    public string ItemName;

    private string[] _inSlots = new string[16];

    public CraftHelper(DragSlot[] craftSlots, List<string> recipie, string itemId)
    {
        Recipie = recipie;
        ItemName = itemId;

        for (int i = 0; i < craftSlots.Length; i++)
        {
            var draggable = craftSlots[i].LinkedDraggable;
            var linkedItem = draggable == null ? null : draggable.GetComponent<InventoryItem>();

            _inSlots[i] = linkedItem == null ? null : linkedItem.UniqName;
        }
    }

    public bool IsMatch()
    {
        var result = true;

        for (int i = 0; i < Recipie.Count; i++)
        {
            var inSlot = _inSlots[i];
            var inRecipie = Recipie[i];

            if (string.IsNullOrEmpty(inSlot) && string.IsNullOrEmpty(inRecipie))
            {
                continue;
            }

            if (inSlot == inRecipie)
            {
                continue;
            }

            result = false;

            break;
        }

        return result;
    }

    public string TryMergeSchemas()
    {
        string resultName = null;
        string currentFoundSchema = null;
        int percentSum = 0;
        int founCount = 0;
        Recipie = new();
        Regex filter = new Regex(@"(.*)_schema(.*)");

        foreach (string oneName in _inSlots)
        {
            if (!string.IsNullOrEmpty(oneName))
            {
                var matches = filter.Match(oneName).Groups.OfType<Group>().Skip(1);

                if (matches.Count() == 2) {
                    Recipie.Add(oneName);
                    var arrayOfGroups = matches.ToArray();
                    var itemId = arrayOfGroups[0].Value;
                    var percent = arrayOfGroups[1].Value;

                    founCount++;

                    if (currentFoundSchema == null) {
                        currentFoundSchema = itemId;
                        percentSum = int.Parse(percent);
                    } else if (itemId != currentFoundSchema) {
                        return null;
                    } else {
                        percentSum += int.Parse(percent);
                    }
                } else {
                    return null;
                }
            }
        }

        if (founCount <= 1) {
            return null;
        }

        percentSum = percentSum > 100 ? 100 : percentSum;
        resultName = $"{currentFoundSchema}_schema{percentSum}";

        ItemName = resultName;

        return resultName;
    }


}