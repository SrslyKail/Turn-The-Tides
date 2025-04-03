using System.Text;
using TMPro;
using TurnTheTides;
using UnityEngine;

public class TileInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI tileNameText;
    public TextMeshProUGUI tileDetailsText;
    public void UpdateTileInfo(HexTile tile)
    {
        tileNameText.text = tile.Terrain.ToString();

        StringBuilder details = new StringBuilder();
        details.AppendLine($"({tile.latitude}, {tile.longitude})");
        details.AppendLine($"Elevation: {tile.Elevation}m");
        details.AppendLine($"Land Use: {tile.landUseLabel}");
        details.AppendLine($"Pollution: {tile.PollutionValue}");

        tileDetailsText.text = details.ToString();
    }
}
