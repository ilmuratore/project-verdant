using System;
using UnityEngine;

public enum StatType
{
    Attacco,
    Difesa,
    Vita
}

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsData data;

    [SerializeField] private int level = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int puntiDisponibili = 0;
    [SerializeField] private int puntiAttacco = 0;
    [SerializeField] private int puntiDifesa = 0;
    [SerializeField] private int puntiVita = 0;

    public event Action OnStatsChanged;

    public int Level => level;
    public int CurrentXp => currentXp;
    public int PuntiDisponibili => puntiDisponibili;
    public int PuntiAttacco => puntiAttacco;
    public int PuntiDifesa => puntiDifesa;
    public int PuntiVita => puntiVita;

    public int AttaccoEffettivo => GetData().attaccoBase + puntiAttacco * GetData().incrementoAttaco;
    public int DifesaEffettivo => GetData().difesaBase + puntiDifesa * GetData().incrementoDifesa;
    public int vitaMassimaEffettiva => GetData().vitaBase + puntiVita * GetData().incrementoVita;
    public int XpNecessari => Mathf.Max(1, Mathf.RoundToInt(GetData().xpBaseLevelUp * Mathf.Pow(GetData().xpCrescita, level - 1)));

    private void Start()
    {
        NotifyChanged();
    }

    public void AddXp(int amount)
    {
        if (amount <= 0) return;

        currentXp += amount;

        while (currentXp >= XpNecessari)
        {
            currentXp -= XpNecessari;
            LevelUp();
        }

        NotifyChanged();
    }

    public bool SpendiPunto(StatType tipo)
    {
        if (puntiDisponibili <= 0) return false;

        switch (tipo)
        {
            case StatType.Attacco:
                puntiAttacco++;
                break;
            case StatType.Difesa:
                puntiDifesa++;
                break;
            case StatType.Vita:
                puntiVita++;
                PlayerHealth health = GetComponent<PlayerHealth>();
                if (health != null) health.IncreaseMaxHealth(GetData().incrementoVita);
                break;
        }

        puntiDisponibili--;
        NotifyChanged();
        return true;
    }

    public int ApplicaDifesa(int dannoInArrivo)
    {
        return Mathf.Max(1, dannoInArrivo - DifesaEffettivo);
    }

    private void LevelUp()
    {
        level++;
        puntiDisponibili += GetData().puntiPerLivello;
    }

    private PlayerStatsData GetData()
    {
        if (data != null) return data;

        data = ScriptableObject.CreateInstance<PlayerStatsData>();
        return data;
    }

    private void NotifyChanged()
    {
        OnStatsChanged?.Invoke();
        UIManager.Instance?.RefreshPlayerStats();
    }
}
