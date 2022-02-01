using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public virtual void Initialize()
    {

    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnEnterTrigger(Collider col) { }
    public virtual void OnStayTrigger(Collider col) { }
    public virtual void OnExitTrigger(Collider col) { }
    public virtual void OnEnterCollision(Collision col) { }
    public virtual void OnStayCollision(Collision col) { }
    public virtual void OnExitCollision(Collision col) { }
}
