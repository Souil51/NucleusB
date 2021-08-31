using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMenuLevelController : MonoBehaviour
{
    private Transform m_levelLabel;
    private Transform m_WorldLabel;

    // Start is called before the first frame update
    void Start()
    {
        m_levelLabel = transform.Find(StaticResources.TRANSFORM_LABEL_LEVEL);
        m_WorldLabel = transform.Find(StaticResources.TRANSFORM_LABEL_WORLD);
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

    public void ChangeWorldText(string szTexte)
    {
        if(m_WorldLabel == null)
            m_WorldLabel = transform.Find(StaticResources.TRANSFORM_LABEL_WORLD);

        m_WorldLabel.GetComponent<Text>().text = szTexte;
    }

    public void DisplayCompleted()
    {
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_COMPLETED).GetComponent<Text>().text = "Completed";
    }

    public void DisplayScore(int nScore)
    {
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_SCORE).gameObject.SetActive(true);
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_SCORE).GetComponent<Text>().text = "Score : " + nScore;
    }

    public void DisplayNotCompleted()
    {
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_COMPLETED).GetComponent<Text>().text = "Not completed";
        m_levelLabel.Find(StaticResources.TRANSFORM_LABEL_SCORE).gameObject.SetActive(false);
    }

    public Transform GetHidingCircle()
    {
        return transform.Find(StaticResources.TRANSFORM_CIRCLE_HIDING);
    }

    public void EnableHidingCircle()
    {
        transform.Find(StaticResources.TRANSFORM_CIRCLE_HIDING).gameObject.SetActive(true);
    }

    public RectTransform GetHidingCircleRectTransform()
    {
        return GetHidingCircle().GetComponent<RectTransform>();
    }

    public void DisableHidingCircleAnimator()
    {
        GetHidingCircle().GetComponent<Animator>().enabled = false;
    }

    public void SetHidingCircleSizeWithCurrentAnchors(float fWidth, float fHeight)
    {
        RectTransform rectTransform = GetHidingCircleRectTransform();

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fHeight);
        rectTransform.ForceUpdateRectTransforms();
    }

    public Vector3 GetHiddingCircleLocalScale()
    {
        return GetHidingCircle().localScale;
    }

    public void SetHiddingCircleLocalScale(Vector3 vNew)
    {
        GetHidingCircle().localScale = vNew;
    }
}
