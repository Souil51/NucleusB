using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public enum Pattern { BRANCHE_4 = 0, BRANCHE_3, BRANCHE_5, BRANCHE_2, BRANCHE_1, LASER_1, LASER_2, BARRIERE}

public class NoyauController : MonoBehaviour
{
    private float m_fBord;
    private readonly float m_fDistanceFromNoyau = 0f;
    private readonly float m_fMinScale = 0.5f;
    private readonly float m_fRapportScale = 0.5f;
    private float m_nucleonCount = 4;
    private float m_fAnglePerNucleon;
    private float m_fNoyauLeftDuration = 0;//Temps restants
    private float m_fOldNoyauLeftDuration = 0;//Temps restant de la frame précédente
    private float m_fTimePassed = 0;
    private readonly List<GameObject> m_lstCouches = new List<GameObject>();
    private readonly List<GameObject> m_lstNucleons = new List<GameObject>();
    private List<LevelPattern> m_lstPatterns;
    private bool m_bIsInit = false;
    private bool m_bPatternCanStart = false;

    private GameController m_gameCtrl;

    private IEnumerator coroutine_pattern;


    // Update is called once per frame
    void Update()
    {
        if (!m_bIsInit)
            return;

        if (!m_gameCtrl.IsPaused())
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 0.25f);

            UpdateLeftDuration((GetLevelCompletionFromNucleons() * m_gameCtrl.GetLevelDuration()) - Time.deltaTime);

            float fCompletion = (m_gameCtrl.GetLevelDuration() - m_fNoyauLeftDuration) / m_gameCtrl.GetLevelDuration();

