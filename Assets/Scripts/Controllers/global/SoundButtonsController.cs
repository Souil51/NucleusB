using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButtonsController : MonoBehaviour
{
    public GameController m_goGameCtrl;

    // Start is called before the first frame update
    void Start()
    {
        UpdateButtons();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(StaticResources.KEY_MUTE_MUSIC))
        {
            if (DataManagerController.instance.m_bMusicSound)
            {
                MuteMusic();
            }
            else
            {
                UnmuteMusic();
            }
        }

        if (Input.GetKeyDown(StaticResources.KEY_MUTE_FX))
        {
            if (DataManagerController.instance.m_bFXSound)
            {
                MuteFX();
            }
            else
            {
                UnmuteFX();
            }
        }
    }

    public void MuteMusic()
    {
        DataManagerController.instance.m_bMusicSound = false;
        transform.Find(StaticResources.TRANSFORM_PANEL_MUSIC).Find(StaticResources.TRANSFORM_LABEL_SOUND_ON).GetComponent<Text>().text = "M to unmute music";
        transform.Find(StaticResources.TRANSFORM_PANEL_MUSIC).Find(StaticResources.TRANSFORM_SOUND_ON).gameObject.SetActive(false);
        transform.Find(StaticResources.TRANSFORM_PANEL_MUSIC).Find(StaticResources.TRANSFORM_SOUND_OFF).gameObject.SetActive(true);

        ThemeManagerController.instance.MuteTheme();

        if (m_goGameCtrl != null)
            m_goGameCtrl.MuteBackgroundMusic();
    }

    public void UnmuteMusic()
    {
        DataManagerController.instance.m_bMusicSound = true;
        transform.Find(StaticResources.TRANSFORM_PANEL_MUSIC).Find(StaticResources.TRANSFORM_LABEL_SOUND_ON).GetComponent<Text>().text = "M to mute music";
        transform.Find(StaticResources.TRANSFORM_PANEL_MUSIC).Find(StaticResources.TRANSFORM_SOUND_ON).gameObject.SetActive(true);
        transform.Find(StaticResources.TRANSFORM_PANEL_MUSIC).Find(StaticResources.TRANSFORM_SOUND_OFF).gameObject.SetActive(false);

        if (m_goGameCtrl != null)
        {
            m_goGameCtrl.UnmuteBackgroundMusic();
        }
        else
        {
            ThemeManagerController.instance.UnmuteTheme();
        }
    }

    public void MuteFX()
    {
        DataManagerController.instance.m_bFXSound = false;
        transform.Find(StaticResources.TRANSFORM_PANEL_FX).Find(StaticResources.TRANSFORM_LABEL_SOUND_ON).GetComponent<Text>().text = "L to unmute FX";
        transform.Find(StaticResources.TRANSFORM_PANEL_FX).Find(StaticResources.TRANSFORM_SOUND_ON).gameObject.SetActive(false);
        transform.Find(StaticResources.TRANSFORM_PANEL_FX).Find(StaticResources.TRANSFORM_SOUND_OFF).gameObject.SetActive(true);
    }

    public void UnmuteFX()
    {
        DataManagerController.instance.m_bFXSound = true;
        transform.Find(StaticResources.TRANSFORM_PANEL_FX).Find(StaticResources.TRANSFORM_LABEL_SOUND_ON).GetComponent<Text>().text = "L to mute FX";
        transform.Find(StaticResources.TRANSFORM_PANEL_FX).Find(StaticResources.TRANSFORM_SOUND_ON).gameObject.SetActive(true);
        transform.Find(StaticResources.TRANSFORM_PANEL_FX).Find(StaticResources.TRANSFORM_SOUND_OFF).gameObject.SetActive(false);
    }

    public void UpdateButtons()
    {
        if (DataManagerController.instance.m_bMusicSound)
            UnmuteMusic();
        else
            MuteMusic();

        if (DataManagerController.instance.m_bFXSound)
            UnmuteFX();
        else
            MuteFX();
    }
}
