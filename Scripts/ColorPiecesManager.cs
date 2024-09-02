using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPiecesManager : MonoBehaviour
{
    [SerializeField]
    private PieceColor piecesColors;
    private BoardManager boardManager;
    private GameManager gameManager;


    private void Awake()
    {
        boardManager = FindObjectOfType<BoardManager>();
        gameManager = FindObjectOfType<GameManager>();
    }
    public bool isOnCheck { get; private set; }

    public void SetKingCheckStatus(bool check)
    {
        isOnCheck = check;
        if (!check)
            boardManager.DestroyCheckTile();

        if (IsCheckMate())
        {
            gameManager.CheckMate(piecesColors);
        }
    }
    public bool hasEnpassantable { get; private set; }
    public void HasEnpassantSpot(bool yes)
    {
        hasEnpassantable = yes;
    }

    public void MakeChildrenSelectable(bool isSelectable)
    {
        int newLayer = -1;
        if (isSelectable)
            newLayer = LayerMask.NameToLayer("Default");
        else
            newLayer = LayerMask.NameToLayer("Ignore Raycast");

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.layer = newLayer;
        }
    }

    public void CheckForStaleMate()
    {
        if (IsStaleMate())
        {
            gameManager.StaleMate();
        }
    }

    public bool IsCheckMate()
    {
        if (isOnCheck)
        {
            int numberOfPossibleMoves = 0;
            for(int i = 0; i < transform.childCount; i++)
            {
                Move allMoves = transform.GetChild(i).GetComponent<PieceController>().MakeValidMove();
                numberOfPossibleMoves += allMoves.movablePositions.Length + allMoves.captureMoves.Length;
            }
            if (numberOfPossibleMoves == 0)
                return true;
            else
                return false;
        }
        return false;
    }

    public bool IsStaleMate()
    {
        if (!isOnCheck)
        {
            int numberOfPossibleMoves = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Move allMoves = transform.GetChild(i).GetComponent<PieceController>().MakeValidMove();
                numberOfPossibleMoves += allMoves.movablePositions.Length + allMoves.captureMoves.Length;
            }
            if (numberOfPossibleMoves == 0)
                return true;
            else
                return false;
        }
        return false;
    }
}
