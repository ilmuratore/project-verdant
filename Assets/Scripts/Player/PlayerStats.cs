using System;
using System.Data;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Configurazione Base")]
    public PlayerStatsData data;

    [Header("Progressione (runtime)")]
    public int level = 1;
    public int currentXp = 0;
    public int puntiDisponibili = 0;

    [Header("Punti spesi (runtime")]
    public int puntiAttacco = 0;
    public int puntiDifesa = 0;
    public int puntiVita = 0;


    public event Action OnStatsChanged;

    public int AttaccoEffettivo
    {
        get { return data.attaccoBase + puntiAttacco * data.incrementoAttaco; }
    }

    public int DifesaEffettivo
    {
        get { return data.difesaBase + puntiDifesa * data.incrementoDifesa; }
    }

    public int vitaMassimaEffettiva
    {
        get { return data.vitaBase + puntiVita * data.incrementoVita;  }
    }

    public int XpNecessari
    {
        get { return Mathf.RoundToInt(data.xpBaseLevelUp * Mathf.Pow(data.xpCrescita, level - 1)); }
    }

    public void AddXp(int amount)
    {
        if (amount <= 0) return;

        currentXp += amount;

        while(currentXp >= XpNecessari)
        {
            currentXp -= XpNecessari;
            LevelUp();
        }
        NotificaCambiamento();
    }

    private void LevelUp()
    {
        level++;
        puntiDisponibili += data.puntiPerLivello;
    }

    public bool SpendiPunto(StatType tipo)
    {
        if (puntiDisponibili <= 0) return false;
        switch (tipo)
        {
            case StatType.Attacco: puntiAttacco++; break;
            case StatType.Difesa: puntiDifesa++; break;
            case StatType.Vita: puntiVita++;
                PlayerHealth health = GetComponent<PlayerHealth>();
                if(health != null)
                {
                    health.AumentaVitaMassima(data.incrementoVita);
                }break;
        }
        puntiDisponibili--;
        NotificaCambiamento();
        return true;
    }

    public int ApplicaDifesa(int dannoInArrivo)
    {
        int dannoRidotto = dannoInArrivo - DifesaEffettivo;
        return Mathf.Max(1, dannoRidotto);
    }

    private void NotificaCambiamento()
    {
        if(OnStatsChanged != null)
        {
            OnStatsChanged.Invoke();
        }
    }
}

public enum StatType
{
    Attacco,
    Difesa,
    Vita

}
