using UnityEngine;

// TODO: finish score system later, this script is not used now.
public class PlayerScore : MonoBehaviour {

    private int score = 0;
    private int killed = 0;
    // private string notification;

    public void IncreaseKillCount() {
        killed++;
        switch (killed) {
        case 1:
            score += 10;
            // notification = null;
            break;
        case 2:
            score += 15;
            // notification = "Double Kill";
            break;
        case 3:
            score += 25;
            // notification = "Triple Kill";
            break;
        case 4:
            score += 40;
            // notification = "Killing Spring";
            break;
        default:
            score += 60;
            // notification = "God Like";
            break;
        }
    }

    public void AddScore(int newScore) {
        score += newScore;
    }

}
