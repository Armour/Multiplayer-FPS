using UnityEngine;
using System.Collections;

public class PlayerScore : MonoBehaviour {

    [HideInInspector] public int score = 0;
    [HideInInspector] public int killed = 0;
    [HideInInspector] public string notification;

    public void addKilled() {
        killed++;
        switch (killed) {
        case 1:
            score += 10;
            notification = null;
            break;
        case 2:
            score += 15;
            notification = "Double Kill";
            break;
        case 3:
            score += 25;
            notification = "Triple Kill";
            break;
        case 4:
            score += 40;
            notification = "Killing Spring";
            break;
        default:
            score += 60;
            notification = "God Like";
            break;
        }
    }

    public void addScore(int newScore) {
        score += newScore;
    }

}
