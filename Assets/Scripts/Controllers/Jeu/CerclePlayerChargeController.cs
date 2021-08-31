using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerclePlayerChargeController : MonoBehaviour
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
            m_playerCtrl.PlayerChargeHasBeenHit();
        }
    }

    public void DisableObject()
    {
        if(!m_playerCtrl.HasPlayerCharge())
            this.gameObject.SetActive(false);
    }
}
