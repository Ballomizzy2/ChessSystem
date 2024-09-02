using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameSettings gameSettings;
    [SerializeField]
    private GameUI gameUI;
    public PieceColor currentColorTurn { get; private set; }
    private PieceColor startColor;

    [SerializeField]
    private ColorPiecesManager whiteManager, blackManager;

    private MoveLogsManager moveLogsManager;

    [SerializeField]
    private GameObject whiteTurnIndicator, blackTurnIndicator;

    private float whiteTimer, blackTimer;
    public void SwitchTurn()
    {
        switch (currentColorTurn)
        {
            case PieceColor.White:
                currentColorTurn = PieceColor.Black;
                blackManager.CheckForStaleMate();
                blackManager.MakeChildrenSelectable(true);
                whiteManager.MakeChildrenSelectable(false);
                blackTurnIndicator.gameObject.SetActive(true);
                whiteTurnIndicator.gameObject.SetActive(false);
                break;
            case PieceColor.Black:
                currentColorTurn = PieceColor.White;
                whiteManager.CheckForStaleMate();
                whiteManager.MakeChildrenSelectable(true);
                blackManager.MakeChildrenSelectable(false);
                blackTurnIndicator.gameObject.SetActive(false);
                whiteTurnIndicator.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void MakeAllPiecesUnSelectable(bool yes)
    {
        if (yes)
        {
            blackManager.MakeChildrenSelectable(false);
            whiteManager.MakeChildrenSelectable(false);
        }
        else
        {
            switch (currentColorTurn)
            {
                case PieceColor.White:
                    whiteManager.MakeChildrenSelectable(true);
                    blackManager.MakeChildrenSelectable(false);
                    break;
                case PieceColor.Black:
                    blackManager.MakeChildrenSelectable(true);
                    whiteManager.MakeChildrenSelectable(false); 
                   break;
                default:
                    break;
            }
        }

    }

    public void CheckMate(PieceColor pieceThatLost)
    {
        switch (pieceThatLost)
        {
            case PieceColor.White:
                gameUI.DisplayWinner(PieceColor.Black);
                MoveLogsManager.MoveLogInfo lastInfo = moveLogsManager.lastInfo;
                MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(lastInfo.piece, lastInfo.oldPos, lastInfo.newPos, lastInfo.isCheck, true, lastInfo.isCapture, lastInfo.isPromote, lastInfo.promotePiece, lastInfo.isKingSideCastle, lastInfo.isQueenSideCastle);
                moveLogsManager.lastEdittedLogText.text = moveLogsManager.MoveLogText(info);
                break;
            case PieceColor.Black:
                gameUI.DisplayWinner(PieceColor.White);
                MoveLogsManager.MoveLogInfo lastInfo1 = moveLogsManager.lastInfo;
                MoveLogsManager.MoveLogInfo info1 = new MoveLogsManager.MoveLogInfo(lastInfo1.piece, lastInfo1.oldPos, lastInfo1.newPos, lastInfo1.isCheck, true, lastInfo1.isCapture, lastInfo1.isPromote, lastInfo1.promotePiece, lastInfo1.isKingSideCastle, lastInfo1.isQueenSideCastle);
                moveLogsManager.lastEdittedLogText.text = moveLogsManager.MoveLogText(info1);
                break;
            default:
                break;
        }
    }
    public void StaleMate()
    {
        gameUI.DisplayStaleMate();
    }

    private void Start()
    {        
        startColor = gameSettings.firstMoveColor;
        currentColorTurn = startColor;
        if(currentColorTurn == PieceColor.Black)
        {
            whiteTurnIndicator.gameObject.SetActive(false);
            blackTurnIndicator.gameObject.SetActive(true);
        }
        else
        {
            whiteTurnIndicator.gameObject.SetActive(true);
            blackTurnIndicator.gameObject.SetActive(false);
        }

        moveLogsManager = FindObjectOfType<MoveLogsManager>();

        whiteTimer = gameSettings.gameMinutes * 60; 
        blackTimer = gameSettings.gameMinutes * 60; //60 for minutes to seconds conversion

        gameSettings.ChangeGameState(GameState.PlayState);
    }

    private void Update()
    {
        if(gameSettings.gameState == GameState.PlayState)
            UpdateTimers();
        CheckIfTimersAreOut();
        
    }

    private void UpdateTimers()
    {
        if (currentColorTurn == PieceColor.Black)
        {
            blackTimer -= Time.deltaTime;
            Debug.Log(blackTimer.ToString());
        }
        else if (currentColorTurn == PieceColor.White)
        {
            whiteTimer -= Time.deltaTime;
            Debug.Log(whiteTimer.ToString());
        }

        //black
        int secondsLeft = (Mathf.CeilToInt((float)blackTimer % 60));
        if (secondsLeft < 10)
        {
            string timerFormattedTextBlack = (Mathf.FloorToInt((float)blackTimer / 60)).ToString() + ":0" + secondsLeft.ToString();
            gameUI.UpdateTimer(timerFormattedTextBlack, "");
        }
        else if (secondsLeft == 60)
        {
            string timerFormattedTextBlack1 = (Mathf.FloorToInt((float)blackTimer / 60)).ToString() + ":00";
            gameUI.UpdateTimer(timerFormattedTextBlack1, "");
        }
        else
        {
            string timerFormattedTextBlack1 = (Mathf.FloorToInt((float)blackTimer / 60)).ToString() + ":" + secondsLeft.ToString();
            gameUI.UpdateTimer(timerFormattedTextBlack1, "");
        }
        //white
        int secondsLeftWhite = (Mathf.CeilToInt((float)whiteTimer % 60));
        if (secondsLeftWhite < 10)
        {
            string timerFormattedTextWhite = (Mathf.FloorToInt((float)whiteTimer / 60)).ToString() + ":0" + secondsLeftWhite.ToString();
            gameUI.UpdateTimer("", timerFormattedTextWhite);
        }
        else if (secondsLeftWhite == 60)
        {
            string timerFormattedTextWhite = (Mathf.FloorToInt((float)whiteTimer / 60)).ToString() + ":00";
            gameUI.UpdateTimer("", timerFormattedTextWhite);
        }
        else
        {
            string timerFormattedTextWhite1 = (Mathf.FloorToInt((float)whiteTimer / 60)).ToString() + ":" + secondsLeftWhite.ToString();
            gameUI.UpdateTimer("", timerFormattedTextWhite1);
        }
    }

    private void CheckIfTimersAreOut()
    {
        if(blackTimer < 1)
        {
            gameSettings.ChangeGameState(GameState.PostState);
            gameUI.UpdateTimer("0:00", "");
            CheckMate(PieceColor.Black);

        }
        if(whiteTimer < 1)
        {
            gameSettings.ChangeGameState(GameState.PostState);
            gameUI.UpdateTimer("", "0:00");
            CheckMate(PieceColor.White);
        }
    }
}


