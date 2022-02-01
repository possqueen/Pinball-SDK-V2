using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StageTimer : MonoBehaviour
{
    public bool Active;
    [SerializeField] TextMeshProUGUI displayTimer;
    float Min;
    float Sec;
    float Cent;
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Active)
        {
            Cent += Time.deltaTime * 100;
            if (Cent >= 100f)
            {
                Sec++;
                Cent = 0;
            }
            if (Sec >= 60f)
            {
                Min++;
                Sec = 0;
            }
        }

        displayTimer.text = string.Format("{00:00}:{01:00}.{02:00}", (int)Min, (int)Sec, (int)Cent);
    }
}
