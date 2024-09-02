using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private TextMeshProUGUI gameEndText;
    [SerializeField]
    private GameObject gameEndIconsHolder;


    [Space]
    [SerializeField]
    private TextMeshProUGUI blackTimer;
    [SerializeField]
    private TextMeshProUGUI whiteTimer;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void UpdateTimer(string blackTimerText, string whiteTimerText)
    {
        if(blackTimerText != "")
            blackTimer.text = blackTimerText;
        if(whiteTimerText != "")
            whiteTimer.text = whiteTimerText;
    }
    public void DisplayWinner(PieceColor pieceColor)
    {
        gameEndIconsHolder.SetActive(true);
        //gameEndText.color = Color.green;
        switch (pieceColor)
        {
            case PieceColor.White:
                gameEndText.text = "White Wins!";
                break;
            case PieceColor.Black:
                gameEndText.text = "Black Wins!";
                break;
            default:
                break;
        }
    }

    public void DisplayStaleMate()
    {
        gameEndIconsHolder.SetActive(true);
        gameEndText.color = Color.yellow;
        gameEndText.text = "Stale Mate";
    }
}
