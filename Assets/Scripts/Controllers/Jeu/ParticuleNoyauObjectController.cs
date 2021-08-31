using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticuleNoyauObjectController : MonoBehaviour
{
    // Start is called before the first frame update
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
