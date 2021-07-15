using UnityEngine;
using TMPro;

public class DefenderButton : MonoBehaviour {

    [SerializeField] private Defender defenderPrefab;
    
    private SpriteRenderer _sprite;
    private DefenderButton[] _buttons;
    private DefenderSpawner _spawner;

    private void Start() {
        _sprite = GetComponent<SpriteRenderer>();
        _buttons = FindObjectsOfType<DefenderButton>();
        _spawner = FindObjectOfType<DefenderSpawner>();

        LabelButtonCost();
    }

    private void LabelButtonCost() {
        var costText = GetComponentInChildren<TextMeshProUGUI>();
        if (!costText) {
            Debug.LogError(name + " has no cost text");
        }
        else {
            costText.text = defenderPrefab.GetStarCost().ToString();
        }
    }

    private void OnMouseDown() {
        foreach (var button in _buttons) {
            button.GetComponent<SpriteRenderer>().color = new Color32(64, 64, 64, 255);
        }
        _sprite.color = Color.white;
        _spawner.SetSelectedDefender(defenderPrefab);
    }
}
