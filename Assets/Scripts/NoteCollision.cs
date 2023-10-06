using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteCollision : MonoBehaviour
{
    public Transform MessageSpawnPoint;
    public GameObject message;
    private bool messageHasSpawned = false;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(!messageHasSpawned)
        {
            messageHasSpawned = true;
            GameObject a = Instantiate(message) as GameObject;
            a.transform.position = new Vector2(MessageSpawnPoint.position.x, MessageSpawnPoint.position.y);
        }
    }
}
