using UnityEngine;

// 物品基类
public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Count { get; set; } = 1;
    public bool CanStack { get; set; }
    public int MaxStack { get; set; } = 99;
}