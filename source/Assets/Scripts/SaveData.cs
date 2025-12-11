using System.Collections.Generic;

[System.Serializable]
public class SavedItem
{
    public string id;   // itemType id
    public int count;   // how many the player owns
}

[System.Serializable]
public class SaveData
{
    public int coins;
    public List<string> unlockedCombos = new List<string>();
    public List<SavedItem> inventory = new List<SavedItem>();
    public bool tutorialCompleted;
} 