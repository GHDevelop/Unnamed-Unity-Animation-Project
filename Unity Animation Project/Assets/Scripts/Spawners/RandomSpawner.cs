using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : BasicSpawner
{
    [Header("Manually Set")]

    [Tooltip("The objects to spawn randomly as a Game Object"),
        SerializeField] private GameObject[] objectsToSpawn;

    protected override void Spawn()
    {
        //objectToSpawn is base class object, objectsToSpawn is randomly selected array
        objectToSpawn = objectsToSpawn[UnityEngine.Random.Range(0, objectsToSpawn.Length)];
        base.Spawn();
    }
}
