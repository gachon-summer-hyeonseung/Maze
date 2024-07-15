using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlusTime"))
        {
            GameManager.Instance.AddTime();
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("PlusScore"))
        {
            GameManager.Instance.AddScore();
            Destroy(other.gameObject);
        }
    }
}
