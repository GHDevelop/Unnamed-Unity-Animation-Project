using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDrops : MonoBehaviour
{
    [SerializeField] public SerializableDictionaryObjectGOFl[] dropsAndChance;
    [SerializeField] public Vector3 offsetToDrop;

    [Header("Viewing Only")]
    [SerializeField] protected float[] cdf;
    [SerializeField] protected Transform myTransform;

    private void Awake()
    {
        FillCDFArray();
        myTransform = GetComponent<Transform>();
    }

    public void DropItem()
    {
        int selection = System.Array.BinarySearch(cdf, UnityEngine.Random.Range(0, cdf[cdf.Length - 1]));

        if (selection < 0)
        {
            selection = ~selection;
        }

        if (dropsAndChance[selection].key != null)
        {
            Instantiate(dropsAndChance[selection].key, myTransform.position + offsetToDrop, Quaternion.identity);
        }
    }

    private void FillCDFArray()
    {
        cdf = new float[dropsAndChance.Length];

        cdf[0] = dropsAndChance[0].value;
        for (int index = 1; index < dropsAndChance.Length; index++)
        {
            cdf[index] = dropsAndChance[index].value + cdf[index - 1];
        }
    }
}
