using UnityEngine;


public enum MiniMapIconType
{ 
    Player,
    Enemy,
    Monk,
    Quest,
    Objective
}
public class MinimapIcon : MonoBehaviour
{

    [Header("Tipo Icona")]
    public MiniMapIconType tipo = MiniMapIconType.Objective;

    [Header("Aspetto")]
    public Color colore = Color.white;
    public int sortingOrder = 500;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = colore;
        sr.sortingOrder = sortingOrder;

    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
