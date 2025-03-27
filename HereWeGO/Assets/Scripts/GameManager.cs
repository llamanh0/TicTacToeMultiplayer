using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Ayarlar")]
    public Sprite xSprite;
    public Sprite oSprite;
    public Color xColor = Color.red;
    public Color oColor = Color.blue;

    private NetworkVariable<int> currentPlayer = new NetworkVariable<int>(0);
    private NetworkVariable<int>[] cells = new NetworkVariable<int>[9];

    public bool IsMyTurn => IsHost && currentPlayer.Value == 0 || IsClient && currentPlayer.Value == 1;

    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < 9; i++) cells[i] = new NetworkVariable<int>(0);
    }

    public void PlayerClick(int cellIndex)
    {
        if (IsMyTurn && cells[cellIndex].Value == 0)
            MakeMoveServerRpc(cellIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void MakeMoveServerRpc(int cellIndex)
    {
        cells[cellIndex].Value = currentPlayer.Value + 1;
        currentPlayer.Value = (currentPlayer.Value + 1) % 2;
        UpdateCellClientRpc(cellIndex, cells[cellIndex].Value);
    }

    [ClientRpc]
    private void UpdateCellClientRpc(int cellIndex, int value)
    {
        Transform cell = transform.GetChild(cellIndex);
        CellController controller = cell.GetComponent<CellController>();

        if (value == 1) controller.SetSymbol(xSprite, xColor);
        else if (value == 2) controller.SetSymbol(oSprite, oColor);
    }
}