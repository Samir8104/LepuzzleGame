using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUpdate : MonoBehaviour
{
    public GameObject player;
    public Transform pastVers;
    public Vector3 recentLocation;
    private Vector3 offset = new Vector3(0,50,0);
    void Start() {
        recentLocation = pastVers.position + offset;
    }
    // Uses late update to not cause conflicts with when the frame is updated
    void LateUpdate()
    {
        if(pastVers.position != recentLocation + offset && Input.GetKeyDown(KeyCode.J)) {
            if(player.GetComponent<Player>().isGrounded) {
            transform.position = pastVers.position + offset;
            recentLocation = pastVers.position + offset;
        }
        }
    }
}
