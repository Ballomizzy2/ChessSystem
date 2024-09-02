using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PieceController : MonoBehaviour
{
    private GameManager gameManager;
    private BoardManager boardManager;
    private PiecesManager piecesManager;
    private ColorPiecesManager colorPiecesManager;
    private MoveLogsManager moveLogsManager;

    Position currentPositionPos, endPositionPos;
    Vector3 positionToMoveTo, currentPosition;
    private bool isPositionSet, isMoving;
  



    private Piece pieceClass;
    public void InitPiece(Piece piece)
    {
        pieceClass = piece;
    }
    public Piece GetPieceClass()
    {
        return pieceClass;
    }

    [Header("Piece Stats")]
    private int moves = 0;
    public int GetMovesAmount()
    {
        return moves;
    }
    [SerializeField]
    private float pieceMoveSpeed = 10f;
    private bool isCastleMove = false;

    public void InitPosition(Position position)
    {
        if (!isPositionSet)
        {
            currentPositionPos = position;
            isPositionSet = true;
        }
    }

    public void MovePiece(Position position, bool isCapturingMove)
    {

        positionToMoveTo = boardManager.GetBoardTilePos(position);
        
        BoardPosition newBoardPosition = BoardManager.GetBoardTile(position);
        newBoardPosition.OccupyBoardTile(pieceClass.pieceColor, pieceClass.pieceType, this);
        
        BoardPosition oldBoardPosition = BoardManager.GetBoardTile(currentPositionPos);
        oldBoardPosition.DeOccupyBoardTile();


        isMoving = true;
        endPositionPos = position;
        if (isCapturingMove)
        {
            //Cpature animation
            BoardManager.GetBoardTile(position).previousPieceOccupant.GoToJail();
        }

    }
    public void MovePiece(Position position, bool isCapturingMove, bool _isCastleMove)
    {

        positionToMoveTo = boardManager.GetBoardTilePos(position);

        BoardPosition newBoardPosition = BoardManager.GetBoardTile(position);
        newBoardPosition.OccupyBoardTile(pieceClass.pieceColor, pieceClass.pieceType, this);

        BoardPosition oldBoardPosition = BoardManager.GetBoardTile(currentPositionPos);
        oldBoardPosition.DeOccupyBoardTile();

        isCastleMove = _isCastleMove;
        isMoving = true;
        endPositionPos = position;
        if (isCapturingMove)
        {
            //Cpature animation
            BoardManager.GetBoardTile(position).previousPieceOccupant.GoToJail();
        }

    }

    private void StopMoving()
    {
        //yield return new WaitForSeconds(1f);
        isMoving = false;
        currentPositionPos = endPositionPos;
        endPositionPos = null;
        moves++;
        UpdatePieceClassData();
        CheckIfChecking();
        if (!isCastleMove)
        {
            gameManager.SwitchTurn();
        }
        else
            isCastleMove = false;
        //Check if checking opponnent king
    }
    public void MoveAnimation(Vector3 position)
    {
        transform.position = Vector3.MoveTowards
        (
            transform.position, 
            new Vector3(position.x, transform.position.y, position.z), 
            pieceMoveSpeed * Time.deltaTime
        );
    }
    public void GoToJail()
    {
        switch (pieceClass.pieceColor)
        {
            case PieceColor.White:
                Transform jail = piecesManager.BlackJailGO.transform;
                transform.position = new Vector3(jail.position.x, transform.position.y, transform.position.x);
                transform.SetParent(jail);
                jail.GetComponent<JailManager>().ArrangePieces();
                break;
            case PieceColor.Black:
                Transform jail1 = piecesManager.WhiteJailGO.transform;
                transform.position = new Vector3(jail1.position.x, transform.position.y, transform.position.x);
                transform.SetParent(jail1);
                jail1.GetComponent<JailManager>().ArrangePieces();
                break;
            default:
                break;
        }
    }
    public void CheckIfChecking()
    {
        Check checkData = pieceClass.IsCheckingOpponent(/*FindPossibleMoves().captureMoves*/);
        Debug.Log(FindPossibleMoves().movablePositions.Length);
        Debug.Log("CheckIfChecking for check " + checkData.isChecking);
        Debug.Log(currentPosition.ToString());
        if (checkData.isChecking)
        {
            switch (pieceClass.pieceColor)
            {
                case PieceColor.Black:
                    piecesManager.SetColorOnCheck(PieceColor.White);
                    boardManager.SetCheckTile(checkData.checkPosition, true);
                    MoveLogsManager.MoveLogInfo lastInfo = moveLogsManager.lastInfo;
                    MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(lastInfo.piece, lastInfo.oldPos, lastInfo.newPos, true, lastInfo.isCheckMate, lastInfo.isCapture, lastInfo.isPromote, lastInfo.promotePiece, lastInfo.isKingSideCastle, lastInfo.isQueenSideCastle);
                    moveLogsManager.lastEdittedLogText.text = moveLogsManager.MoveLogText(info);
                    break;
                case PieceColor.White:
                    piecesManager.SetColorOnCheck(PieceColor.Black);
                    boardManager.SetCheckTile(checkData.checkPosition, true);
                    MoveLogsManager.MoveLogInfo lastInfo1 = moveLogsManager.lastInfo;
                    MoveLogsManager.MoveLogInfo info1 = new MoveLogsManager.MoveLogInfo(lastInfo1.piece, lastInfo1.oldPos, lastInfo1.newPos, true, lastInfo1.isCheckMate, lastInfo1.isCapture, lastInfo1.isPromote, lastInfo1.promotePiece, lastInfo1.isKingSideCastle, lastInfo1.isQueenSideCastle);
                    moveLogsManager.lastEdittedLogText.text = moveLogsManager.MoveLogText(info1);
                    break;
                default:
                    break;
            }
        }
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        boardManager = FindObjectOfType<BoardManager>();
        piecesManager = FindObjectOfType<PiecesManager>();
        colorPiecesManager = FindObjectOfType<ColorPiecesManager>();
        moveLogsManager = FindObjectOfType<MoveLogsManager>();
    }
    private void Start()
    {
        InitPosition(pieceClass.startPosition);
        colorPiecesManager = transform.parent.GetComponent<ColorPiecesManager>();
    }

    private void Update()
    {
        UpdatePieceClassData();

        if (isMoving)
        {
            MoveAnimation(positionToMoveTo);
            if(transform.position == currentPosition)
                StopMoving();
        }
        
    }

    private void LateUpdate()
    {
        if (isMoving)
        {
            currentPosition = transform.position;
        }
    }
    public Move FindPossibleMoves()
    {
        return pieceClass.SimulateMove();
    }

    public void UpdatePieceClassData()
    {
        pieceClass.piecePosition = currentPositionPos;
        pieceClass.moves = moves;
        if (!pieceClass.controller)
        {
            pieceClass.controller = this;
        }
    }

    private Position[] FilterMovesThatMakeCheckThreats(Move prospectiveMoves)
    {
        List<Position> badPositions = new List<Position>();
        List<Piece> piecesThatCanCheck = new List<Piece>();

        for(int i = 0; i < prospectiveMoves.movablePositions.Length; i++)
        {
            // Checks if the piece moving causes their king to be on check

            //Find the container of othe opponents players
            Transform piecesColorContainer = null;
            switch (pieceClass.pieceColor)
            {
                case PieceColor.White:
                    piecesColorContainer = piecesManager.BlackPiecesContainer.transform;
                    break;
                case PieceColor.Black:
                    piecesColorContainer = piecesManager.WhitePiecesContainer.transform;
                    break;
                default:
                    break;
            }

            //Loop through all the opponent's pieces
            for (int x = 0; x < piecesColorContainer.childCount; x++)
            {
                //Simulate moves for each opponent piece as if the current piece has moved
                Move simulatedMoves = piecesColorContainer.GetChild(x).GetComponent<PieceController>().pieceClass.SimulateMove
                (
                    currentPositionPos, prospectiveMoves.movablePositions[i]
                );

                List<Position> capturePositions = new List<Position>();
                foreach (Position pos in simulatedMoves.captureMoves)
                {
                    capturePositions.Add(pos);
                }

                for (int j = 0; j < capturePositions.Count; j++)
                {
                    BoardPosition boardPos = BoardManager.GetBoardTile(capturePositions[j]);
                    if ((boardPos.occupantType == PiecesType.King && boardPos.occupantColor == pieceClass.pieceColor)
                        || this.pieceClass.pieceType == PiecesType.King && prospectiveMoves.movablePositions[i].Equals(capturePositions[j]))
                    {
                        badPositions.Add(prospectiveMoves.movablePositions[i]);
                        //piecesThatCanCheck.Add(piecesColorContainer.GetChild(x).GetComponent<PieceController>().pieceClass);
                        //return true;
                    }
                    //if King, try to check if any piece can stop castling by checking the castling path
                    if (pieceClass.pieceType == PiecesType.King && moves == 0 && !colorPiecesManager.isOnCheck)
                    {
                        for (int y = 0; y < badPositions.Count; y++)
                        {
                            //if a king's castle path is on check, remove the castle position

                            if((currentPositionPos.yPos == prospectiveMoves.movablePositions[i].yPos) 
                                && ((BoardManager.PosCharToInt(badPositions[y].xPos) + 1) == BoardManager.PosCharToInt(prospectiveMoves.movablePositions[i].xPos)
                                || (BoardManager.PosCharToInt(badPositions[y].xPos) - 1) == BoardManager.PosCharToInt(prospectiveMoves.movablePositions[i].xPos)))
                            {
                                badPositions.Add(prospectiveMoves.movablePositions[i]);
                            }

                        }
                    }
                }
            }
        }

        for(int i = 0; i < prospectiveMoves.captureMoves.Length; i++)
        {
            // Checks if the piece capturing causes their king to be in check
            //Find the container of othe opponents players
            Transform piecesColorContainer = null;
            switch (pieceClass.pieceColor)
            {
                case PieceColor.White:
                    piecesColorContainer = piecesManager.BlackPiecesContainer.transform;
                    break;
                case PieceColor.Black:
                    piecesColorContainer = piecesManager.WhitePiecesContainer.transform;
                    break;
                default:
                    break;
            }

            //Loop through all the opponent's pieces
            for (int x = 0; x < piecesColorContainer.childCount; x++)
            {
                //Simulate moves for each opponent piece as if the current piece has moved
                Move simulatedMoves = piecesColorContainer.GetChild(x).GetComponent<PieceController>().pieceClass.SimulateMove
                (
                    currentPositionPos, prospectiveMoves.captureMoves[i]
                );

                List<Position> capturePositions = new List<Position>();
                foreach (Position pos in simulatedMoves.captureMoves)
                {
                    capturePositions.Add(pos);
                }
                
                for (int j = 0; j < capturePositions.Count; j++)
                {
                    BoardPosition boardPos = BoardManager.GetBoardTile(capturePositions[j]);
                    if ((boardPos.occupantType == PiecesType.King && boardPos.occupantColor == pieceClass.pieceColor) 
                        || this.pieceClass.pieceType == PiecesType.King && prospectiveMoves.captureMoves[i].Equals(capturePositions[j]))
                    {
                        if (!(piecesColorContainer.GetChild(x).GetComponent<PieceController>().currentPositionPos.Equals(prospectiveMoves.captureMoves[i])))
                            badPositions.Add(prospectiveMoves.captureMoves[i]);                         
                            //return true;
                    }

                }
            }
        }
        return badPositions.ToArray();
        //return false;

    }

    public Move MakeValidMove()
    {
        Move moves = FindPossibleMoves();
        //Filter out moves that may cause them to be on check
        /*if (!DoesMoveMakesCheckThreats(moves))
        {
            boardManager.HighlightTiles(moves.movablePositions, moves.captureMoves);
        }*/

        Position[] badPositions = FilterMovesThatMakeCheckThreats(moves);
        Position[] movablePositions = moves.movablePositions;
        Position[] captureMoves = moves.captureMoves;
        List<Position> movablePositionsList = new List<Position>();
        List<Position> capturePositionsList = new List<Position>();

        for (int x = 0; x < movablePositions.Length; x++)
        {
            for (int i = 0; i < badPositions.Length; i++)
            {
                if ((movablePositions[x] != null && badPositions[i] != null) && movablePositions[x].Equals(badPositions[i]))
                {
                    movablePositions[x] = null;
                }
            }
        }

        for (int x = 0; x < captureMoves.Length; x++)
        {
            for (int i = 0; i < badPositions.Length; i++)
            {
                if ((captureMoves[x] != null && badPositions[i] != null) && captureMoves[x].Equals(badPositions[i]))
                {
                    captureMoves[x] = null;
                }
            }
        }

        for (int i = 0; i < movablePositions.Length; i++)
        {
            if (movablePositions[i] != null)
                movablePositionsList.Add(movablePositions[i]);
        }

        for (int i = 0; i < captureMoves.Length; i++)
        {
            if (captureMoves[i] != null)
                capturePositionsList.Add(captureMoves[i]);
        }
        Move newMove = new Move(movablePositionsList.ToArray(), capturePositionsList.ToArray(), null);
        return newMove;
        
    }

    private void OnMouseDown()
    {
        if (gameManager.currentColorTurn == pieceClass.pieceColor)
        {
            boardManager.SelectTile(currentPositionPos);
            Move validMove = MakeValidMove();
            //boardManager.HighlightTiles(moves.movablePositions, moves.captureMoves);
            boardManager.HighlightTiles(validMove.movablePositions, validMove.captureMoves, validMove.specialMoves);
        }
    }
}


/*
 * 
//Move a piece
1. User Selects the piece \
    a. Spawn Selector under piece but above board \

2. Show the available spots
    a. Check if spot is available
    b. Check if spot would make king on check
    c. if King, Check if new spot would make king on check
    d. Show spots

3. User Selects spot
    a. Piece moves
    b. Highlight previous spot
    c. Highlight new spot
    d. Occupy new Spot and deOccupy old spot

*/

