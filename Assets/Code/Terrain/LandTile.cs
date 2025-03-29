namespace TurnTheTides
{
    /// <summary>
    /// base for all land-based to extend from.
    /// Allows us to store all land terrain in the same collections.
    /// Made by Corey Buchan.
    /// </summary>
    internal class LandTile: HexTile
    {
        protected override void SetPollution()
        {
            pollutionValue = 0;
        }
    }
}
