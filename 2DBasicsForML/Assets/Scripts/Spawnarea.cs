using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnarea : MonoBehaviour
{
    Vector3 origin;
    Vector3 range;

    public Color GizmosColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);

    private void Start() {
        origin = transform.position;
        range = transform.localScale / 2.0f;
    }

    public Vector3 GetRandomPosition(){
        Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x),
                                0,
                                Random.Range(-range.z, range.z));
        return randomRange + origin;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = GizmosColor;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

}
