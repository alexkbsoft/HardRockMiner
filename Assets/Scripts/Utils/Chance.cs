using UnityEngine;

public class Chance
{
    public static int GetRandomTier(float[] tiers)
    {
        var luck = Random.Range(0, 1.0f);
        // previous chance
        var prev = 0.0;
        // Iterate through possible tiers
        for (var tier = 0; tier < tiers.Length; tier += 1)
        {
            // ignore tiers with no chance
            if (tiers[tier] == 0.0)
            {
                continue;
            }

            // check whether chance has given us this tier
            else if (prev <= luck && luck <= prev + tiers[tier])
            {
                return tier;
            }

            // update chance range
            prev += tiers[tier];
        }

        return 0;
    }
}