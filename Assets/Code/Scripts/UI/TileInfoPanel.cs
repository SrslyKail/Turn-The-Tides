using System.Text;
using TMPro;
using TurnTheTides;
using UnityEngine;

/// <summary>
/// Class to manage the tile info panel.
/// The tile info panel displays information about the currently selected tile.
/// 
/// Written by Gurjeet Bhangoo.
/// </summary>
public class TileInfoPanel : MonoBehaviour
{
    /// <summary>
    /// The reference to the tile name text component. (The one with the big bold text.)
    /// </summary>
    public TextMeshProUGUI tileNameText;
    /// <summary>
    /// The reference to the tile details text component.
    /// </summary>
    public TextMeshProUGUI tileDetailsText;

    /// <summary>
    /// The currently selected tile.
    /// </summary>
    [SerializeField]
    private HexTile currentTile;

    /// <summary>
    /// Updates the tile info panel with the currently selected tile.
    /// If no tile is selected, it clears the tile info panel.
    /// </summary>
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

    /// <summary>
    /// Updates the tile info panel with the specified tile.
    /// </summary>
    /// <param name="tile"></param>
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

    /// <summary>
    /// Clears the tile info panel.
    /// </summary>
    public void ClearTileInfo()
    {
        tileNameText.text = "No Tile Selected";
        tileDetailsText.text = string.Empty;
    }
}
