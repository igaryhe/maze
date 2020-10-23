using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject spotlight;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // spotlight.SetActive(true);
            spotlight.GetComponent<Animator>().SetBool("lightOn", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { 
            // light.SetActive(false);
        }
    }
}
