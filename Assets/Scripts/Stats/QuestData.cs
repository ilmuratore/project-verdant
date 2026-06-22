using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ondata
{
    public string nome = "Ondata";
    public int numeroNemici = 3;

    [Tooltip("Se true, tutti i nemici di questi nemici generati (assalto finale)")]
    public bool ondataFinale = false;
}


[CreateAssetMenu(fileName = "QuestData", menuName = "RPG/Quest")]
public class QuestData : ScriptableObject
{
    [Header("Testi Quest")]
    public string titolo = "Il villaggio invaso";
    [TextArea] public string descrizione = "Hanno invaso il villaggio! Elimina tutti i nemici prima che tutti i monaci vengano uccisi!";

    [Header("Ricompensa")]
    public int xpRicompensa = 100;

    [Header("Ondate")]
    public List<Ondata> ondate = new List<Ondata>()
    {
        new Ondata { nome = "Prima ondata", numeroNemici = 3, ondataFinale = false},
        new Ondata { nome = "Seconda ondata", numeroNemici = 5, ondataFinale = false},
        new Ondata { nome = "Terza ondata", numeroNemici = 7, ondataFinale = false},
        new Ondata { nome = "Ondata finale", numeroNemici = 9, ondataFinale = true},

    };

    public int TotaleNemici
    {
        get { int totale = 0;
            foreach( Ondata o in ondate)
            {
                totale += o.numeroNemici;
            }
            return totale;
        }
    }
}
