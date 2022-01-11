using System.Collections;
using UnityEngine;

public class AttackerSpawner : MonoBehaviour {
    
    [SerializeField] private float startSpawnDelay = 30;
    [SerializeField] private float endSpawnDelay = 7;
    [SerializeField] private float deviationPercent = 0.5f;
    
    [SerializeField] private Attacker[] attackerPrefabArray;

    private GameTimer _gameTimer;
    private float _levelTime;
    private int _timePercent;
    
    private bool _spawn = true;
    private float _spawnDelay;
    private float _delayDeviation;

    private IEnumerator Start() {
        _gameTimer = FindObjectOfType<GameTimer>();

        while (_spawn) {
            _levelTime = _gameTimer.GetGameTime();
            _spawnDelay = Mathf.Lerp(startSpawnDelay, endSpawnDelay, _levelTime);
            _delayDeviation = _spawnDelay * deviationPercent;
            yield return new WaitForSeconds(Random.Range(_spawnDelay - _delayDeviation, _spawnDelay + _delayDeviation));
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
        var spawnPos = new Vector2(transform.position.x + 1.5f, transform.position.y);
        if (myAttacker.GetIfHasSpawnAnimation()) {
            spawnPos = transform.position;
        }
        var newAttacker = Instantiate(myAttacker, spawnPos, transform.rotation);
        newAttacker.transform.parent = transform;
    }
}
