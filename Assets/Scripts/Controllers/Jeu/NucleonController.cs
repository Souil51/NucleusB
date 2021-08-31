using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleonController : MonoBehaviour
{
    public enum NucleonType { PROTON, NEUTRON}

    public delegate void NucleonEventHandler(object sender, EventArgs args);

    public event NucleonEventHandler NucleonEvent;

    public Sprite sprt_RED;
    public Sprite sprt_BLUE;

    public Material material_RED;
    public Material material_BLUE;

    private Transform m_tNucleon;
    private ParticleSystem m_partSystem;

    private Sprite current_Sprt;
    private Material current_material;

    public int m_nIndex = 0;
    private bool m_bDead = false;

    // Start is called before the first frame update
    void Start()
    {
        if (m_tNucleon == null)
            m_tNucleon = this.transform.Find(StaticResources.TRANSFORM_NUCLEON);

        m_partSystem = this.transform.Find(StaticResources.TRANSFORM_PART_SUSTEM).GetComponent<ParticleSystem>();

        m_tNucleon.gameObject.GetComponent<SpriteRenderer>().sprite = current_Sprt;
        this.transform.Find(StaticResources.TRANSFORM_PART_SUSTEM).GetComponent<ParticleSystemRenderer>().material = current_material;

        m_tNucleon.gameObject.GetComponent<NucleonSpriteCollider>().NucleonSpriteEvent += NucleonSpriteColliderHit;

    }


    public void SetSprite(NucleonType type)
    {
        switch (type)
        {
            case NucleonType.PROTON:
                current_Sprt = sprt_RED;
                current_material = material_RED;
                break;
            case NucleonType.NEUTRON:
                current_Sprt = sprt_BLUE;
                current_material = material_BLUE;
                break;
        }
    }

    public void Disappear()
    {
        m_bDead = true;
        if (gameObject.activeSelf)
        {
            SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_NUCLEON_DIE);
            StartCoroutine(UnscaleAndExplode());
        }
    }

    public bool IsDead()
    {
        return m_bDead;
    }

    public void ChangeAngle(float fAngle)
    {
        StartCoroutine(MoveAngles(fAngle));
    }

    private void NucleonSpriteColliderHit(object sender, EventArgs args)
    {
        NucleonEvent?.Invoke(this, new EventArgs());
    }

    #region Coroutines
    private IEnumerator UnscaleAndExplode()
    {
        float fScale = 0.5f;

        for (float f = 1f / fScale; f >= 0; f -= Time.deltaTime)
        {
            m_tNucleon.localScale = new Vector3(f * fScale * m_tNucleon.localScale.x, f * fScale * m_tNucleon.localScale.y, m_tNucleon.localScale.z);

            if (m_tNucleon.localScale.x < 0.05f)
                break;

            yield return null;
        }

        m_tNucleon.gameObject.SetActive(false);
        m_partSystem.gameObject.SetActive(true);
        m_partSystem.Play();

        yield return new WaitForSeconds(1);

        this.gameObject.SetActive(false);
    }

    private IEnumerator MoveAngles(float fAngle)
    {
        float fDelta = gameObject.transform.localEulerAngles.z - fAngle;

        int nSigne = fDelta > 0 ? -1 : 1;

        float fAngleAdded;

        float fAbsValue = Mathf.Abs(fDelta);

        if (fAbsValue > 180)
            fAbsValue %= 180;

        for (float f = 2f; f >= 0; f -= Time.deltaTime)
        {
            fAngleAdded = Time.deltaTime * fAbsValue * nSigne / 2;

            gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y, gameObject.transform.localEulerAngles.z + fAngleAdded);

            yield return null;
        }
    }
    #endregion
}
