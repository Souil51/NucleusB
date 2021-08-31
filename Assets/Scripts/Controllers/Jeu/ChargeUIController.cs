using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeUIController : MonoBehaviour
{
    public void DisableGameObject()
    {
        Transform tChargePleine = transform.Find(StaticResources.TRANSFORM_CHARGE_PLEINE);
        Transform tChargeContour = transform.Find(StaticResources.TRANSFORM_CHARGE_CONTOUR);

        tChargePleine.gameObject.SetActive(false);
        tChargeContour.gameObject.SetActive(false);
    }

    public void EnableGameObject()
    {
        Transform tChargePleine = transform.Find(StaticResources.TRANSFORM_CHARGE_PLEINE);
        Transform tChargeContour = transform.Find(StaticResources.TRANSFORM_CHARGE_CONTOUR);

        tChargePleine.gameObject.SetActive(true);
        tChargeContour.gameObject.SetActive(true);
    }
}
