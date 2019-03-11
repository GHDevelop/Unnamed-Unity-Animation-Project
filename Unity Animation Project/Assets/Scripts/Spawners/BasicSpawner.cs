using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpawner : MonoBehaviour
{
    [Header("Manually Set")]

    [Tooltip("The object to spawn as a Game Object"),
        SerializeField] private GameObject objectToSpawn;

    [Tooltip("The time until the object respawns."),
        SerializeField] private float timeToRespawn;

    [Tooltip("The maximum number of the object that can be spawned at once."),
        SerializeField] private int numObjectsToSpawnMax;

    [Tooltip("Should the object spawn the moment it's loaded?"),
        SerializeField] private bool spawnImmediatelyOnLoad = false;

    [Header("Viewing Only")]

    [Tooltip("The attached transform. Set automatically. \nDO NOT EDIT"),
        SerializeField] private Transform myTransform;

    [Tooltip("The list of objects spawned by the spawner. \nDO NOT EDIT"),
        SerializeField] private List<GameObject> objectsSpawned = new List<GameObject>();

    [Tooltip("Used so that only one respawn timer can run at once. \nProbably not safe to edit."),
        SerializeField] private bool isCoroutineRunning = false;

    
    /// <summary>
    /// Spawns first object immediately if desired and gets transform.
    /// </summary>
    void Start()
    {
        myTransform = GetComponent<Transform>();
        if (spawnImmediatelyOnLoad)
        {
            objectsSpawned.Add(Instantiate(objectToSpawn, myTransform.position, myTransform.rotation));
        }
    }

    /// <summary>
    /// Maintains the list of spawned objects and spawns new ones as necessary
    /// </summary>
    void Update()
    {
        RemoveDestroyedObjectsFromList();
        SpawnObjectIfAble();
    }

    /// <summary>
    /// Starts the timer to spawn an object if the spawner is able to.
    /// </summary>
    private void SpawnObjectIfAble()
    {
        if (objectsSpawned.Count < numObjectsToSpawnMax && isCoroutineRunning == false)
        {
            StartCoroutine("SpawnObjectAfterTimer");
        }
    }

    /// <summary>
    /// Removes objects that are no longer present from the spawned objects list
    /// </summary>
    private void RemoveDestroyedObjectsFromList()
    {
        for (int index = 0; index < objectsSpawned.Count; index++)
        {
            if (objectsSpawned[index] == null)
            {
                objectsSpawned.Remove(objectsSpawned[index]);
            }
        }
    }

    /// <summary>
    /// Timer for spawning objects.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnObjectAfterTimer()
    {
        isCoroutineRunning = true;

        for (float timer = timeToRespawn; timer > 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        objectsSpawned.Add(Instantiate(objectToSpawn, myTransform.position, myTransform.rotation));
        isCoroutineRunning = false;
    }
}
