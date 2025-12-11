using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Coins { get; private set; }

    public HUDController hud;

    private HashSet<string> discoveredCombos = new HashSet<string>();

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        hud?.SetMoney(Coins);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        if (Coins < 0) Coins = 0;
        hud?.SetMoney(Coins);
    }
    
    public void SetCoins(int amount)
    {
        Coins = Mathf.Max(0, amount);
        hud?.SetMoney(Coins);
    }

    public bool RegisterCombo(ComboDefinition combo)
    {
        if (combo == null) return false;
        if (discoveredCombos.Add(combo.id))
        {
            return true;
        }
        return false;
    }
    
    public List<string> GetUnlockedComboIds()
    {
        return new List<string>(discoveredCombos);
    }
    
    public void SetUnlockedCombos(List<string> ids)
    {
        discoveredCombos = new HashSet<string>(ids);
    }
}