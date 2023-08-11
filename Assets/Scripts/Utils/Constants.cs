using UnityEngine;

public class Constants
{
    public static readonly Vector3 TargetOffset = Vector3.up * 0.7f;
    public static readonly Vector3 LevelOrigin = new Vector3(0, 0, 50);
}

public enum InventoryTabs {
    Settings = 0,
    Craft = 1,
    Inventory = 2,
    Shop = 3
}

