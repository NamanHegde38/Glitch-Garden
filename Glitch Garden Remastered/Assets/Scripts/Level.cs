using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "Level 00", menuName = "Level")]
public class Level : SerializedScriptableObject {
    
    [BoxGroup("Settings")] [SerializeField]
    [PropertyRange(160, 240)]
    private int levelTime = 160;
    
    [BoxGroup("Settings")] [SerializeField]
    [PropertyRange(0, 250)]
    private int stars = 100;
    
    [BoxGroup("Settings")] [SerializeField]
    [PropertyRange(0, 100)]
    private int starsOverTime = 50;
    
    [BoxGroup("Characters")] [SerializeField]
    private DefenderType[] defenders;
    
    [BoxGroup("Characters")] [SerializeField]
    [AssetList(Path = "Prefabs/Attackers")]
    private Attacker[] attackers;
    
    [BoxGroup("Spawners")] [SerializeField]
    [MinMaxSlider(3, 30)]
    private Vector2 spawnDelay = new Vector2(3, 30);

    [BoxGroup("Spawners")] [SerializeField]
    [PropertyRange(0, 100)]
    private int deviationPercent;
    
    [BoxGroup("Level")] [SerializeField]
    [AssetsOnly]
    private GameObject gameCanvas;
    
    [BoxGroup("Level")] [SerializeField]
    [AssetsOnly]
    private GameObject postProcessing;
    
    [BoxGroup("Level")] [SerializeField]
    [AssetsOnly]
    private GameObject camera;

    [BoxGroup("Defender Layout")] [OdinSerialize]
    [TableMatrix(SquareCells = true)]
    private DefenderType[,] _defenderLayout = new DefenderType[9,5];

    public int GetLevelTime() {
        return levelTime;
    }
    
    public int GetStars() {
        return stars;
    }
    
    public int GetStarsOverTime() {
        return starsOverTime;
    }
    
    public DefenderType[] GetDefenders() {
        return defenders;
    }
    
    public Attacker[] GetAttackers() {
        return attackers;
    }

    public float GetMinSpawnDelay() {
        return spawnDelay.x;
    }

    public float GetMaxSpawnDelay() {
        return spawnDelay.y;
    }
    
    public float GetDeviationPercent() {
        return deviationPercent / 100;
    }

    public GameObject GetGameCanvas() {
        return gameCanvas;
    }
    
    public GameObject GetPostProcessing() {
        return postProcessing;
    }

    public GameObject GetCamera() {
        return camera;
    }
    
    public DefenderType[,] GetDefenderLayout() {
        return _defenderLayout;
    }
}
