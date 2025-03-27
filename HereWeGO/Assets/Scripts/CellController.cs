using UnityEngine;

public class CellController : MonoBehaviour
{
    public int cellIndex; // 0-8 arasý
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        GameManager.Instance.PlayerClick(cellIndex);
    }

    public void SetSymbol(Sprite sprite, Color color)
    {
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
    }
}