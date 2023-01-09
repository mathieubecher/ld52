using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZOrder : MonoBehaviour
{
    private SpriteRenderer[] m_sprites;
    void Awake()
    {
        m_sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        int y = -(int)Mathf.Floor(transform.position.y * 10) * 10;
        foreach(var sprite in m_sprites)
        {
            sprite.sortingOrder = y + (sprite.sortingOrder%10);
        }
    }
}