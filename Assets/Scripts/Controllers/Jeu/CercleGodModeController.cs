using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CercleGodModeController : MonoBehaviour
{
    PlayerController m_playerCtrl = null;

    // Start is called before the first frame update
    void Start()
    {
        Transform player = this.transform.parent.transform;

        m_playerCtrl = player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticResources.TAG_ELECTRON)
            || collision.CompareTag(StaticResources.TAG_LASER) 
            || collision.CompareTag(StaticResources.TAG_BARRIERE))
        {
            m_playerCtrl.GodModeHasBeenHit();
        }
    }

    public void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
