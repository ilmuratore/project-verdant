using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    private const string SaveFileName = "savegame.json";
    public static string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void SaveCurrentGame()
    {
        PlayerSaveData data = PlayerProgressMemory.CaptureFromCurrentPlayer(true);
        if( data == null)
        {
            Debug.LogWarning("Salvataggio non riuscito.");
            return;
        }
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Partita salvata in: " + SavePath);
    }

    public static bool LoadSaveGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Nessun salvataggio presente in:" + SavePath);
            return false;
        }
        string json = File.ReadAllText(SavePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

        if (data == null || !data.hasData || string.IsNullOrWhiteSpace(data.sceneName))
        {
            Debug.LogWarning("Salvataggio non valido.");
            return false;
        }
        PlayerProgressMemory.SetRuntimeData(data);
        Time.timeScale = 1f;
        SceneManager.LoadScene(data.sceneName);
        return true;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.LogWarning("Salvataggio Eliminato: " + SavePath);
        } else
        {
            Debug.LogWarning("Nessun salvataggio da eliminare.");
        }
    }
}
