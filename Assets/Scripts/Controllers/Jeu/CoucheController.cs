using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoucheController : MonoBehaviour
{
    public static int m_nNombreParties = 20;
    public static int m_nAngleParPartie = 18;
    public static int m_nOffsetChild = 1;

    private int m_nBarriereCount = 0;

    private Transform m_tCoucheCollider = null;
    private GameObject m_goNoyau = null;
    private GameController m_gameCtrl = null;

    // Start is called before the first frame update
    void Start()
    {
        m_tCoucheCollider = transform.Find(StaticResources.TRANSFORM_COUCHE_COLLIDER);

        int nCount = transform.childCount - m_nOffsetChild;

        int nCurrentAngle = m_nAngleParPartie * 2;

        //Une couche est composée de plusieurs partie pour pouvoir animer chaque partie de façon indépendante
        for(int i = 0; i < nCount; i++)
        {
            Transform tChild = transform.GetChild(i);
            tChild.gameObject.transform.rotation = Quaternion.Euler(0, 0, nCurrentAngle);

            nCurrentAngle -= m_nAngleParPartie;
        }

        m_goNoyau = GameObject.FindGameObjectWithTag(StaticResources.TAG_NOYAU);

        m_goNoyau.GetComponent<NoyauController>().AddCouche(gameObject);

        m_gameCtrl = GameObject.FindGameObjectWithTag(StaticResources.TAG_GAME_CONTROLLER).GetComponent<GameController>();
    }


    public void InitBarriere(int nStart, int nbPart, float fDuration, float fWarningDuration = 2f)
    {
        StartCoroutine(StartBarriereWarning(nStart, nbPart, fDuration, fWarningDuration));
    }

    private void InitBarriereAnimation(int nStart, int nbPart, float fDuration)
    {
        AddBarriereToCount();

        m_tCoucheCollider.gameObject.SetActive(true);

        List<Transform> lstPartAnimated = new List<Transform>();

        for (int i = nStart; i < nStart + nbPart; i++)
        {
            int nIndexCourant = i % m_nNombreParties;
            Transform childCurrent = transform.GetChild(nIndexCourant);

            childCurrent.GetComponent<Animator>().Play(StaticResources.ANIMATION_ANIMATION);

            lstPartAnimated.Add(childCurrent);
        }

        InitPolygonCollider(nStart, nbPart);

        StartCoroutine(WaitAndStopBarriere(lstPartAnimated, fDuration));
    }

    private void InitPolygonCollider(int nStartIndex, int nbParts)
    {
        int nAngleStart = nStartIndex * m_nAngleParPartie;

        int nAngleEnd = nAngleStart + nbParts * m_nAngleParPartie;

        PolygonCollider2D pc = m_tCoucheCollider.GetComponent<PolygonCollider2D>();

        List<Vector2> lstPoints = new List<Vector2>();

        int nPoints = 10;

        float fOuterRadius = m_gameCtrl.GetTailleCouche() + 0.1f;
        float fInnerRadius = m_gameCtrl.GetTailleCouche() - 0.1f;

        float fAngle = nAngleEnd - nAngleStart;
        float fIncreaseAngle = fAngle / (float)nPoints;
        float fCurrentAngle = -nAngleStart;

        for (int i = 0; i < nPoints + 1; i++)
        {
            float xOuter = fOuterRadius * Mathf.Cos(fCurrentAngle * Mathf.Deg2Rad);
            float yOuter = fOuterRadius * Mathf.Sin(fCurrentAngle * Mathf.Deg2Rad);

            lstPoints.Add(new Vector2(xOuter, yOuter));

            fCurrentAngle -= fIncreaseAngle;
        }

        fCurrentAngle += fIncreaseAngle;

        for (int i = 0; i < nPoints + 1; i++)
        {
            float xInner = fInnerRadius * Mathf.Cos(fCurrentAngle * Mathf.Deg2Rad);
            float yInner = fInnerRadius * Mathf.Sin(fCurrentAngle * Mathf.Deg2Rad);

            lstPoints.Add(new Vector2(xInner, yInner));

            fCurrentAngle += fIncreaseAngle;
        }

        pc.points = lstPoints.ToArray();
        pc.SetPath(0, lstPoints.ToArray());
    }

    private void StopBarriere(List<Transform> lstTransforms)
    {
        m_tCoucheCollider.gameObject.SetActive(false);

        foreach (Transform t in lstTransforms)
        {
            t.GetComponent<Animator>().Play(StaticResources.ANIMATION_IDLE);
        }

        RemoveBarriereToCount();
    }

    private void AddBarriereToCount()
    {
        m_nBarriereCount++;

        if(m_nBarriereCount == 1)
            SoundManager.PlayAudioSource(GetComponent<AudioSource>());
    }

    private void RemoveBarriereToCount()
    {
        m_nBarriereCount--;

        if(m_nBarriereCount == 0)
            GetComponent<AudioSource>().Stop();
    }

    private IEnumerator WaitAndStopBarriere(List<Transform> lstTransforms, float fDuration)
    {
        yield return new WaitForSeconds(fDuration);

        StopBarriere(lstTransforms);
    }

    private IEnumerator StartBarriereWarning(int nStart, int nbPart, float fDuration, float fDurationWarning)
    {
        for (int i = nStart; i < nStart + nbPart; i++)
        {
            Transform childCurrent = transform.GetChild(i % m_nNombreParties);

            childCurrent.GetComponent<Animator>().Play(StaticResources.ANIMATION_COUCHE_WARNING);
        }

        yield return new WaitForSeconds(fDuration);

        for (int i = nStart; i < nStart + nbPart; i++)
        {
            Transform childCurrent = transform.GetChild(i % m_nNombreParties);

            childCurrent.GetComponent<Animator>().Play(StaticResources.ANIMATION_IDLE);
        }

        InitBarriereAnimation(nStart, nbPart, fDurationWarning);
    }
}
