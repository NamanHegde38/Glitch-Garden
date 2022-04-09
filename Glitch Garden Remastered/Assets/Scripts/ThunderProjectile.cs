using System.Collections;
using UnityEngine;

public class ThunderProjectile : MonoBehaviour {

    [SerializeField] private float strikeDelay = 0.05f;
    [SerializeField] private float damageArea = 0.5f;
    [SerializeField] private LayerMask defenderLayer;
    
    private int _damage;

    public void SetDamage(int damage) {
        _damage = damage;
    }
    
    private IEnumerator Start() {
        Destroy(gameObject, 2f);
        yield return new WaitForSeconds(strikeDelay);
        var defendersToDamage = Physics2D.OverlapCircleAll(transform.position, damageArea, defenderLayer);
        if (defendersToDamage.Length <= 0) yield break;
        
        foreach (var defender in defendersToDamage) {
            defender.GetComponent<Health>().DealDamage(_damage);
        }
    }
}
