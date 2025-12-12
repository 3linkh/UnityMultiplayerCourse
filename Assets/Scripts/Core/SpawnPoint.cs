using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    void OnEnable()
    {
        spawnPoints.Add(this);
    }
    void OnDisable()
    {
        spawnPoints.Remove(this);
    }
    public static Vector3 GetRandomSpawnPos()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.Log("No spawn points available!");
            return Vector3.zero;
        }
        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blueViolet;
        Gizmos.DrawSphere(transform.position, 1f);
    }

}
