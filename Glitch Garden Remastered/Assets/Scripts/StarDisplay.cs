using TMPro;
using UnityEngine;

public class StarDisplay : MonoBehaviour {

    [SerializeField] private int stars = 100;

    private TextMeshProUGUI _starText;

    private void Start() {
        _starText = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        _starText.text = stars.ToString();
    }

    public bool HaveEnoughStars(int amount) {
        return stars >= amount;
    }
    
    public void AddStars(int amount) {
        stars += amount;
        UpdateDisplay();
    }

    public void SpendStars(int amount) {
        if (stars < amount) return;
        stars -= amount;
        UpdateDisplay();
    }
}
