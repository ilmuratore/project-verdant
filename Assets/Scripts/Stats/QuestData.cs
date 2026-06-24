using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ondata
{
    public string nome = "Ondata";
    public int numeroNemici = 3;
    public bool ondataFinale = false;
}

[CreateAssetMenu(fileName = "QuestData", menuName = "RPG/Quest")]
public class QuestData : ScriptableObject
{
    public string titolo = "Il villaggio invaso";
    [TextArea] public string descrizione = "Hanno invaso il villaggio. Elimina tutti i nemici e proteggi i monaci.";
    public int xpRicompensa = 100;
    public List<Ondata> ondate = new List<Ondata>();

    public int TotaleNemici
    {
        get
        {
            int totale = 0;
            if (ondate == null) return totale;

            foreach (Ondata ondata in ondate)
            {
                if (ondata != null) totale += Mathf.Max(0, ondata.numeroNemici);
            }

            return totale;
        }
    }

    private void OnValidate()
    {
        if (ondate != null && ondate.Count > 0) return;

        ondate = new List<Ondata>
        {
            new Ondata { nome = "Prima ondata", numeroNemici = 3 },
            new Ondata { nome = "Seconda ondata", numeroNemici = 5 },
            new Ondata { nome = "Terza ondata", numeroNemici = 7 },
            new Ondata { nome = "Ondata finale", numeroNemici = 9, ondataFinale = true }
        };
    }
}
