using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveSprite : MonoBehaviour
{
    public SpriteRenderer spriteRender;
    public Sprite empty;
    void Start()
    {
        spriteRender.sprite = empty;
    }
}
