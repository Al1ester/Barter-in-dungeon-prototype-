using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shopkeeper
{
    public string npcID;
    public string name;
    public int relationshipStatus;
    public int basePriceModifier;
    public List<ExchangeRate> exchangeRates = new List<ExchangeRate>();

    public int GetExchangeRate(string playerMaterial, string shopMaterial)
    {
        var rate = exchangeRates.Find(r => r.playerMaterial == playerMaterial && r.shopMaterial == shopMaterial);
        return rate != null ? rate.exchangeRate : 0;
    }

    public void LoadRelationshipStatus()
    {
        relationshipStatus = NPCDataManager.Instance.GetRelationshipStatus(npcID);
    }

    public void SaveRelationshipStatus()
    {
        NPCDataManager.Instance.SaveRelationshipStatus(npcID, relationshipStatus);
    }
}
