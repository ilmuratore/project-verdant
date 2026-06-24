using UnityEngine;

public enum MiniMapIconType
{
    Player,
    Enemy,
    Monk,
    Quest,
    Objective
}

[RequireComponent(typeof(SpriteRenderer))]
public class MinimapIcon : MonoBehaviour
{
    public MiniMapIconType tipo = MiniMapIconType.Objective;
    public Color colore = Color.white;
    public int sortingOrder = 500;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplySettings();
    }

    private void OnValidate()
    {
        sr = GetComponent<SpriteRenderer>();
        ApplySettings();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    private void ApplySettings()
    {
        if (sr == null) return;
        sr.color = colore;
        sr.sortingOrder = sortingOrder;
    }
}
