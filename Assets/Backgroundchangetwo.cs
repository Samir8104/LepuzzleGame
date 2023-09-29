using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgroundchangetwo : MonoBehaviour
{
    public Vector3 offset = new Vector3(0,-50,0);
    public GameObject player;
    public Transform playerloc;
     void LateUpdate() {
        transform.position = playerloc.position + offset;
    }
}
