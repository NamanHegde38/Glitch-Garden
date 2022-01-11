using System.Collections;
using TMPro;
using UnityEngine;

public class StarDisplay : MonoBehaviour {

    [SerializeField] private int stars = 100;
    [SerializeField] private int starsOverTime = 3;
    [SerializeField] private int starTime = 5;
    private bool _giveStarsOverTime = true;

    private TextMeshProUGUI _starText;

    private IEnumerator Start() {
        _starText = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();

        while (_giveStarsOverTime) {
            yield return new WaitForSeconds(starTime);
            AddStars(starsOverTime);
        }
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
