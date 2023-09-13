using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerOreCollection : MonoBehaviour
{
    public OreCollection storedOre = new OreCollection();
    
    [Header("Ore Pickup Spawn:")] // TODO: put this somewhere else at some point
    public float minOreAmount = 0.5f;
    
    [Space(5)]
    
    public PickUp coalPickupPrefab;
    public PickUp goldPickupPrefab;
    public PickUp boosterPickupPrefab;
    
    public void AddOre(OreCollection ore, Vector3 origin)
    {
        storedOre.AddOreCollection(ore);
        SpawnPickups(origin);
    }
    
    private void SpawnPickups(Vector3 origin)
    {
        if (storedOre[WallType.Gold] > minOreAmount)
            SpawnPickup(WallType.Gold, goldPickupPrefab, origin);

        if (storedOre[WallType.Coal] > minOreAmount)
            SpawnPickup(WallType.Coal, coalPickupPrefab, origin);
        
        if (storedOre[WallType.Booster] > minOreAmount)
            SpawnPickup(WallType.Booster, boosterPickupPrefab, origin);
    }

    private void SpawnPickup(WallType wallType, PickUp prefab) =>
        SpawnPickup(wallType, prefab, transform.position);
    private void SpawnPickup(WallType wallType, PickUp prefab, Vector3 origin)
    {
        Instantiate(prefab, origin, Quaternion.identity).GetComponent<PickUp>().amount = storedOre[wallType];
        storedOre[wallType] -= storedOre[wallType];
    }
}

[System.Serializable]
public class OreCollection : Dictionary<WallType, float>
{
    public OreCollection()
    {
        // initialize all ores to 0
        foreach (WallType wallType in (WallType[])Enum.GetValues(typeof(WallType)))
            TryAdd(wallType, 0);
    }

    public void AddOre(WallType wallType, float amount)
    {
        //TryAdd(wallType, 0);
        this[wallType] += amount;
    }

    public void AddOreCollection(OreCollection otherCollection)
    {
        foreach (WallType wallType in otherCollection.Keys)
            AddOre(wallType, otherCollection[wallType]);
    }
}