using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitFondController : MonoBehaviour
{
    private readonly float m_fSpeed = 100;
    public int m_nSens = 1;

    public Sprite sprt_1;
    public Sprite sprt_2;
    public Sprite sprt_3;
    public Sprite sprt_4;

    // Start is called before the first frame update
    void Start()
    {
        float angle = 25;

        angle *= Mathf.Deg2Rad;
        float xComponent = Mathf.Cos(angle) * m_fSpeed;
        float zComponent = Mathf.Sin(angle) * m_fSpeed;
        Vector3 forceApplied = new Vector3(xComponent, zComponent, 0);

        //this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 25));
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(forceApplied * m_nSens);

        //Choix sprite
        int nRand = Random.Range(0, 4);
        SpriteRenderer sprtRenderer = GetComponent<SpriteRenderer>();

        switch (nRand)
        {
            case 0: sprtRenderer.sprite = sprt_1; break;
            case 1: sprtRenderer.sprite = sprt_2; break;
            case 2: sprtRenderer.sprite = sprt_3; break;
            case 3: sprtRenderer.sprite = sprt_4; break;
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
