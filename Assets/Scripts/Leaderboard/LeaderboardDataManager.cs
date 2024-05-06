using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public static class LeaderboardDataManager
{
    public static List<KeyValuePair<string,float>> GetList()
    {
        List < KeyValuePair<string, float>> namesAndScores = new();
        for (int i = 1; i <= 8; i++)
        {
            string prefsKey = "leader" + i;
            string name;
            float score;
            if (PlayerPrefs.HasKey(prefsKey + "name"))
            {
                name = PlayerPrefs.GetString(prefsKey + "name");
            }
            else name = "Unnamed" + i;
            if (PlayerPrefs.HasKey(prefsKey + "highscore"))
            {
                score = PlayerPrefs.GetFloat(prefsKey + "highscore");
            }
            else score = 0;
            namesAndScores.Add(new(name, score));
        }
        return namesAndScores;
    }

    public static List<KeyValuePair<string, float>> GetListSorted()
    {
        List<KeyValuePair<string, float>> kvps = new();
        foreach (KeyValuePair<string, float> entry in GetList())
        {
            kvps.Add(entry);
        }
        return kvps.OrderBy(x => -x.Value).ToList();
    }

    public static void SetList(List<KeyValuePair<string, float>> namesAndScores)
    {
        List<KeyValuePair<string, float>> kvps = new();
        foreach (KeyValuePair<string, float> entry in namesAndScores)
        {
            kvps.Add(entry);
        }
        kvps = kvps.OrderBy(x => -x.Value).ToList();
        for (int i = 1; i <= 8; i++)
        {
            string prefsKey = "leader" + i;
            PlayerPrefs.SetString(prefsKey + "name", kvps[i-1].Key);
            PlayerPrefs.SetFloat(prefsKey + "highscore", kvps[i-1].Value);
        }
    }

    public static void TryAddToList(string name, float score)
    {
        List<KeyValuePair<string, float>> namesAndScores  = GetList();
        List<KeyValuePair<string, float>> kvps = new();
        foreach (KeyValuePair<string, float> entry in namesAndScores)
        {
            kvps.Add(entry);
        }
        kvps = kvps.OrderBy(x => -x.Value).ToList();
        
        for (int i = 0; i < kvps.Count; i++)
        {
            if (score >= kvps[i].Value)
            {
                kvps.RemoveAt(kvps.Count - 1);
                KeyValuePair<string, float> newLeader = new(name, score);
                kvps.Insert(i, newLeader);
                SetList(kvps);
                return;
            }
        }
    }

    public static bool CheckLeaderboardEligibility(float score)
    {
        List<KeyValuePair<string, float>> namesAndScores = GetList();
        foreach (KeyValuePair<string, float> entry in namesAndScores)
        {
            if (score >= entry.Value)
            {
                return true;
            }
        }
        return false;
    }
}