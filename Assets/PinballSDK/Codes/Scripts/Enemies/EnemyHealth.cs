using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int MaxHP;
    int HP;

    private void Start()
    {
        HP = MaxHP;
    }

    private void OnEnable()
    {
        //Reset HP upon being respawned
        HP = MaxHP;
    }
    public void DealDamage()
    {
        HP--;
        if (HP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
