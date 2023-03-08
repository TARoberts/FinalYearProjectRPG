using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    public PlayerStats _player;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
         if (_player == null)
        {
            _player = FindObjectOfType<PlayerStats>();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Player")
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
            _player.playerHealthValue -= numCollisionEvents / 10;
        }

    }
}
