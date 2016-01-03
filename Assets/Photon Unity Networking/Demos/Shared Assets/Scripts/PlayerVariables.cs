using UnityEngine;

public class PlayerVariables
{
    private static Color[] playerColors = new Color[] {Color.yellow, Color.red, Color.green, Color.cyan};
    private static string[] playerColorNames = new string[] {"yellow", "red", "green", "cyan"};
    private static Material[] playerMaterials = new Material[playerColors.Length];

    public static Color GetColor(int playerId)
    {
        if (playerId <= 0)
        {
            return Color.white;
        }
        return playerColors[playerId%playerColors.Length];
    }

    public static string GetColorName(int playerId)
    {
        if (playerId <= 0)
        {
            return "none";
        }
        return playerColorNames[playerId%playerColors.Length];
    }

    public static Material GetMaterial(Material original, int playerId)
    {
        Material result = playerMaterials[playerId%playerMaterials.Length];

        if (result == null)
        {
            result = new Material(original);
            result.color = GetColor(playerId);
            playerMaterials[playerId%playerMaterials.Length] = result;
        }

        return result;
    }
}