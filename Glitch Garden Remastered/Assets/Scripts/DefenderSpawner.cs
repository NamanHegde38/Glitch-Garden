using UnityEngine;

public class DefenderSpawner : MonoBehaviour {

    private Defender _defender;
    private GameObject _defenderParent;
    private StarDisplay _starDisplay;

    private const string Defenders = "Defenders";
    private void Start() {
        _starDisplay = FindObjectOfType<StarDisplay>();
        CreateDefenderParent();
    }

    private void CreateDefenderParent() {
        _defenderParent = GameObject.Find(Defenders);
        if (!_defenderParent) {
            _defenderParent = new GameObject(Defenders);
        }
    }

    private void OnMouseDown() {
        if (!_defender) return;
        AttemptToPlaceDefenderAt(GetSquareClicked());
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
    
    private static Vector2 GetSquareClicked() {
        var clickPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var worldPos = Camera.main.ScreenToWorldPoint(clickPos);
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
}
