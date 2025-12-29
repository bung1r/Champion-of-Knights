using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "RoundManager/RoundDatabase")]
public class RoundDatabase : ScriptableObject
{
    public List<RoundData> rounds = new List<RoundData>();

    public RoundData GetRoundData(int roundIndex)
    {
        if (roundIndex - 1 < rounds.Count)
        {
            return rounds[roundIndex - 1];
        }
        else
        {
            return rounds[rounds.Count - 1];
        }
    }
}