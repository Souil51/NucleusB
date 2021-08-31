using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CercleNoyauController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndDisappear());
    }

    private IEnumerator WaitAndDisappear()
    {
        yield return new WaitForSeconds(0.55f);

        Destroy(this.gameObject);
    }
}
