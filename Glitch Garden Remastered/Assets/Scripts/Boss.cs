using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Boss : MonoBehaviour {
    [SerializeField] private MMFeedbacks spawnFeedback;

    public void PlaySpawnFeedback() {
        spawnFeedback.PlayFeedbacks();
    }
}
