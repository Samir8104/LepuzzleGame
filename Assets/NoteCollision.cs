using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCollision : MonoBehaviour
{
    public Transform MessageSpawnPoint;
    public GameObject message;
    
    void OnTriggerEnter2D(Collider2D col) {
        GameObject a = Instantiate(message)  as GameObject;
        a.transform.position = new Vector2(MessageSpawnPoint.position.x, MessageSpawnPoint.position.y);
    }
}
