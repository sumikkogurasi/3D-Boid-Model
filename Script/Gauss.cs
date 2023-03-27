using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gauss : MonoBehaviour
{
    [SerializeField]
    public GameObject target;
    // Update is called once per frame
    void Start()
    {
        
        var distance = Mathf.Abs(NormalDistribution() * 333f);
        var duration = Mathf.Abs(NormalDistribution() * 1f);

        Instantiate(target, new Vector2(distance, duration), Quaternion.identity);
    }

    float NormalDistribution()
    {
        var ret = 0f;
        for (int i = 0; i < 12; i++)
        {
            ret += Random.value;
        }
        return ret - 6f;
    }
}
