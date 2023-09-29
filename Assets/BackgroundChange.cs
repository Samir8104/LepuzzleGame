using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundChange : MonoBehaviour
{
    public SpriteRenderer spriter;
    public GameObject player;
    public Sprite f;
    public Sprite p;         
    public Transform playerloc;
    void FixedUpdate() {
        if(player.GetComponent<move>().warping) {
            Debug.Log("warping");
            if(player.GetComponent<move>().inFuture) {
                spriter.sprite = p;
                            Debug.Log("topast");

            } else {
                spriter.sprite = f;
                            Debug.Log("tofuture");

            }
        }
    }
            void LateUpdate() {
        transform.position = playerloc.position;
    }
    }


