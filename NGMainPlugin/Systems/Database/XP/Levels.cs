namespace PRMainPlugin.Systems.Database;

public class Levels
{
    internal static void CheckLevel(PlayerData data)
    {
        float multiplier = 1.083f;

        while (data.XP >= data.RequiredXP)
        {
            data.XP = (int)(data.XP - data.RequiredXP);
            data.Level++;

            data.RequiredXP = (int)(data.RequiredXP * multiplier);
        }
    }
}
