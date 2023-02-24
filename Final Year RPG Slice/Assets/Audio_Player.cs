using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Player : MonoBehaviour
{

    [SerializeField] private AudioClip[] swordSounds;
    private int _index = 0;
    [SerializeField] private AudioSource _sword;

    private void Start()
    {
        _index = Random.Range(0, swordSounds.Length);
    }

    public void PlaySound()
    {
        _index = Random.Range(0, swordSounds.Length);
        
        _sword.PlayOneShot(swordSounds[_index]);
    }
}
