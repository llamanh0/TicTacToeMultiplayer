using UnityEditor.Build.Content;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public int row;
    public int col;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnMouseDown()
    {
        if(gameManager != null && gameManager.IsMyTurn())
        {
            gameManager.PlayerMove(row, col);
        }
    }
}
