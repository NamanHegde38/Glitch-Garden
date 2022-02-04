using System;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Defender : MonoBehaviour {

    [SerializeField] private int starCost = 100;
    [SerializeField] private DefenderMaterial material;
    [SerializeField] private DefenderType type;

    [SerializeField] private MMFeedbacks spawnFeedback;

    private void Start() {
        spawnFeedback.Initialization();
        spawnFeedback.PlayFeedbacks();
    }

    public void AddStars(int amount) {
        FindObjectOfType<StarDisplay>().AddStars(amount);
    }

    public int GetStarCost() {
        return starCost;
    }

    public DefenderMaterial GetDefenderMaterial() {
        return material;
    }

    public DefenderType GetDefenderType() {
        return type;
    }
}

