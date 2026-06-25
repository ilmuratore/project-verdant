using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerProgressMemory
{ 
    public static PlayerSaveData RuntimeData { get; private set; }
    public static bool HasRuntimeData => RuntimeData != null && RuntimeData.hasData;
    //                  
    public static PlayerSaveData CaptureFromCurrentPlayer(bool applyPosition)
    {
        PlayerStats stats = Object.FindFirstObjectByType<PlayerStats>();
        PlayerHealth health = Object.FindAnyObjectByType<PlayerHealth>();

        if (stats == null || health == null) return null;

        Transform playerTransform = stats.transform;

        PlayerSaveData data = new PlayerSaveData
        {
            hasData = true,
            sceneName = SceneManager.GetActiveScene().name,
            applyPosition = applyPosition,
            level = stats.Level,
            currentXp = stats.CurrentXp,
            puntiDisponibili = stats.PuntiDisponibili,
            puntiAttacco = stats.PuntiAttacco,
            puntiDifesa = stats.PuntiDifesa,
            puntiVita = stats.PuntiVita,
            currentHealth = health.CurrentHealth,
            posX = playerTransform.position.x,
            posY = playerTransform.position.y,
            posZ = playerTransform.position.z
        };

        RuntimeData = data;
        return data;
    }

    public static void SetRuntimeData(PlayerSaveData data)
    {
        RuntimeData = data;
    }

    public static void Clear()
    {
        RuntimeData = null;
    }

    public static bool TryApplyToPlayer(PlayerStats stats, PlayerHealth health, Transform playerTransform)
    {
        if (!HasRuntimeData) return false;
        if (stats == null || health == null || playerTransform == null) return false;
        stats.ApplySaveData(RuntimeData);
        health.ApplySavedHealth(RuntimeData.currentHealth);
        if (RuntimeData.applyPosition)
        {
            playerTransform.position = new Vector3(RuntimeData.posX, RuntimeData.posY, RuntimeData.posZ);
        }
        UIManager.Instance?.RefreshPlayerStats();
        UIManager.Instance?.UpdatePlayerHealth(health.CurrentHealth, health.MaxHealth);
        return true;
    }
}
