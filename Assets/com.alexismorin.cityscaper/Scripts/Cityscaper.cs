using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cityscaper : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)]
    float urbanDensity = 0.5f;
    [SerializeField]
    [Range(0.2f, 10f)]
    float voxelSize = 0.4f;
    [SerializeField]
    [Range(0.2f, 1f)]
    float noiseMagnitude = 0.3f;
    [SerializeField]
    LayerMask spawnMask = Physics.DefaultRaycastLayers;

    [Space(10)]

    [SerializeField]
    GameObject[] buildingPrefabs;
    [SerializeField]
    GameObject[] naturePrefabs;

    List<Vector3> instancePositions;


    public void Regenerate()
    {
        Clear();

        Bounds bounds = GetComponent<BoxCollider>().bounds;
        Vector3 bottomLeft = bounds.min;
        Vector3 bottomRight = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        Vector3 endLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        Vector3 topRight = bounds.max;

        for (float z = bottomLeft.z; z < endLeft.z; z += voxelSize)
        {
            Vector3 voxelZ = new Vector3(bottomLeft.x, bottomLeft.y, z);
            Vector3 noiseAdjustedZ = new Vector3(voxelZ.x + Random.Range(-noiseMagnitude, noiseMagnitude), voxelZ.y + Random.Range(-noiseMagnitude, noiseMagnitude), voxelZ.z + Random.Range(-noiseMagnitude, noiseMagnitude));

            RaycastHit hitZ;
            if (Physics.Raycast(noiseAdjustedZ, transform.TransformDirection(Vector3.down), out hitZ, 100f, spawnMask, QueryTriggerInteraction.Ignore))
            {
                instancePositions.Add(hitZ.point);
            }

            for (float x = bottomLeft.x; x < bottomRight.x; x += voxelSize)
            {
                Vector3 voxelX = new Vector3(x, bottomLeft.y, z);
                Vector3 noiseAdjustedX = new Vector3(voxelX.x + Random.Range(-noiseMagnitude, noiseMagnitude), voxelX.y + Random.Range(-noiseMagnitude, noiseMagnitude), voxelX.z + Random.Range(-noiseMagnitude, noiseMagnitude));

                RaycastHit hitX;
                if (Physics.Raycast(noiseAdjustedX, transform.TransformDirection(Vector3.down), out hitX, 100f, spawnMask, QueryTriggerInteraction.Ignore))
                {
                    instancePositions.Add(hitX.point);
                }

                for (float y = bottomLeft.y; y < topRight.y; y += voxelSize)
                {
                    Vector3 voxelY = new Vector3(x, y, z);
                    Vector3 noiseAdjustedY = new Vector3(voxelY.x + Random.Range(-noiseMagnitude, noiseMagnitude), voxelY.y + Random.Range(-noiseMagnitude, noiseMagnitude), voxelY.z + Random.Range(-noiseMagnitude, noiseMagnitude));

                    RaycastHit hitY;
                    if (Physics.Raycast(noiseAdjustedY, transform.TransformDirection(Vector3.down), out hitY, 100f, spawnMask, QueryTriggerInteraction.Ignore))
                    {
                        instancePositions.Add(hitY.point);
                    }
                }
            }
        }

        Cityscape();
    }


    // clears all previews and spawned instances
    void Clear()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        instancePositions = new List<Vector3>();
    }

    void Cityscape()
    {
        for (int i = 0; i < instancePositions.Count; i++)
        {

            if (Random.Range(0f, 1f) > urbanDensity)
            {
                if (naturePrefabs.Length > 0)
                {
                    // spawn foliage
                    GameObject instancedDetailMesh = PrefabUtility.InstantiatePrefab(naturePrefabs[Random.Range(0, naturePrefabs.Length)]) as GameObject;
                    instancedDetailMesh.transform.parent = this.transform;
                    instancedDetailMesh.transform.position = instancePositions[i];
                    instancedDetailMesh.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                    instancedDetailMesh.transform.localScale *= Random.Range(0.9f, 2.2f);
                }
            }
            else
            {
                if (buildingPrefabs.Length > 0)
                {
                    if (Random.Range(0f, 1f) < (1f / Vector3.Distance(instancePositions[i], GetComponent<BoxCollider>().bounds.center)))
                    {
                        // spawn city
                        GameObject instancedDetailMesh = PrefabUtility.InstantiatePrefab(buildingPrefabs[Random.Range(0, buildingPrefabs.Length)]) as GameObject;
                        instancedDetailMesh.transform.parent = this.transform;
                        instancedDetailMesh.transform.position = instancePositions[i];
                        instancedDetailMesh.transform.rotation = this.transform.rotation;
                    }


                }
            }

        }
    }
}
