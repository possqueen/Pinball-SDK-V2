using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RingCounter : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] TextMeshProUGUI counter;

    private void Update()
    {
        counter.text = string.Format("{00:000}", playerHealth.RingAmount);
    }
}
