using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackDamage : MonoBehaviour
{
    public int damage = 5;

    [SerializeField] private GameObject _manager;
    private Difficulty_Manager _self;
    // Start is called before the first frame update
    void Start()
    {
        if (_manager == null)
        {
            _manager = GameObject.FindGameObjectWithTag("Difficulty_Manager");
        }

        _self = _manager.GetComponent<Difficulty_Manager>();
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
            Debug.Log("Hit a monster");
            AIScript foeHP = other.GetComponentInParent<AIScript>();
            if (foeHP == null)
            {
                foeHP = other.GetComponent<AIScript>();
            }

            if (foeHP != null)
            {
                float dam = damage * _self.incomingDamageModifer;
                int realDamage = (int) dam;
                foeHP.monsterHP -= realDamage;
                Debug.Log(realDamage);
            }
            else
            {
                Debug.Log("Fuck");
            }
        }
    }
}
