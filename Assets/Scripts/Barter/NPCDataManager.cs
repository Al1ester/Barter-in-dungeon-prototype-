using System.Collections.Generic;
using UnityEngine;

public class NPCDataManager : MonoBehaviour
{
    public static NPCDataManager Instance;
    private Dictionary<string, int> npcRelationshipStatuses = new Dictionary<string, int>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveRelationshipStatus(string npcID, int relationshipStatus)
    {
        npcRelationshipStatuses[npcID] = relationshipStatus;
    }

    public int GetRelationshipStatus(string npcID)
    {
        return npcRelationshipStatuses.ContainsKey(npcID) ? npcRelationshipStatuses[npcID] : 0;
    }
}
    