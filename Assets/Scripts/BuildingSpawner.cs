// Author: Eric Moser
// Date: 7/15/22

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public List<GameObject> BuildingPool;
    public float RoadCheckRadius = 2f;

    // Start is called before the first frame update
    void Start()
    {
        int spawn = 0;
        spawn = Random.Range(0, BuildingPool.Count);
        FindRoad();
        Instantiate(BuildingPool[spawn],transform.position,Quaternion.identity);

    }

    public void FindRoad()
    {
        Collider[] sensed = Physics.OverlapSphere(transform.position, RoadCheckRadius);
        foreach (Collider item in sensed)
        {
            Debug.Log(item.name);
            if (item.CompareTag("Road"))
            {
                
                Vector3 rotationDir = (item.transform.position - transform.position);
                transform.rotation = Quaternion.Euler(new Vector3(0, rotationDir.y, 0));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
