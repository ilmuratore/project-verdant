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

    [Header("Nomi oggetti nel prefab HUD")]
    [SerializeField] private string hudRootName = "HUD";
    [SerializeField] private string pannelloDialogoName = "Panel_Quest";
    [SerializeField] private string testoRigaName = "TestoDialogo";
    [SerializeField] private string nomeParlanteName = "Personaggio";
    [SerializeField] private string bottoneAccettaName = "Accetta";

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
    private bool buttonBound = false;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        ResolveReferences();
        BindButton();

        if (pannelloDialogo != null) pannelloDialogo.SetActive(false);
        if (bottoneAccetta != null) bottoneAccetta.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!dialogoAperto) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ProssimaRiga();
        }
    }

    private void ResolveReferences()
    {
        Transform hudRoot = SceneReferenceFinder.ResolveSceneTransform(null, hudRootName);
        Transform root = hudRoot != null ? hudRoot : transform;

        pannelloDialogo = SceneReferenceFinder.ResolveSceneObject(pannelloDialogo, root, pannelloDialogoName);
        Transform panelRoot = pannelloDialogo != null ? pannelloDialogo.transform : root;

        testoRiga = SceneReferenceFinder.ResolveComponentInChildren(testoRiga, panelRoot, testoRigaName);
        nomeParlante = SceneReferenceFinder.ResolveComponentInChildren(nomeParlante, panelRoot, nomeParlanteName);
        bottoneAccetta = SceneReferenceFinder.ResolveComponentInChildren(bottoneAccetta, panelRoot, bottoneAccettaName);

        if (player == null || !SceneReferenceFinder.IsSceneInstance(player))
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerMovement>();
            }
        }
    }

    private void BindButton()
    {
        if (buttonBound) return;
        if (bottoneAccetta == null) return;

        bottoneAccetta.onClick.RemoveAllListeners();
        bottoneAccetta.onClick.AddListener(AccettaQuest);
        buttonBound = true;
    }

    public void ApriDialogo(NPC_AI npc)
    {
        ResolveReferences();
        BindButton();

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
        if (righe == null || righe.Length == 0) return;

        if (testoRiga != null) testoRiga.text = righe[indiceRiga];
        if (nomeParlante != null) nomeParlante.text = "Monaco";

        bool ultima = indiceRiga >= righe.Length - 1;
        if (bottoneAccetta != null) bottoneAccetta.gameObject.SetActive(ultima);
    }

    private void AccettaQuest()
    {
        if (QuestManager.Instance != null)
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
