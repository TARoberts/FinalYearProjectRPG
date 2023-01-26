using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFire : MonoBehaviour
{

    [SerializeField] private GameObject red, yellow, orange;

    // Update is called once per frame
    private void OnEnable()
    {
        red.GetComponent<ParticleSystem>().Play();
        yellow.GetComponent<ParticleSystem>().Play();
        orange.GetComponent<ParticleSystem>().Play();
    }

    private void OnDisable()
    {
        red.GetComponent<ParticleSystem>().Stop();
        yellow.GetComponent<ParticleSystem>().Stop();
        orange.GetComponent<ParticleSystem>().Stop();
    }
}
