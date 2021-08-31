using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonController : MonoBehaviour
{
    public enum PhotonColor { RED, BLUE, GREEN }

    public Sprite sprt_RED;
    public Sprite sprt_GREEN;
    public Sprite sprt_BLUE;

    private readonly string m_szQueueBleuAnimationClip = "queue_bleu";
    private readonly string m_szQueueRougeAnimationClip = "queue_rouge";
    private readonly string m_szQueueVerteAnimationClip = "queue_verte";
    private readonly float m_fSpeed = 100;
    private int m_nCouche = 1;
    private bool bStopped = false;
    public bool m_bPhotonStopped = false;
    private bool m_bIsCollected = false;
    public List<Sprite> m_lstSprites;
    public PhotonColor m_photonColor;

    private GameController m_gameCtrl = null;
    private Animator m_animator;
    private Transform m_tQueue;

    void Start()
    {
        float fTargetX = -this.transform.position.x;
        float fTargetY = -this.transform.position.y;

        //Calcul de l'angle avec le centre
        float angle = Mathf.Atan2(fTargetY, fTargetX) * Mathf.Rad2Deg;
        //Rotation pour regarder le centre
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //Ajout d'une force dans la direction du regard de l'objet, donc vers le centre
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(-transform.position.normalized * m_fSpeed);

        m_gameCtrl = GameObject.FindGameObjectWithTag(StaticResources.TAG_GAME_CONTROLLER).GetComponent<GameController>();

        this.m_lstSprites = new List<Sprite>() {sprt_RED, sprt_BLUE, sprt_GREEN};
        int nRand = Random.Range(0, m_gameCtrl.GetCoucheCount());
        this.gameObject.GetComponent<SpriteRenderer>().sprite = m_lstSprites[nRand];
        m_nCouche = nRand + 1;

        m_tQueue = this.transform.Find(StaticResources.TRANSFORM_PHOTON_QUEUE);
        m_animator = m_tQueue.GetComponent<Animator>();

        switch (nRand)
        {
            case 0:
                m_animator.Play(m_szQueueRougeAnimationClip);
                m_photonColor = PhotonColor.RED;
                break;
            case 1:
                m_animator.Play(m_szQueueBleuAnimationClip);
                m_photonColor = PhotonColor.BLUE;
                break;
            case 2:
                m_animator.Play(m_szQueueVerteAnimationClip);
                m_photonColor = PhotonColor.GREEN;
                break;
        }
    }

    void Update()
    {
        if (!bStopped)
        {
            float distanceToCenter = Mathf.Sqrt((transform.position.x * transform.position.x) + (transform.position.y * transform.position.y));

            if (distanceToCenter <= m_nCouche * m_gameCtrl.GetTailleCouche() && !m_bIsCollected)
            {
                bStopped = true;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                m_bPhotonStopped = true;

                StartDeath();
            }
        }
    }

    public void StartCollectAnimation()
    {
        if (!m_bIsCollected)
        {
            m_bIsCollected = true;
            StartCoroutine(QueueDisappear());
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.position.normalized * m_fSpeed / 10);
            GetComponent<Animator>().Play(StaticResources.ANIMATION_NUCLEON_TO_PLAYER);
        }
    }

    public bool IsAlreadyCollected()
    {
        return m_bIsCollected;
    }

    private void StartDeath()
    {
        GetComponent<Animator>().Play(StaticResources.ANIMATION_ELECTRON_STOPPED);
        StartCoroutine(QueueDisappear());
    }

    private void DestroyFromAnimation()
    {
        Destroy(gameObject);
    }

    private void DisableQueue()
    {
        m_tQueue.gameObject.SetActive(false);
    }

    #region Coroutines
    private IEnumerator Disappear(float fScale = 1.0f)
    {
        for (float f = 1f / fScale; f >= 0; f -= Time.deltaTime)
        {
            if (m_bIsCollected)
                yield break;

            gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, f * fScale);

            yield return null;
        }

        GameObject.Destroy(this.gameObject);
    }
    private IEnumerator QueueDisappear(float fScale = 10f)
    {
        for (float f = 1f / fScale; f >= 0; f -= Time.deltaTime)
        {
            m_tQueue.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, f * fScale);

            yield return null;
        }
    }
    #endregion
}
