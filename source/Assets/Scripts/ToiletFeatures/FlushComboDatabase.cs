using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComboDefinition
{
    public string id;
    public string displayName;
    public FlushItemType itemA;
    public FlushItemType itemB;
    public int bonusCoins;
    public string firstTimeMessage;
    public string repeatMessage;
}

public class FlushComboDatabase : MonoBehaviour
{
    public List<ComboDefinition> combos;
    Dictionary<(FlushItemType, FlushItemType), ComboDefinition> lookup;

    void Awake()
    {
        lookup = new();
        foreach (var c in combos)
        {
            if (c.itemA == c.itemB) continue;
            var k = MakeKey(c.itemA, c.itemB);
            if (!lookup.ContainsKey(k)) lookup.Add(k, c);
        }
    }

    (FlushItemType, FlushItemType) MakeKey(FlushItemType a, FlushItemType b)
        => (a <= b) ? (a, b) : (b, a);

    public ComboDefinition GetCombo(FlushItemType a, FlushItemType b)
    {
        if (a == b) return null; // enforce rule again
        lookup.TryGetValue(MakeKey(a, b), out var combo);
        return combo;
    }
}
