using System;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject spotlight;
    public int noiseCount;

    private void Start()
    {
        AudioManager.Instance.Play(noiseCount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // spotlight.SetActive(true);
            spotlight.GetComponent<Animator>().SetBool("lightOn", true);
            AudioManager.Instance.Stop(noiseCount);
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