            UpdateLevelCompletion(fCompletion);
        }

        if (m_bPatternCanStart)
        {
            //Gestion des patterns
            //Compatage du temps écoulé dans m_fTimePassed
            //Et on vérifie à chaque update si un ou plusieurs patterns doivent être lancé
            //La liste des patterns sera limitée dans ça ne dervait pas poser de soucis de performances
            //S'il y a trop de patterns, à voir pour créer des pools de patterns pour faire des recherches seulement dans les patterns de 0 à 10 sec puis 10 à 20...
            List<LevelPattern> lstPatternToLaunch = m_lstPatterns.FindAll(item => item.m_fLaunchTime < m_fTimePassed);

            foreach (LevelPattern lvlPattern in lstPatternToLaunch)
            {
                StartPattern(lvlPattern.m_pattern, lvlPattern.m_nAngle, lvlPattern.m_fTimeBetweenShots, lvlPattern.m_nForce, lvlPattern.m_fRotationAngle, lvlPattern.m_fDuration);

                m_lstPatterns.Remove(lvlPattern);
            }

            m_fTimePassed += Time.deltaTime;
        }
    }

    public void InitNoyau(int nbNucleon)
    {
        m_nucleonCount = nbNucleon;

        m_gameCtrl = GameObject.FindGameObjectWithTag(StaticResources.TAG_GAME_CONTROLLER).GetComponent<GameController>();

        m_fBord = this.gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;

        IEnumerator coroutineTest = PatternTest();
        StartCoroutine(coroutineTest);

        m_fAnglePerNucleon = 360 / m_nucleonCount;

        InitNucleons();

        m_bIsInit = true;
    }

    public void InitDuration(float fDuration)
    {
        UpdateLeftDuration(fDuration);
        m_fOldNoyauLeftDuration = m_fNoyauLeftDuration;
    }

    private void InitNucleons()
    {
        GameObject goNucleon;
        NucleonController nucleonCtrl;

        for (int i = 0; i < m_nucleonCount; i++)
        {
            goNucleon = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_NUCELON));
            nucleonCtrl = goNucleon.GetComponent<NucleonController>();

            goNucleon.transform.eulerAngles = new Vector3(goNucleon.transform.eulerAngles.x, goNucleon.transform.eulerAngles.y, i * m_fAnglePerNucleon);
            goNucleon.transform.SetParent(this.transform);
            goNucleon.transform.localScale = new Vector3(1, 1, 1);
            goNucleon.transform.position = new Vector3(goNucleon.transform.position.x, goNucleon.transform.position.y, -(m_nucleonCount - i));
            
            if (i % 2 == 0)
                nucleonCtrl.SetSprite(NucleonController.NucleonType.PROTON);
            else
                nucleonCtrl.SetSprite(NucleonController.NucleonType.NEUTRON);

            nucleonCtrl.NucleonEvent += NucleonWasHit;
            nucleonCtrl.m_nIndex = i;

            m_lstNucleons.Add(goNucleon);
        }
    }

    private void UpdateNucleonsAngle()
    {
        int nNucleonIndex = GetReducedNucleonCourant();

        if (nNucleonIndex + 1 >= m_lstNucleons.Count - 1)
            return;

        int nAlive = 0;

        foreach (GameObject go in m_lstNucleons)
        {
            NucleonController nucCtrl = go.GetComponent<NucleonController>();

            if(!nucCtrl.IsDead())
                nAlive++;
        }

        if (nAlive == 0)
            return;

        float fAlpha = 360 / (nAlive);
        float fCurrentAngle = 0;

        int i = 0;

        foreach(GameObject go in m_lstNucleons)
        {
            NucleonController nucCtrl = go.GetComponent<NucleonController>();

            if(!nucCtrl.IsDead())
            {
                nucCtrl.ChangeAngle(i * fAlpha);

                fCurrentAngle += fAlpha;
                fCurrentAngle %= 360;

                i++;
            }
        }
    }

    public void AddCouche(GameObject go)
    {
        this.m_lstCouches.Add(go);
    }

    #region Gestion de la durée

    public void UpdateLevelCompletion(float fCompletion, bool bUpdateAfterNucleonHit = false)
    {
        if (m_lstNucleons.Count > 0)
        {
            int nNucleonIndex = GetReducedNucleonCourant();//Récupération du nucleon en cours de réduction

            float fPart = 1f / m_nucleonCount;

            int nLeftDurationIndex = (int)(m_gameCtrl.GetLevelCompletion() / fPart);
            int nOldLeftDurationIndex = (int)(m_gameCtrl.GetLevelCompletion(m_fOldNoyauLeftDuration) / fPart);

            //Si le nucleon courant doit disparaitre cette frame
            if ((nLeftDurationIndex != nOldLeftDurationIndex && !bUpdateAfterNucleonHit))
            {
                m_lstNucleons[nNucleonIndex].GetComponent<NucleonController>().Disappear();

                nNucleonIndex = GetReducedNucleonCourant();

                UpdateNucleonsAngle();
            }

            //Réduction du nucléon en cours
            if (!(nNucleonIndex > m_lstNucleons.Count - 1))
            {
                GameObject goCurrentNucleon = m_lstNucleons[nNucleonIndex];

                float fComple = fCompletion;

                float fCurrentComp = fComple - (fPart * ((int)(fComple / fPart)));
                fCurrentComp *= m_nucleonCount;
                fCurrentComp = 1 - fCurrentComp;

                float fNucleonbBaseScaleMin = 1 - m_fMinScale;
                float fNucleonNewScale = fNucleonbBaseScaleMin * fCurrentComp;

                goCurrentNucleon.transform.localScale = new Vector3(fNucleonNewScale + m_fMinScale, fNucleonNewScale + m_fMinScale, 1);

            }
        }

        m_gameCtrl.NoyauLeftDurationChanged(m_fNoyauLeftDuration);
    }

    public float GetDurationLeft()
    {
        return m_fNoyauLeftDuration;
    }

    private void UpdateLeftDuration(float fNewLeftDuration)
    {
        m_fOldNoyauLeftDuration = m_fNoyauLeftDuration;
        m_fNoyauLeftDuration = fNewLeftDuration;
    }

    //Calcul de la complétion à partir de l'état des nucleons
    private float GetLevelCompletionFromNucleons()
    {
        float fSomme = 0;

        foreach (GameObject go in m_lstNucleons)
        {
            NucleonController nucCtrl = go.GetComponent<NucleonController>();
            if (!nucCtrl.IsDead())
                fSomme += (go.transform.localScale.x - m_fMinScale) / m_fRapportScale;
        }

        fSomme /= m_nucleonCount;

        return fSomme;
    }

    public void NucleonWasHit(object sender, EventArgs args)
    {
        NucleonController nucleon = (NucleonController)sender;

        float fPart = 1f / m_nucleonCount;

        float fToRemove;

        int nIndexNucleon = GetReducedNucleonCourant();

        if (nucleon.m_nIndex != nIndexNucleon)
        {
            //C'est pas le nucleon en train de disparaitre ->
            fToRemove = fPart;
        }
        else
        {
            float fLevelCompleton = m_gameCtrl.GetLevelCompletion();

            float fTemp1 = fPart - (fLevelCompleton % fPart);

            fToRemove = fTemp1;
        }

        float fSecondsToRemove = m_gameCtrl.GetLevelDuration() * fToRemove; ;

        if (m_fNoyauLeftDuration - fSecondsToRemove < 0)
            UpdateLeftDuration(0);
        else
            UpdateLeftDuration(m_fNoyauLeftDuration - fSecondsToRemove);

        m_gameCtrl.NoyauLeftDurationChanged(m_gameCtrl.GetLevelCompletion());

        UpdateLevelCompletion(m_gameCtrl.GetLevelCompletion(), true);

        SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_PLAYER_CHARGE_HIT_NUCLEON);
        m_gameCtrl.NoyauHasBeenHit();

        nucleon.Disappear();
        UpdateNucleonsAngle();
    }

    private int GetReducedNucleonCourant(int nIndexExclu = -1)
    {
        int nRes = 0;

        for (int i = 0; i < m_lstNucleons.Count; i++)
        {
            if (!m_lstNucleons[i].GetComponent<NucleonController>().IsDead() && i != nIndexExclu)
            {
                nRes = i;
                break;
            }
        }

        return nRes;
    }

    #endregion

    #region Gestion des patterns

    public void StartPatterns()
    {
        m_bPatternCanStart = true;
    }

    private void ShootElectron(float fAngle, int nForce = 100)
    {
        float posX = (m_fBord + m_fDistanceFromNoyau) * Mathf.Cos(fAngle * Mathf.Deg2Rad);
        float posY = (m_fBord + m_fDistanceFromNoyau) * Mathf.Sin(fAngle * Mathf.Deg2Rad);

        GameObject goProj = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_ELECTRON));

        goProj.transform.position = new Vector3(posX, posY, goProj.transform.position.z);

        //Ajout d'une force dans la direction du regard de l'objet, donc vers le centre
        goProj.GetComponent<Rigidbody2D>().AddForce(goProj.transform.position.normalized * nForce);

        SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_TIR_ELECTRON, 0.2f);
    }

    private void SpawnLaser(float fStartingAngle, Pattern pattern, float fAngleRotation, float fDuration)
    {
        int nBranches = 1;

        switch (pattern)
        {
            case Pattern.LASER_1:
                nBranches = 1;
                break;
            case Pattern.LASER_2:
                nBranches = 2;
                break;
        }

        float fAngle = 360 / nBranches;

        for (int i = 0; i < nBranches; i++)
        {
            GameObject goLaser = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_LASER));

            LaserController laserCtrl = goLaser.GetComponent<LaserController>();

            float fCurrentStartingAngle = fStartingAngle + (i * fAngle);

            laserCtrl.InitLaser(fCurrentStartingAngle, fAngleRotation, fDuration);
        }
    }

    /// <summary>Lance de la pattern en paramètre</summary>
    /// <param name="nAngle">Angle principal</param>
    /// <param name="fTimeBetweenShots">Temps en secondes entre chaque tir</param>
    /// <param name="fDuration">Durée du pattern en secondes</param>
    private void StartPattern(Pattern pattern, int nAngle, float fTimeBetweenShots, int nForce, float fRotationAngle, float fDuration = 1.0f)
    {
        bool bStartCoroutine = false;

        switch (pattern)
        {
            case Pattern.BRANCHE_5:
            case Pattern.BRANCHE_4:
            case Pattern.BRANCHE_3:
            case Pattern.BRANCHE_2:
            case Pattern.BRANCHE_1:
                bStartCoroutine = true;
                coroutine_pattern = PatternCoroutine_Branches(pattern, nAngle, fTimeBetweenShots, nForce, fRotationAngle, fDuration);
                break;
            case Pattern.LASER_1:
            case Pattern.LASER_2:
                SpawnLaser((float)nAngle, pattern, fRotationAngle, fDuration);
                break;
            case Pattern.BARRIERE:
                InitRandomBarriereCouche(fDuration);
                break;
        }

        if (bStartCoroutine)
            StartCoroutine(coroutine_pattern);
    }

    private void InitRandomBarriereCouche(float fDuration)
    {
        int nIndexStart = UnityRandom.Range(0, CoucheController.m_nNombreParties - 1);

        int nCouche = UnityRandom.Range(0, m_lstCouches.Count);

        float fRandType = UnityRandom.Range(0f, 1f);
        int nMaxPart = 0;

        if (fRandType <= 0.9f)
            nMaxPart = 5;
        else if (fRandType > 0.9f && fRandType <= 0.98f)
            nMaxPart = 10;
        else if (fRandType > 0.98f)
            nMaxPart = 15;

        int nbPart = UnityRandom.Range(1, nMaxPart);

        m_lstCouches[nCouche].GetComponent<CoucheController>().InitBarriere(nIndexStart, nbPart, fDuration);
    }

    public void SetPatterns(int nWorld, int nLevel)
    {
        m_lstPatterns = LevelManager.GetPatterns(nWorld, nLevel);
    }

    #endregion

    #region Coroutines

    //Test des patterns
    private IEnumerator PatternTest()
    {
        yield return new WaitForSeconds(2);

        /*StartPattern(Pattern.BRANCHE_1, 45, 0.25f, 0f, 2.0f);

        yield return new WaitForSeconds(2);

        StartPattern(Pattern.BRANCHE_2, 45, 0.25f, 0f, 2.0f);

        yield return new WaitForSeconds(2);

        StartPattern(Pattern.BRANCHE_3, 45, 0.25f, 0f, 2.0f);

        yield return new WaitForSeconds(2);

        StartPattern(Pattern.BRANCHE_4, 45, 0.25f, 0f, 2.0f);

        yield return new WaitForSeconds(2);

        StartPattern(Pattern.BRANCHE_5, 45, 0.25f, 0f, 2.0f);

        yield return new WaitForSeconds(2);

        yield return new WaitForSeconds(2);*/

        //StartPattern(Pattern.LASER_1, 45, 0, 45, 8);

        //StartPattern(Pattern.BRANCHE_2, 45, 0.15f, 5f, 5.0f);

        //StartCoroutine(coroutine_TestBarriere());

        //yield return new WaitForSeconds(2);

        //StartPattern(Pattern.BARRIERE, -1, -1, -1, 5f);

        //StartPattern(Pattern.BRANCHE_2, 45, 0.3f, 10f, 10.0f);

        //m_lstCouches[0].GetComponent<CoucheController>().InitBarriere(0, 2, 5);

        //yield return new WaitForSeconds(1);

        //StartPattern(Pattern.BRANCHE_2, 45, 0.5f, 10f, 5.0f);

        //yield return new WaitForSeconds(1);

        //StartPattern(Pattern.LASER_1, 45, 0, 45, 5.0f);
    }


    //Test des barrières
    private IEnumerator coroutine_TestBarriere()
    {
        for(int i = 0; i < 50; i++)
        {
            InitRandomBarriereCouche(0.4f);

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <param name="nAngle">Angle principal, les autres branches de la croix sont à +90, +180 et +270 degrés</param>
    /// <param name="fTimeBetweenShots">Temps en secondes entre chaque tir</param>
    /// <param name="fDuration">Durée du pattern en secondes</param>
    private IEnumerator PatternCoroutine_Branches(Pattern pattern, int nMainAngle, float fTimePerShot, int nForce, float fRotationAngle, float fTimePattern = 1.0f)
    {
        float timeStart = Time.time;

        float fAngle;
        int nCount = 0;

        switch (pattern)
        {
            case Pattern.BRANCHE_5:
                nCount = 5;
                break;
            case Pattern.BRANCHE_4:
                nCount = 4;
                break;
            case Pattern.BRANCHE_3:
                nCount = 3;
                break;
            case Pattern.BRANCHE_2:
                nCount = 2;
                break;
            case Pattern.BRANCHE_1:
                nCount = 1;
                break;
        }

        fAngle = 360 / nCount;

        float fAngleRotation = 0f;

        while(true)
        {

            for(int i = 0; i < nCount; i++)
            {
                ShootElectron((nMainAngle + (i * fAngle)) + fAngleRotation, nForce);
            }

            if (Time.time - timeStart > fTimePattern)//Dès qu'on dépasse la durée du pattern, on l'arrête
                break;

            fAngleRotation = (fAngleRotation + fRotationAngle) % 360;

            yield return new WaitForSeconds(fTimePerShot);
        }
    }

    #endregion
}
