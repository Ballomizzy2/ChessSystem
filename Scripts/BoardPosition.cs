using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardPosition : MonoBehaviour
{
    private BoardManager boardManager;

    private Position boardPosition;
    public bool isHighlighted { get; private set; }
    public bool isCaptureSpot { get; private set; }
    public bool isCastleSpot { get; private set; }
    public bool isEnPassantableSpot { get; private set; }
    public bool isEnPassantableCaptureSpot { get; private set; }
    public void HighlightTile(bool yes)
    {
        isHighlighted = yes;
    }
    public void MarkCaptureTile(bool yes)
    {
        isCaptureSpot = yes;
    }
    public void SetBoardPositionAsCastleSpot(bool yes)
    {
        isCastleSpot = yes;
    }
    public void SetTileAsEnPassantableSpot(bool yes)
    {
        isEnPassantableSpot = yes;
    }
    public void SetTileAsEnPassantCaptureSpot(bool yes)
    {
        isEnPassantableCaptureSpot = yes;
    }
    public bool isOccupied { get; private set; } = false;
    public PieceColor occupantColor { get; private set; } = PieceColor.Empty;
    public PiecesType occupantType { get; private set; }
    public PieceController pieceOccupant { get; private set; }
    public PieceController previousPieceOccupant { get; private set; }


    public void OccupyBoardTile(PieceColor _occupanntColor, PiecesType _pieceType ,PieceController _pieceController)
    {
        isOccupied = true;
        occupantColor = _occupanntColor;
        occupantType = _pieceType;
        pieceOccupant = _pieceController;
    }
    public void DeOccupyBoardTile()
    {
        isOccupied = false;
        occupantColor = PieceColor.Empty;
        pieceOccupant = null;
    }

    private GameManager gameManager;
    private Renderer rend;
    private PawnPromotionMenuScript pawnPromotion;
    private MoveLogsManager moveLogsManager;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        rend = GetComponent<Renderer>();
        boardManager = FindObjectOfType<BoardManager>();
        pawnPromotion = FindObjectOfType<PawnPromotionMenuScript>();
        moveLogsManager = FindObjectOfType<MoveLogsManager>();
    }

    public void SetBoardPositionColor(Color color)
    {
        rend.material.color = color;
    }
    public Color GetBoardPositionColor()
    {
        return rend.material.color;
    }

    public BoardPosition GetBoardPositionController()
    {
        return this;
    }

    public void SetPosition(int x, int y)
    {
        char c = ' ';
        switch (x)
        {
            case 1:
                c = 'a';
                break;
            case 2: 
                c = 'b';
                break;
            case 3:
                c = 'c';
                break;
            case 4:
                c = 'd';
                break;
            case 5:
                c = 'e';
                break;
            case 6:
                c = 'f';
                break;
            case 7:
                c = 'g';
                break;
            case 8:
                c = 'h';
                break;
            default:
                break;

        }
        boardPosition = new Position(c, y);
    }
    public Position GetBoardPosition()
    {
        return this.boardPosition;
    }
    public string GetBoardPositionString()
    {
        string s = ("Board Pos " + boardPosition.xPos.ToString() + boardPosition.yPos.ToString());
        return s;
    }

    private void OnMouseDown()
    {
        Debug.Log("Tile was clicked");
        if (isHighlighted) 
        {
            if (isOccupied)
            {
                previousPieceOccupant = pieceOccupant;
            }
            isOccupied = true;
            int yPosValue = boardPosition.yPos;

            //Castle
            if (isCastleSpot)
            {
                int yPosForCastle = boardManager.selectedTileCtrl.pieceOccupant.GetPieceClass().piecePosition.yPos;
                Position kingPosition;
                bool isKingSideCastle = false, isQueenSideCastle = false;
                if(BoardManager.PosCharToInt(boardPosition.xPos) > 5)
                {
                    isKingSideCastle = true;
                    kingPosition = new Position('g', yPosForCastle);
                    boardManager.selectedTileCtrl.pieceOccupant.MovePiece(kingPosition, false, true);
                    BoardPosition rookBoardPos = BoardManager.GetBoardTile(new Position('h', yPosForCastle));
                    rookBoardPos.pieceOccupant.MovePiece(new Position('f', yPosForCastle), false);
                }
                else
                {
                    isQueenSideCastle = true;
                    kingPosition = new Position('c', yPosForCastle);
                    boardManager.selectedTileCtrl.pieceOccupant.MovePiece(kingPosition, false, true);
                    BoardPosition rookBoardPos = BoardManager.GetBoardTile(new Position('a', yPosForCastle));
                    rookBoardPos.pieceOccupant.MovePiece(new Position('d', yPosForCastle), false);
                }
                pieceOccupant.transform.parent.GetComponent<ColorPiecesManager>().SetKingCheckStatus(false);
                MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(boardManager.selectedTileCtrl.occupantType, boardManager.selectedTileCtrl.boardPosition, kingPosition, false, false, false, false, PiecesType.King, isKingSideCastle, isQueenSideCastle);
                moveLogsManager.MakeMoveLog(info);
                //gameManager.SwitchTurn();
            }
            //EnPassant
            else if (isEnPassantableCaptureSpot)
            {
                isEnPassantableCaptureSpot = false;
                BoardPosition aboveCaptureSpot = BoardManager.GetBoardTile(new Position(boardPosition.xPos, (boardPosition.yPos + 1)));
                BoardPosition belowCaptureSpot = BoardManager.GetBoardTile(new Position(boardPosition.xPos, (boardPosition.yPos - 1)));
                if (aboveCaptureSpot.isEnPassantableSpot && aboveCaptureSpot.occupantColor != occupantColor 
                                                         && aboveCaptureSpot.occupantType == PiecesType.Pawn && aboveCaptureSpot.pieceOccupant.GetPieceClass().moves == 1)
                {
                    aboveCaptureSpot.pieceOccupant.GoToJail();
                    aboveCaptureSpot.DeOccupyBoardTile();
                    boardManager.selectedTileCtrl.pieceOccupant.MovePiece(boardPosition, false);
                    MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(boardManager.selectedTileCtrl.occupantType, boardManager.selectedTileCtrl.boardPosition, aboveCaptureSpot.boardPosition, false, false, true, false, PiecesType.King, false, false);
                    moveLogsManager.MakeMoveLog(info);
                }
                if (belowCaptureSpot.isEnPassantableSpot && belowCaptureSpot.occupantColor != occupantColor
                                                         && belowCaptureSpot.occupantType == PiecesType.Pawn && belowCaptureSpot.pieceOccupant.GetPieceClass().moves == 1)
                {
                    belowCaptureSpot.pieceOccupant.GoToJail();
                    belowCaptureSpot.DeOccupyBoardTile();
                    boardManager.selectedTileCtrl.pieceOccupant.MovePiece(boardPosition, false);
                    MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(boardManager.selectedTileCtrl.occupantType, boardManager.selectedTileCtrl.boardPosition, belowCaptureSpot.boardPosition, false, false, true, false, PiecesType.King, false, false);
                    moveLogsManager.MakeMoveLog(info);
                }
                pieceOccupant.transform.parent.GetComponent<ColorPiecesManager>().SetKingCheckStatus(false);
            }
            //Pawn Promotion
            else if((yPosValue == 8 || yPosValue == 1) && boardManager.selectedTileCtrl.occupantType == PiecesType.Pawn)
            {
                pawnPromotion.ShowMenu(boardManager.selectedTileCtrl.occupantColor, boardPosition, boardManager.selectedTileCtrl.boardPosition);
                boardManager.selectedTileCtrl.pieceOccupant.MovePiece(boardPosition, isCaptureSpot);
            }
            else
            {
                boardManager.selectedTileCtrl.pieceOccupant.MovePiece(boardPosition, isCaptureSpot);
                pieceOccupant.transform.parent.GetComponent<ColorPiecesManager>().SetKingCheckStatus(false);
                MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(boardManager.selectedTileCtrl.occupantType, boardManager.selectedTileCtrl.boardPosition, boardPosition, false, false, isCaptureSpot, false, PiecesType.King, false, false);
                moveLogsManager.MakeMoveLog(info);
            }
            pieceOccupant.GetComponentInParent<ColorPiecesManager>().HasEnpassantSpot(false);
            boardManager.DeHighlightTiles();
            boardManager.DeSelectTile();
        }
    }
}

[System.Serializable]
public class Position
{
    public char xPos;
    public int yPos;
    public bool isCastlePosition = false,
                isEnPassantableSpot = false,
                isEnPassantableCaptureSpot = false;

    public Position(char _xPos, int _yPos)
    {
        xPos = _xPos;
        yPos = _yPos;
    }
    public Position(string xy)
    {
        xPos = xy[0];
        yPos = xy[1] - 48; //convert the char to int by removing 48 i.e. ASCII
    }
    public Position (int _x, int _y)
    {
        xPos = BoardManager.PosIntToChar(_x);
        yPos = _y;
    }

    public void SetAsCastleSpot()
    {
        isCastlePosition = true;
    }

    public void SetAsEnPassantSpot()
    {
        isEnPassantableSpot = true;
    }
    public void SetAsEnPassantCaptureSpot()
    {
        isEnPassantableCaptureSpot = true;
    }

    public override string ToString()
    {
        return xPos.ToString() + yPos.ToString();
    }

    public bool Equals(Position position)
    {
        if(this.xPos == position.xPos && this.yPos == position.yPos)
            return true;
        else 
            return false;
    }
}
