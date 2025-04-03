using System.Text;
using TMPro;
using TurnTheTides;
using UnityEngine;

public class TileInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI tileNameText;
    public TextMeshProUGUI tileDetailsText;

    [SerializeField]
    private HexTile currentTile;

    public void UpdateTileInfo()
    {
        if (currentTile != null)
        {
            UpdateTileInfo(currentTile);
        }
        else
        {
            ClearTileInfo();
        }
    }

    public void UpdateTileInfo(HexTile tile)
    {
        currentTile = tile;

        tileNameText.text = tile.Terrain.ToString();

        StringBuilder details = new StringBuilder();
        details.AppendLine($"({tile.latitude}, {tile.longitude})");
        details.AppendLine($"Elevation: {tile.Elevation}m");
        details.AppendLine($"Land Use: {tile.landUseLabel}");
        details.AppendLine($"Pollution: {tile.PollutionValue}");

        tileDetailsText.text = details.ToString();
    }

    public void ClearTileInfo()
    {
        tileNameText.text = "No Tile Selected";
        tileDetailsText.text = string.Empty;
    }
}
