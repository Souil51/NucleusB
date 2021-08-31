using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //La vitesse n'est pas proportionnelle à la couche sinon le changement de couche modifie trop la vitesse (x2)
    private readonly Dictionary<int, float> m_dicSpeedParCouche = new Dictionary<int, float>()
    {
        { 1, 1.2f },
        { 2, 0.8f },
        { 3, 0.6f}
    };

    private float m_fCurrentAngle;
    private float m_fCurrentSpriteAngle;
    private float m_fDistanceNoyau;
    private float m_fMoveSpeed = 1.2f;
    private readonly float m_fPlayerChargeRorationSpeed = 2f;
    private int m_nCurrentCouche = 1;
    private int m_nNombreCouche = 2;
    private int m_nChargeCounter;
    private int m_nSanteCounter;
    private bool m_bFirstFrame = true;
    private bool m_bGodModAvailable = true;
    private bool m_bCoucheHasChanged_Z = false;
    private bool m_bCoucheHasChanged_S = false;
    private bool m_IsInRecoverymode = false;
    private bool m_isInGodMode = false;
    private bool m_hasPlayerCharge = false;
    private bool m_bPlayerChargeReady = false;
    private float m_nCurrentPlayerChargeAngle = 0;
    private bool m_bStop = false;
    private bool m_blayerChargeNegatif = false;
    private bool m_blayerChargePositif = false;
    private bool m_bAudioMaxChargeIsPlaying = false;
    private bool m_bIsDead = false;

    private GameController m_gameCtrl;
    private Transform tCerclePlayerCharge = null;
    private Transform tPlayerChargeHolder = null;
    private Transform tPlayerCharge = null;
    private Transform tCerclePlayerGodMode = null;
    private Animator m_animator;

    /* Audio */
    private AudioSource m_audioSource = null;
    public AudioClip m_audioCharge = null;
    public AudioClip m_audioMaxCharge = null;
    public AudioClip m_audioShotPlayerCharge = null;

    private IEnumerator coroutine_recovery;
    private IEnumerator coroutine_endScreen;
    private IEnumerator coroutine_godMode;

    private bool m_bInit = false;
    private bool m_bIsChangingCouche = false;
    private bool m_bIsStopped = false;


    void Update()
    {
        if (!m_bInit)
            return;

        if (m_bFirstFrame)
        {
            UpdateCharges();
            UpdateSante();
            m_bFirstFrame = false;
        }

        if (m_gameCtrl.m_IsGameEnded)
            return;

        if (!m_gameCtrl.IsPaused() && !m_bIsStopped)
        {
            //Déplacement sur la couche
            if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_RIGHT, DataManagerController.instance.m_keyboardLayout)) && !m_bStop)
            {
                MovePlayer(-m_fMoveSpeed);
            }
            else if (Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_LEFT, DataManagerController.instance.m_keyboardLayout)) && !m_bStop)
            {
                MovePlayer(m_fMoveSpeed);
            }

            bool bKeyZ = Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_UP, DataManagerController.instance.m_keyboardLayout));
            bool bKeyS = Input.GetKey(StaticResources.GetKeyCode(StaticResources.KEY_DOWN, DataManagerController.instance.m_keyboardLayout));

            bool bChangeCoucheOK = false;

            //CHangement de couche
            if ((bKeyZ || bKeyS) && m_nChargeCounter > 0 && !m_bIsChangingCouche)
            {
                m_bIsChangingCouche = true;

                if (bKeyZ)
                {
                    if (m_nCurrentCouche < m_nNombreCouche)
                    {
                        m_nCurrentCouche++;
                        m_fDistanceNoyau += m_gameCtrl.GetTailleCouche();
                        bChangeCoucheOK = true;

                        AudioChangeCouche();
                        MovePlayer(0);
                    }
                }
                else if (bKeyS)
                {
                    if (m_nCurrentCouche > 1 && m_nNombreCouche > 1)
                    {
                        m_nCurrentCouche--;
                        m_fDistanceNoyau -= m_gameCtrl.GetTailleCouche();
                        bChangeCoucheOK = true;

                        AudioChangeCouche();
                        MovePlayer(0);
                    }
                }

                if (((bKeyZ && !m_bCoucheHasChanged_Z) || (bKeyS && !m_bCoucheHasChanged_S)) && bChangeCoucheOK)
                {
                    if (bKeyZ)
                        m_bCoucheHasChanged_Z = true;

                    if (bKeyS)
                        m_bCoucheHasChanged_S = true;

                    m_nChargeCounter--;
                    UpdateCharges();
                }

                if (bChangeCoucheOK)
                {
                    m_fMoveSpeed = m_dicSpeedParCouche[m_nCurrentCouche];
                }

                StartCoroutine(WaitAndAllowChangingCouche());
            }

            if (!bKeyS)
            {
                m_bCoucheHasChanged_S = false;
            }

            if (!bKeyZ)
            {
                m_bCoucheHasChanged_Z = false;
            }

            //God mode seulement si plus de 1 PV et pas en recovery
            if (Input.GetKey(StaticResources.KEY_ACTION) && m_bGodModAvailable && !m_isInGodMode && m_nSanteCounter > 1 && !m_IsInRecoverymode)
            {
                m_isInGodMode = true;

                m_nSanteCounter--;

                UpdateSante();

                tCerclePlayerGodMode.gameObject.SetActive(true);

                coroutine_godMode = GodModeAnimation();

                StartCoroutine(coroutine_godMode);

                m_bGodModAvailable = false;
            }

            bool bEKeyDown = Input.GetKey(StaticResources.Key_ACTION_2);


            //Relachement de la touche d'action en ayant la player charge chargée
            if (m_bStop && !bEKeyDown)
            {
                if (!m_bPlayerChargeReady)
                {
                    StopAudioSource();
                    
                    m_bStop = false;
                }
                else
                {
                    AudioPlayerShotPlayerCharge();
                    ShotPlayerCharge();
                    m_hasPlayerCharge = false;
                    m_bStop = false;
                }
            }

            //E = tir d'une playerCharge (fait des dégats au noyau, fait un bond dans la complétion du niveau)
            if (m_hasPlayerCharge)
            {
                //La plyer charge tourne autour du joueur si il n'est pas en train d'appuyer sur la touche d'action
                if (!m_bStop)
                {
                    tPlayerChargeHolder.localRotation = Quaternion.Euler(0, 0, m_nCurrentPlayerChargeAngle);
                    m_nCurrentPlayerChargeAngle = (m_nCurrentPlayerChargeAngle + m_fPlayerChargeRorationSpeed) % 360;
                }

                //Premier appuie sur la touche
                if (bEKeyDown && !m_bStop)
                {
                    AudioPlayerCharge();
                    m_bStop = true;
                    tPlayerCharge.localScale = new Vector3(0.12f, 0.12f, tPlayerCharge.localScale.z);
                    m_blayerChargeNegatif = m_nCurrentPlayerChargeAngle > 180;
                    m_blayerChargePositif = m_nCurrentPlayerChargeAngle < 180;
                }

                //Si on appuie sur la touche, la playuer charge se déplace face au noyau puis grossit
                if (m_bStop)
                {
                    //Gestion du déplacement dans un sens
                    if (m_nCurrentPlayerChargeAngle < 180 && m_blayerChargePositif)
                    {
                        m_nCurrentPlayerChargeAngle = (m_nCurrentPlayerChargeAngle + m_fPlayerChargeRorationSpeed) % 360;
                        tPlayerChargeHolder.localRotation = Quaternion.Euler(0, 0, m_nCurrentPlayerChargeAngle);
                    }
                    else if (m_blayerChargePositif)
                    {
                        if (tPlayerCharge.localScale.x <= 0.2f)
                        {
                            tPlayerCharge.localScale = new Vector3(tPlayerCharge.localScale.x + 0.005f, tPlayerCharge.localScale.y + 0.005f, tPlayerCharge.localScale.z);
                        }
                        else
                        {
                            m_bPlayerChargeReady = true;
                            AudioPlayerMaxCharge();
                        }
                    }

                    //Gestion du déplacement dans l'autre sens
                    if (m_nCurrentPlayerChargeAngle > 180 && m_blayerChargeNegatif)
                    {
                        m_nCurrentPlayerChargeAngle = (m_nCurrentPlayerChargeAngle - m_fPlayerChargeRorationSpeed) % 360;
                        tPlayerChargeHolder.localRotation = Quaternion.Euler(0, 0, m_nCurrentPlayerChargeAngle);
                    }
                    else if (m_blayerChargeNegatif)
                    {
                        if (tPlayerCharge.localScale.x <= 0.2f)
                        {
                            tPlayerCharge.localScale = new Vector3(tPlayerCharge.localScale.x + 0.005f, tPlayerCharge.localScale.y + 0.005f, tPlayerCharge.localScale.z);
                        }
                        else
                        {
                            m_bPlayerChargeReady = true;
                            AudioPlayerMaxCharge();
                        }
                    }
                }
            }
        }
    }


    //Initialise les composants du Player
    public void InitPlayer(int nbCouches)
    {
        m_nNombreCouche = nbCouches;

        m_gameCtrl = GameObject.FindGameObjectWithTag(StaticResources.TAG_GAME_CONTROLLER).GetComponent<GameController>();

        transform.position = new Vector3(m_gameCtrl.GetTailleCouche(), transform.position.y, transform.position.z);

        m_fCurrentAngle = 0f;
        m_fCurrentSpriteAngle = m_fCurrentAngle + 90f;

        m_fDistanceNoyau = this.transform.position.x;

        m_nChargeCounter = m_gameCtrl.GetMaxCharges();
        m_nSanteCounter = m_gameCtrl.GetMaxSante();

        tCerclePlayerCharge = this.transform.Find(StaticResources.TRANSFORM_CERCLE_PLAYER_CHARGE);
        tPlayerChargeHolder = this.transform.Find(StaticResources.TRANSFORM_PLAYER_CHARGE_HOLDER);
        tPlayerCharge = tPlayerChargeHolder.transform.Find(StaticResources.TRANSFORM_PLAYER_CHARGE);

        tCerclePlayerGodMode = this.transform.Find(StaticResources.TRANSFORM_CERCLE_GOD_MODE);

        m_animator = GetComponent<Animator>();

        m_audioSource = GetComponent<AudioSource>();

        m_bInit = true;
    }

    #region Getter / Setter

    public bool IsInRecoveryMode()
    {
        return m_IsInRecoverymode;
    }

    public void SetPlayerStop(bool bValue)
    {
        m_bIsStopped = bValue;
    }

    public bool IsDead()
    {
        return m_bIsDead;
    }

    //Passage en recovery mode (le joueur ne prend pas de dégat dans ce mode)
    private void SetRecoveryMode(bool bValue)
    {
        if (m_IsInRecoverymode && bValue) return;

        m_IsInRecoverymode = bValue;

        if (bValue)
        {
            SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_RECOVERY);

            m_gameCtrl.PlayerHasBeenHit();

            coroutine_recovery = RecoveryAnimation();
            StartCoroutine(coroutine_recovery);
        }
    }

    #endregion

    #region Fonction du player

    //Déplace le joueur selon la couche et l'angle voulu
    private void MovePlayer(float fAngle)
    {
        //MAJ de l'angle
        m_fCurrentAngle += fAngle;
        m_fCurrentAngle %= 360;

        float radValue = m_fCurrentAngle * Mathf.Deg2Rad;

        //Calcul des nouvelles positions
        float x = Mathf.Cos(radValue) * m_fDistanceNoyau;
        float y = Mathf.Sin(radValue) * m_fDistanceNoyau;

        this.transform.position = new Vector3(x, y, this.transform.position.z);

        m_fCurrentSpriteAngle += fAngle;

        //Rotation pour toujours faire face au centre
        this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_fCurrentSpriteAngle));
    }

    //Demande au GameController de mettre à jour les charges dans l'UI
    private void UpdateCharges()
    {
        m_gameCtrl.UpdatePlayerCharge(m_nChargeCounter);
    }

    //Demande au GameController de mettre à jour la santé dans l'UI
    private void UpdateSante()
    {
        m_gameCtrl.UpdatePlayerSante(m_nSanteCounter);
    }

    //Ajoute 1 de santé
    public void AddSante()
    {
        if (m_nSanteCounter < m_gameCtrl.GetMaxSante())
        {
            m_nSanteCounter++;

            UpdateSante();
        }
    }

    private void EndScreenAndRestart()
    {
        coroutine_endScreen = WaitAndDisappear(m_gameCtrl.EndGameScreen);
        StartCoroutine(coroutine_endScreen);
    }

    #endregion

    #region Player charge

    public void PlayerChargeHasBeenHit()
    {
        StopPlayerCharge();

        StopAudioSource();
        SetRecoveryMode(true);

        tCerclePlayerCharge.GetComponent<Animator>().Play(StaticResources.ANIMATION_PLAYER_CHARGE_HAS_BEEN_HIT);
    }

    private void StopPlayerCharge() 
    {
        m_hasPlayerCharge = false;
        m_bPlayerChargeReady = false;
        //tCerclePlayerCharge.gameObject.SetActive(false);
        tPlayerChargeHolder.gameObject.SetActive(false);
    }

    public void GodModeHasBeenHit()
    {
        
    }

    private void StopGodMode()
    {
        m_isInGodMode = false;
        tCerclePlayerGodMode.gameObject.SetActive(false);
    }

    //Donne au joueur une PlayerCharge
    public void GivePlayerCharge()
    {
        m_hasPlayerCharge = true;

        SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_GIVE_PLAYER_CHARGE);

        //Reset de la player charge
        tPlayerChargeHolder.gameObject.SetActive(true);
        tCerclePlayerCharge.gameObject.SetActive(true);
        tCerclePlayerCharge.GetComponent<CircleCollider2D>().enabled = true;
    }

    //Tir la player Charge
    private void ShotPlayerCharge()
    {
        //MAJ de l'angle
        GameObject goProj = (GameObject)Instantiate(Resources.Load(StaticResources.RESOURCE_PLAYER_CHARGE));

        //Calcul des nouvelles positions
        goProj.transform.position = new Vector3(tPlayerCharge.position.x, tPlayerCharge.position.y, -1);

        tPlayerChargeHolder.gameObject.SetActive(false);
        tCerclePlayerCharge.gameObject.SetActive(false);

        //Rotation pour toujours faire face au centre
        goProj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_fCurrentSpriteAngle));

        Transform tTest = goProj.transform.Find(StaticResources.TRANSFORM_QUEUE);
        tTest.rotation = Quaternion.Euler(new Vector3(0, 0, m_fCurrentSpriteAngle - 90));

        //Ajout d'une force dans la direction du regard de l'objet, donc vers le centre
        goProj.GetComponent<Rigidbody2D>().AddForce(goProj.transform.position.normalized * -500);

        tPlayerCharge.localScale = new Vector3(0.12f, 0.12f, 1);

        m_gameCtrl.PlayerShotCharge();
    }

    public bool HasPlayerCharge()
    {
        return m_hasPlayerCharge;
    }
    #endregion

    #region Evenements

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_gameCtrl.m_IsGameEnded)
            return;

        //Photon : gain de points
        if (collision.CompareTag(StaticResources.TAG_PHOTON))
        {
            if (m_nChargeCounter < m_gameCtrl.GetMaxCharges())
                m_nChargeCounter++;

            PhotonController photonCtrl = collision.gameObject.GetComponent<PhotonController>();

            if (!photonCtrl.IsAlreadyCollected())
            {
                photonCtrl.StartCollectAnimation();

                UpdateCharges();

                int nScoreAdded = GameController.SCORE_PHOTON;

                if (!photonCtrl.m_bPhotonStopped)
                    nScoreAdded = GameController.SCORE_PHOTON * GameController.SCORE_COEF;

                m_gameCtrl.AddScore(nScoreAdded);
                SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_PHOTON_DISAPPEAR);

                if (photonCtrl.m_photonColor == PhotonController.PhotonColor.BLUE)
                {
                    m_animator.Play(StaticResources.ANIMATION_GET_PHOTON_BLUE);
                }
                else if (photonCtrl.m_photonColor == PhotonController.PhotonColor.RED)
                {
                    m_animator.Play(StaticResources.ANIMATION_GET_PHOTON_RED);
                }
            }
        }

        bool bPlayerHasDied = false;

        //Electron
        if (collision.CompareTag(StaticResources.TAG_ELECTRON) && !m_isInGodMode && !m_IsInRecoverymode && !m_gameCtrl.m_IsGameEnded)
        {
            if (m_nSanteCounter > 0)
                m_nSanteCounter--;

            Destroy(collision.gameObject);

            UpdateSante();

            if (m_nSanteCounter == 0)
            {
                bPlayerHasDied = true;
                m_gameCtrl.m_IsGameEnded = true;

                EndScreenAndRestart();
            }
            else
            {
                SetRecoveryMode(true);
            }
        }

        //Barriere ou Laser : même fonction
        if ((collision.CompareTag(StaticResources.TAG_BARRIERE) || collision.CompareTag(StaticResources.TAG_LASER)) && !m_isInGodMode && !m_IsInRecoverymode && !m_gameCtrl.m_IsGameEnded)
        {
            if (m_nSanteCounter > 0)
                m_nSanteCounter--;

            UpdateSante();

            if (m_nSanteCounter == 0)
            {
                bPlayerHasDied = true;
                m_gameCtrl.m_IsGameEnded = true;

                EndScreenAndRestart();
            }
            else
            {
                SetRecoveryMode(true);
            }
        }

        if (bPlayerHasDied)
        {
            SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_PLAYER_DIE);
            m_bIsDead = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticResources.TAG_BARRIERE) && !m_isInGodMode && !m_IsInRecoverymode)
        {
            if (m_nSanteCounter > 0)
                m_nSanteCounter--;

            UpdateSante();

            if (m_nSanteCounter == 0)
            {
                m_gameCtrl.m_IsGameEnded = true;

                EndScreenAndRestart();
            }
            else
            {
                SetRecoveryMode(true);
            }
        }
    }

    #endregion

    #region Coroutines

    //Aniamtion de recovery
    private IEnumerator RecoveryAnimation()
    {
        m_animator.Play(StaticResources.ANIMATION_RECOVERY_MODE);

        yield return new WaitForSeconds(1f);

        SetRecoveryMode(false);
    }

    //Animation de disparition
    private IEnumerator WaitAndDisappear(System.Func<bool> callbackFunction)
    {
        m_animator.Play(StaticResources.ANIMATION_DISPARITION);

        yield return new WaitForSeconds(1f);

        m_gameCtrl.EndScreenFade(callbackFunction);
    }

    //Animation de God Mode
    private IEnumerator GodModeAnimation()
    {
        yield return new WaitForSeconds(2);

        m_isInGodMode = false;

        tCerclePlayerGodMode.gameObject.GetComponent<Animator>().Play(StaticResources.ANIMATION_GOD_MODE_HAS_BEEN_HIT);
    }

    private IEnumerator WaitAndAllowChangingCouche()
    {
        yield return new WaitForSeconds(0.5f);

        m_bIsChangingCouche = false;
    }

    #endregion

    #region Audio

    private void AudioPlayerCharge()
    {
        m_audioSource.Stop();
        m_audioSource.loop = false;
        m_audioSource.clip = m_audioCharge;

        SoundManager.PlayAudioSource(m_audioSource);
    }

    private void AudioPlayerMaxCharge()
    {
        if (!m_bAudioMaxChargeIsPlaying)
        {
            m_audioSource.Stop();
            m_audioSource.loop = true;
            m_audioSource.clip = m_audioMaxCharge;

            SoundManager.PlayAudioSource(m_audioSource);

            m_bAudioMaxChargeIsPlaying = true;
        }
    }

    private void AudioPlayerShotPlayerCharge()
    {
        m_audioSource.Stop();
        m_audioSource.loop = false;
        m_audioSource.clip = m_audioShotPlayerCharge;

        SoundManager.PlayAudioSource(m_audioSource);

        m_bAudioMaxChargeIsPlaying = false;
    }

    private void AudioChangeCouche()
    {
        SoundManager.PlaySound(SoundManager.AUDIO.AUDIO_CHANGE_COUCHE);
    }

    private void StopAudioSource()
    {
        m_audioSource.Stop();
        m_bAudioMaxChargeIsPlaying = false;
    }

    #endregion
}
