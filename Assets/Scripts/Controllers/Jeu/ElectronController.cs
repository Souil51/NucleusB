using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronController : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(StaticResources.TAG_PLAYER_CHARGE))
        {
            Destroy(collision.gameObject);
        }

        if(collision.CompareTag(StaticResources.TAG_CERCLE_PLAYER))
        {
            Destroy(this.gameObject);
        }
    }
}
