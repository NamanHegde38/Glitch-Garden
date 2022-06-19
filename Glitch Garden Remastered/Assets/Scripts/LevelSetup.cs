using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelSetup : MonoBehaviour {

    [BoxGroup("Defenders")] [SerializeField]
    [AssetSelector(Paths = "Assets/Prefabs/Defenders")]
    private GameObject
        trophyPrefab,
        doubleTrophyPrefab,
        cactusPrefab,
        scarecrowPrefab,
        statuePrefab,
        gravestonePrefab,
        vendorPrefab,
        gnomePrefab;

    [BoxGroup("Level")]
    [SerializeField] private Level level;

    [SerializeField] private bool isBossLevel;
    [SerializeField] private bool isSurvivalLevel;

    private GameObject _levelTimer;
    private GameObject _bossHealth;
    private GameObject _starText;
    private GameObject _defenderButtons;
    private GameObject[] _attackerSpawners;
    private GameObject _gameCanvas;
    private GameObject _postProcessing;
    private GameObject _cinemachineCamera;

    private LevelController _levelController;

    private void Awake() {
        SetComponents();
        SetLevel();
    }

    private void Start() {
        _levelController.OnLevelStart += SetLevelOnStart;
    }

    private void SetLevelOnStart(object sender, EventArgs e) {

        if (!isBossLevel && !isSurvivalLevel) {
            _levelTimer = GameObject.FindWithTag("Level Timer");
            _levelTimer.GetComponent<GameTimer>().SetGameTime(level.GetLevelTime());
        }
        _starText = GameObject.FindWithTag("Star Text");
        _defenderButtons = GameObject.FindWithTag("Defender Buttons");
        
        

        _starText.GetComponent<StarDisplay>().SetStars(level.GetStars());
        _starText.GetComponent<StarDisplay>().SetStarsOverTime(level.GetStarsOverTime());
        
        _defenderButtons.GetComponent<DefenderToolbar>().SetDefenders(level.GetDefenders());

        for (var i = 0; i < 9; i++) {
            for (var a = 0; a < 5; a++) {
                if (level.GetDefenderLayout()[i, a] != DefenderType.None) {
                    SpawnDefender(SelectDefenderPrefab(level.GetDefenderLayout()[i, a]), new Vector2(i+1, 5-a));
                }
            }
        }
    }
    
    private void SetLevel() {

        foreach (var attackerSpawner in _attackerSpawners) {
            attackerSpawner.GetComponent<AttackerSpawner>().SetStartSpawnDelay(level.GetMaxSpawnDelay());
            attackerSpawner.GetComponent<AttackerSpawner>().SetEndSpawnDelay(level.GetMinSpawnDelay());
            attackerSpawner.GetComponent<AttackerSpawner>().SetDeviationPercent(level.GetDeviationPercent());
            attackerSpawner.GetComponent<AttackerSpawner>().SetAttackerArray(level.GetAttackers());
            attackerSpawner.GetComponent<AttackerSpawner>().SetShinyArray(level.GetShinyAttackers());
        }

        if (_gameCanvas != level.GetGameCanvas()) {
            Destroy(_gameCanvas);
            Instantiate(level.GetGameCanvas());
        }

        if (_postProcessing != level.GetPostProcessing()) {
            Destroy(_postProcessing);
            Instantiate(level.GetPostProcessing());
        }

        if (_cinemachineCamera != level.GetCamera()) {
            Destroy(_cinemachineCamera);
            Instantiate(level.GetCamera()); 
        }
    }

    private GameObject SelectDefenderPrefab(DefenderType defenderType) {
        GameObject defenderPrefab;
        
        switch (defenderType) {
            case DefenderType.Trophy:
                defenderPrefab = trophyPrefab;
                break;
            case DefenderType.DoubleTrophy:
                defenderPrefab = doubleTrophyPrefab;
                break;
            case DefenderType.Cactus:
                defenderPrefab = cactusPrefab;
                break;
            case DefenderType.Scarecrow:
                defenderPrefab = scarecrowPrefab;
                break;
            case DefenderType.Statue:
                defenderPrefab = statuePrefab;
                break;
            case DefenderType.Gravestone:
                defenderPrefab = gravestonePrefab;
                break;
            case DefenderType.Vendor:
                defenderPrefab = vendorPrefab;
                break;
            case DefenderType.Gnome:
                defenderPrefab = gnomePrefab;
                break;
            default:
                defenderPrefab = trophyPrefab;
                break;
        }

        return defenderPrefab;
    }
    
    private void SpawnDefender(GameObject defender, Vector2 rawPosition) {
        var roundedPos = SnapToGrid(rawPosition);
        var newDefender = Instantiate(defender, roundedPos, Quaternion.identity);
        newDefender.transform.parent = GameObject.Find("Defenders").transform;
    }
    
    private static Vector2 SnapToGrid(Vector2 rawWorldPos) {
        var newX = Mathf.RoundToInt(rawWorldPos.x);
        var newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector2(newX, newY);
    }
    
    private void SetComponents() {
        _attackerSpawners = GameObject.FindGameObjectsWithTag("Attacker Spawner");
        _gameCanvas = GameObject.FindWithTag("Game Canvas");
        _cinemachineCamera = GameObject.FindWithTag("Cinemachine");
        _postProcessing = GameObject.FindWithTag("Post Processing");
        _levelController = FindObjectOfType<LevelController>().GetComponent<LevelController>();
    }
}
