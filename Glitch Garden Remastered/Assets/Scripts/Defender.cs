using UnityEngine;

public class Defender : MonoBehaviour {

    [SerializeField] private int starCost = 100;

    public void AddStars(int amount) {
        FindObjectOfType<StarDisplay>().AddStars(amount);
    }

    public int GetStarCost() {
        return starCost;
    }
    
}
