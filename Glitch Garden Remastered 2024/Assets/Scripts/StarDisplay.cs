using System.Collections;
using TMPro;
using UnityEngine;

public class StarDisplay : MonoBehaviour {

    private int _stars = 100;
    private int _starsOverTime = 50;
    private int _starTime = 10;
    private bool _giveStarsOverTime = true;

    private TextMeshProUGUI _starText;

    private IEnumerator Start() {
        _starText = GetComponent<TextMeshProUGUI>();
        UpdateDisplay();

        while (_giveStarsOverTime) {
            yield return new WaitForSeconds(_starTime);
            AddStars(_starsOverTime);
        }
    }

    private void UpdateDisplay() {
        _starText.text = _stars.ToString();
    }

    public bool HaveEnoughStars(int amount) {
        return _stars >= amount;
    }

    public void SetStars(int startStars) {
        _stars = startStars;
    }
    
    public void SetStarsOverTime(int starsOverTime) {
        _starsOverTime = starsOverTime;
    }
    
    public void AddStars(int amount) {
        var resultingStars = _stars + amount;
        _stars = resultingStars > 999 ? 999 : resultingStars;
        UpdateDisplay();
    }

    public void SpendStars(int amount) {
        if (_stars < amount) return;
        _stars -= amount;
        UpdateDisplay();
    }
}
