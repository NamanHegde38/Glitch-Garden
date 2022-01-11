using MoreMountains.Feedbacks;
using UnityEngine;

public class Shredder : MonoBehaviour {

    [SerializeField] private MMFeedbacks lifeLostFeedback;
    
    private void OnTriggerEnter2D(Collider2D collision) {
        Destroy(collision.gameObject);

        var yPos = Mathf.RoundToInt(collision.gameObject.transform.position.y);
        var feedbackPosition = new Vector2(transform.position.x + 2f, yPos - 0.3f);
        lifeLostFeedback.PlayFeedbacks(feedbackPosition);
    }
}
