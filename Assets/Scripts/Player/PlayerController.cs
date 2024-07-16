using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;

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
        else if (other.CompareTag("StopObsticle"))
        {
            StartCoroutine(IEStopObsticle());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ShowHint"))
        {
            MazeGenerator.Instance.ShowHint(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            Destroy(other.gameObject);
        }
    }

    IEnumerator IEStopObsticle()
    {
        movement.StopMovement();
        yield return new WaitForSeconds(3.0f);
        movement.StartMovement();
    }
}
