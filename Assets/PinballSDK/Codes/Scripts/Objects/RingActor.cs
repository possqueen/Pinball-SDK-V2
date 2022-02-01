using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingActor : MonoBehaviour
{
    public int Value;
    public bool IsLost;
    public float Lifetime;
    public float CollectibleLifetime; //How long into the lifetime does the lost ring become collectible
    public float flickerStartTime;
    public int FlickerRate;
    [SerializeField] GameObject ringMesh;
    public AnimationCurve FlickerRateOverLifetime;
    [HideInInspector] public Vector3 SpawnPos;
    [HideInInspector] public Quaternion SpawnRot;
    float t;
    float frameT;
    bool meshActive;
    [HideInInspector] public bool collectible;
    private void Start()
    {
        //Set initial spawn stuff so we can easily reset any rings affected by electric shields
        SpawnPos = transform.position;
        SpawnRot = transform.rotation;
    }

    private void OnEnable()
    {
        t = 0;
        frameT = 0;
        meshActive = true;
        collectible = !IsLost;
    }

    private void Update()
    {
        if (IsLost)
        {
            t += Time.deltaTime;
            if (t >= CollectibleLifetime) collectible = true;
            if (t >= Lifetime)
            {
                gameObject.SetActive(false);
            }
            if (t >= flickerStartTime)
            {
                float flickerRate = (float)FlickerRate * FlickerRateOverLifetime.Evaluate(t / Lifetime);
                frameT++;
                if (frameT >= flickerRate)
                {
                    meshActive = !meshActive;
                    frameT = 0;
                }
            }
            ringMesh.SetActive(meshActive);
        }
    }

    

    public void ResetRing()
    {
        transform.position = SpawnPos;
        transform.rotation = SpawnRot;
    }
}
