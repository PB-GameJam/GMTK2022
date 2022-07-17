// Author: Eric Moser
// Date: 7/15/22

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public List<GameObject> BuildingPool;

    // Start is called before the first frame update
    void Start()
    {
        int spawn = 0;
        spawn = Random.Range(0, BuildingPool.Count);
        Instantiate(BuildingPool[spawn],transform);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
