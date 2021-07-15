using System.Collections;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour {

    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 5f;
    [SerializeField] private Attacker[] attackerPrefabArray;
    
    private bool _spawn = true;

    private IEnumerator Start() {
        while (_spawn) {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
            SpawnAttacker();
        }
    }

    public void StopSpawning() {
        _spawn = false;
    }
    
    private void SpawnAttacker() {
        var attackerIndex = Random.Range(0, attackerPrefabArray.Length);
        Spawn(attackerPrefabArray[attackerIndex]);
    }

    private void Spawn(Attacker myAttacker) {
        var newAttacker = Instantiate(myAttacker, transform.position, transform.rotation);
        newAttacker.transform.parent = transform;
    }
}
