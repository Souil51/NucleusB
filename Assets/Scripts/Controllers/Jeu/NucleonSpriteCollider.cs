using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleonSpriteCollider : MonoBehaviour
{
    public delegate void NucleonSpriteEventHandler(object sender, EventArgs args);

    public event NucleonSpriteEventHandler NucleonSpriteEvent;

    public int m_nIndex;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticResources.TAG_PLAYER_CHARGE))
        {
            NucleonSpriteEvent?.Invoke(this, new EventArgs());

            Destroy(collision.gameObject);
        }
    }
}
