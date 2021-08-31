using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanteUIController : MonoBehaviour
{
    public void DisableGameObject()
    {
        Transform tSantePleine = transform.Find(StaticResources.TRANSFORM_SANTE_PLEINE);
        Transform tSanteContour = transform.Find(StaticResources.TRANSFORM_SANTE_CONTOUR);

        tSantePleine.gameObject.SetActive(false);
        tSanteContour.gameObject.SetActive(false);
    }

    public void EnableGameObject()
    {
        Transform tSantePleine = transform.Find(StaticResources.TRANSFORM_SANTE_PLEINE);
        Transform tSanteContour = transform.Find(StaticResources.TRANSFORM_SANTE_CONTOUR);

        tSantePleine.gameObject.SetActive(true);
        tSanteContour.gameObject.SetActive(true);
    }
}
