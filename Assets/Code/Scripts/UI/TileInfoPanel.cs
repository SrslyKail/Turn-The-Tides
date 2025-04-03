using TMPro;
using TurnTheTides;
using UnityEngine;

public class TileInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI tileNameText;
    public void UpdateTileInfo(HexTile tile)
    {
        tileNameText.text = tile.Terrain.ToString();
    }
}
