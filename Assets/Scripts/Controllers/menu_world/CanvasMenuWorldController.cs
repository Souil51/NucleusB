using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenuWorldController : MonoBehaviour
{
    private Transform m_levelLabel;

    // Start is called before the first frame update
    void Start()
    {
        m_levelLabel = transform.Find(StaticResources.TRANSFORM_LABEL_LEVEL);
    }

    // Update is called once per frame
    
    public void DisableLabel()
    {
        m_levelLabel.gameObject.SetActive(false);
    }

    public void EnabledLabel()
    {
        m_levelLabel.gameObject.SetActive(true);
    }

    public void LevelLabelDisappear()
    {
        m_levelLabel.GetComponent<Animator>().Play(StaticResources.ANIMATION_LEVEL_LABEL_DISAPPEAR);
    }

    public void LevelLabelAppear()
    {
        m_levelLabel.GetComponent<Animator>().Play(StaticResources.ANIMATION_LEVEL_LABEL_APPEAR);
    }

    public void ChangeLevelText(string szTexte)
    {
        m_levelLabel.GetComponent<Text>().text = szTexte;
    }

    public void DisplayNotUnlocked()
    {
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_LOCKED).gameObject.SetActive(true);
    }

    public void HideNotUnlocked()
    {
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_LOCKED).gameObject.SetActive(false);
    }
}
