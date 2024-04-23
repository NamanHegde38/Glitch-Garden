using System.Collections;
using UnityEngine;

public class IceProjectile : MonoBehaviour {

    [SerializeField] private float freezeTime = 10f;
    [SerializeField] private float attackTime = 3f;
    [SerializeField] private Vector2 attackAreaSize = new (0.5f, 8);
    [SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private LayerMask defenderLayer;
    private Vector2 _transformPosition;
    
    private IEnumerator Start() {
        yield return new WaitForSeconds(attackDelay);
        
        _transformPosition = transform.position;
        var pointA = new Vector2(_transformPosition.x - (attackAreaSize.x / 2), _transformPosition.y + attackAreaSize.y);
        var pointB = new Vector2(_transformPosition.x + (attackAreaSize.x / 2), _transformPosition.y);
        
        var defendersToAttack = Physics2D.OverlapAreaAll(pointA, pointB, defenderLayer);
        Debug.Log(defendersToAttack.Length);
        foreach (var defender in defendersToAttack) {
            defender.GetComponent<Health>().Freeze(freezeTime);
        }
        
        yield return new WaitForSeconds(attackTime);
        
        transform.GetChild(0).GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(gameObject, 2f);
    }
}
