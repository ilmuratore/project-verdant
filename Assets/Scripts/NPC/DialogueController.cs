using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("Riferimenti UI")]
    public GameObject pannelloDialogo;
    public TMP_Text testoRiga;
    public TMP_Text nomeParlante;
    public Button bottoneAccetta;

    [Header("Riferimenti player")]
    public PlayerMovement player;

    [Header("Righe Dialogo")]
    [TextArea]
    public string[] righe = new string[]
    {
        "Monaco: Eroe, hanno invaso il villaggio!",
        "Monaco: i nemici sono numerosi, attento!",
        "Eroe: conta su di me!",
        "Monaco: eliminali tutti. Difendi il villaggio!"
    };

    private NPC_AI npcCorrente;
    private int indiceRiga = 0;
    private bool dialogoAperto = false;

    void Start()
    {
        if (pannelloDialogo != null) pannelloDialogo.SetActive(false);
        if(bottoneAccetta != null)
        {
            bottoneAccetta.gameObject.SetActive(false);
            bottoneAccetta.onClick.AddListener(AccettaQuest);
        }
    }

    void Update()
    {
        if (!dialogoAperto) return;
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ProssimaRiga();
        }
    }

    public void ApriDialogo(NPC_AI npc)
    {
        npcCorrente = npc;
        dialogoAperto = true;
        indiceRiga = 0;
        if (player != null) player.BloccaController();
        if (pannelloDialogo != null) pannelloDialogo.SetActive(true);
        if (bottoneAccetta != null) bottoneAccetta.gameObject.SetActive(false);

        MostraRiga();
    }

    private void ProssimaRiga()
    {
        if (indiceRiga >= righe.Length - 1)
        {
            return;
        }
        indiceRiga++;
        MostraRiga();
    }

    private void MostraRiga()
    {
        if (testoRiga != null) testoRiga.text = righe[indiceRiga];
        bool ultima = (indiceRiga >= righe.Length - 1);
        if (bottoneAccetta != null) bottoneAccetta.gameObject.SetActive(ultima);
    }

    private void AccettaQuest()
    {
        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.IniziaQuest();
        }
        ChiudiDialogo();
    }

    private void ChiudiDialogo()
    {
        dialogoAperto = false;
        if (pannelloDialogo != null) pannelloDialogo.SetActive(false);
        if (bottoneAccetta != null) bottoneAccetta.gameObject.SetActive(false);

        if (player != null) player.SbloccaController();
        if (npcCorrente != null) npcCorrente.FineDialogo();
        npcCorrente = null;
    }
}
