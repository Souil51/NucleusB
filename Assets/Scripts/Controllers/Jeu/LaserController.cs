using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    private float m_fAngleRotation;
    private float m_fDuration;
    private float m_fCurrentAngle;
    private float m_RotationAngleUpdate;


    // Update is called once per frame
    void Update()
    {
        //L'angle de rotation est d�fini pour 1 seconde
        //Si il est = 10, le laser tournera de 10� en 1 seconde
        //Donc la rotation de chaque update d�pend du temps qu'� dur� la derni�re frame
        m_RotationAngleUpdate = m_fAngleRotation * Time.deltaTime;

        m_fCurrentAngle += m_RotationAngleUpdate;
        m_fCurrentAngle %= 360;

        UpdateRotation();
    }

    public void InitLaser(float fStartingAngle, float fAngleRotation, float fDuration)
    {
        m_fAngleRotation = fAngleRotation;
        m_fCurrentAngle = (fStartingAngle - 90) % 360;
        m_fDuration = fDuration;

        UpdateRotation();

        StartCoroutine(WaitAndDestroy());
    }

    //Met � jour la rotation Z du laser
    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, m_fCurrentAngle));
    }

    //D�truit certains objets au contact (PlayerCharge)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticResources.TAG_PLAYER_CHARGE))
        {
            Destroy(collision.gameObject);
        }
    }

    //Coroutine pour d�truire l'objet apr�s X temps
    public IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(m_fDuration);

        Destroy(this.gameObject);
    }
}
