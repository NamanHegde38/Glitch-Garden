using UnityEngine;

public class DefenderSpawner : MonoBehaviour {

    [SerializeField] private GameObject trophyPreview;
    [SerializeField] private GameObject doubleTrophyPreview;
    [SerializeField] private GameObject cactusPreview;
    [SerializeField] private GameObject scarecrowPreview;
    [SerializeField] private GameObject statuePreview;
    [SerializeField] private GameObject gravestonePreview;
    [SerializeField] private GameObject vendorPreview;
    [SerializeField] private GameObject gnomePreview;

    [SerializeField] private Transform previewMask;
    [SerializeField] private GameObject previewBands;

    [SerializeField] private float lerpSpeed;

    private Defender _defender;
    private GameObject _defenderParent;
    private StarDisplay _starDisplay;
    private GameObject _spawnedPreview;
    private GameObject _spawnedBands;
    private Camera _mainCam;
    private DefenderType _defenderType;

    private const string Defenders = "Defenders";
    

    private void Start() {
        _starDisplay = FindObjectOfType<StarDisplay>();
        _mainCam = Camera.main;
        CreateDefenderParent();
    }

    private void CreateDefenderParent() {
        _defenderParent = GameObject.Find(Defenders);
        if (!_defenderParent) {
            _defenderParent = new GameObject(Defenders);
        }
    }

    private void OnMouseEnter() {
        if (!_defender) return;

        _defenderType = _defender.GetDefenderType();
        _spawnedPreview = Instantiate(GetDefenderPreview(_defenderType), GetSquareClicked(), Quaternion.identity);
        _spawnedBands = Instantiate(previewBands, GetSquareClicked(), Quaternion.identity, previewMask);
    }

    private void OnMouseOver() {
        if (!_defender) return;
        
        
        _spawnedPreview.transform.position = Vector2.Lerp(_spawnedPreview.transform.position, GetSquareClicked(), lerpSpeed * Time.deltaTime);
        _spawnedBands.transform.position = Vector2.Lerp(_spawnedBands.transform.position, GetSquareClicked(), lerpSpeed * Time.deltaTime);
    }
    
    private void OnMouseDown() {
        if (!_defender) return;
        AttemptToPlaceDefenderAt(GetSquareClicked());
    }

    private void OnMouseExit() {
        if (_spawnedPreview) {
            Destroy(_spawnedPreview);
            Destroy(_spawnedBands);
        }
    }

    public void SetSelectedDefender(Defender defenderToSelect) {
        _defender = defenderToSelect;
    }

    private void AttemptToPlaceDefenderAt(Vector2 gridPos) {
        var defenderCost = _defender.GetStarCost();
        
        if (!_starDisplay.HaveEnoughStars(defenderCost)) return;
        SpawnDefender(gridPos);
        _starDisplay.SpendStars(defenderCost);
    }

    private Vector2 GetSquareClicked() {
        var clickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var worldPos = _mainCam.ScreenToWorldPoint(clickPos);
        var gridPos = SnapToGrid(worldPos);
        return gridPos;
    }

    private static Vector2 SnapToGrid(Vector2 rawWorldPos) {
        var newX = Mathf.RoundToInt(rawWorldPos.x);
        var newY = Mathf.RoundToInt(rawWorldPos.y);
        return new Vector2(newX, newY);
    }
    
    private void SpawnDefender(Vector2 roundedPos) {
        var newDefender = Instantiate(_defender, roundedPos, Quaternion.identity);
        newDefender.transform.parent = _defenderParent.transform;
    }
    
    private GameObject GetDefenderPreview(DefenderType defenderType) {
        GameObject defenderPreview;
        
        switch (defenderType) {
            case DefenderType.Trophy:
                defenderPreview = trophyPreview;
                break;
            case DefenderType.DoubleTrophy:
                defenderPreview = doubleTrophyPreview;
                break;
            case DefenderType.Cactus:
                defenderPreview = cactusPreview;
                break;
            case DefenderType.Scarecrow:
                defenderPreview = scarecrowPreview;
                break;
            case DefenderType.Statue:
                defenderPreview = statuePreview;
                break;
            case DefenderType.Gravestone:
                defenderPreview = gravestonePreview;
                break;
            case DefenderType.Vendor:
                defenderPreview = vendorPreview;
                break;
            case DefenderType.Gnome:
                defenderPreview = gnomePreview;
                break;
            default:
                defenderPreview = trophyPreview;
                break;
        }

        return defenderPreview;
    }
}
