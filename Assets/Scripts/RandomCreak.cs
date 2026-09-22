using UnityEngine;
using System.Collections;

public class RandomCreak : MonoBehaviour
{
    public AudioSource audioSource;

    public float minimumDelay = 8f;
    public float maximumDelay = 20f;

    void Start()
    {
        StartCoroutine(CreakLoop());
    }

    IEnumerator CreakLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minimumDelay, maximumDelay)
            );

            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}