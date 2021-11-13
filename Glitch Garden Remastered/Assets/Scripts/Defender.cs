using UnityEngine;

public class Defender : MonoBehaviour {

    [SerializeField] private int starCost = 100;
    [SerializeField] private DefenderMaterial material;

    public void AddStars(int amount) {
        FindObjectOfType<StarDisplay>().AddStars(amount);
    }

    public int GetStarCost() {
        return starCost;
    }

    public DefenderMaterial GetDefenderMaterial() {
        return material;
    }
    
}
