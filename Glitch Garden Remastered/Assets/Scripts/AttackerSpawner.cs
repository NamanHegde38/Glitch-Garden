using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackerSpawner : MonoBehaviour {
    
    private float _startSpawnDelay = 30;
    private float _endSpawnDelay = 7;
    private float _deviationPercent = 0.75f;
    
    private Attacker[] _attackerPrefabArray;

    private GameTimer _gameTimer;
    private float _levelTime;
    private int _timePercent;
    
    private bool _spawn = false;
    private float _spawnDelay;
    private float _delayDeviation;

    private LevelController _levelController;

    public void SetStartSpawnDelay(float startSpawnDelay) {
        _startSpawnDelay = startSpawnDelay;
    }

    public void SetEndSpawnDelay(float endSpawnDelay) {
        _endSpawnDelay = endSpawnDelay;
    }
    
    public void SetDeviationPercent(float deviationPercent) {
        _deviationPercent = deviationPercent;
    }

    public void SetAttackerArray(Attacker[] attackerPrefabArray) {
        _attackerPrefabArray = attackerPrefabArray;
    }

    public void StopSpawning() {
        _spawn = false;
    }
    
    private void SpawnAttacker() {
        var attackerIndex = Random.Range(0, _attackerPrefabArray.Length);
        Spawn(_attackerPrefabArray[attackerIndex]);
    }

    private void Start() {
        _levelController = FindObjectOfType<LevelController>().GetComponent<LevelController>();
        _levelController.OnLevelStart += StartGame;
    }

    private void StartGame(object sender, EventArgs e) {
        _spawn = true;
        _gameTimer = FindObjectOfType<GameTimer>();
        StartCoroutine(WhileSpawn());
    }

    private IEnumerator WhileSpawn() {
        while (_spawn) {
            _levelTime = _gameTimer.GetGameTime();
            _spawnDelay = Mathf.Lerp(_startSpawnDelay, _endSpawnDelay, _levelTime);
            _delayDeviation = _spawnDelay * _deviationPercent;
            yield return new WaitForSeconds(Random.Range(_spawnDelay - _delayDeviation, _spawnDelay + _delayDeviation));
            SpawnAttacker();
        }
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
