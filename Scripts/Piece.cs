using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Piece
{
    public PiecesType pieceType;
    public PieceColor pieceColor;
    public GameObject pieceGameObject;
    public Position piecePosition, startPosition;
    public PieceController controller;
    public int moves = 0;
    public bool isEnPassantable;
    private Position simulateMoveWithException = null,
                     simulateMoveWithOccupiedPosition = null;

    public void SetValues(PiecesType _piecesType, PieceColor _pieceColor, GameObject _pieceGameObject, Position _startPosition)
    {
        pieceType = _piecesType;
        pieceColor = _pieceColor;
        pieceGameObject = _pieceGameObject;
        startPosition = _startPosition;
    }

    public Move SimulateMove()
    {
        switch (pieceType)
        {
            case PiecesType.Pawn:
                return pawnMove();
            case PiecesType.Rook:
                return rookMove();
            case PiecesType.Knight:
                return knightMove();
            case PiecesType.Bishop:
                return bishopMove();
            case PiecesType.Queen:
                return queenMove();
            case PiecesType.King:
                return kingMove();
            default:
                return new Move();
        }
    }
    public Move SimulateMove(Position positionException, Position newFakeOccupiedPosition)
    {
        simulateMoveWithException = positionException;
        simulateMoveWithOccupiedPosition = newFakeOccupiedPosition;
        switch (pieceType)
        {
            case PiecesType.Pawn:
                return pawnMove();
            case PiecesType.Rook:
                return rookMove();
            case PiecesType.Knight:
                return knightMove();
            case PiecesType.Bishop:
                return bishopMove();
            case PiecesType.Queen:
                return queenMove();
            case PiecesType.King:
                return kingMove();
            default:
                return new Move();
        }
    }

    private Move pawnMove()
    {
        List<Position> maxMoves = new List<Position>();
        List<Position> captureMoves = new List<Position>();
        List<Position> specialMoves = new List<Position>();
        int sign = 0;//Use this to change the forward of the piece if it's the other piece
        switch (pieceColor)
        {
            case PieceColor.White:
                sign = -1;//negative
                break;
            case PieceColor.Black:
                sign = 1;
                break;
            default:
                break;
        }
        // Check for pawn capture spots
        for (int i = 0, x = 1; i < 2; i++)
        {
            Position newPos;
            if(i % 2 == 0)
                newPos = (new Position(BoardManager.PosCharToInt(piecePosition.xPos) + x, piecePosition.yPos + (1 * sign)));
            else
                newPos = (new Position(BoardManager.PosCharToInt(piecePosition.xPos) - x, piecePosition.yPos + (1 * sign)));
            
            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                captureMoves.Add(newPos);
                //check if spot is occupied by opponent or else remove it
                //also check if the position is an exception position
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);

                if (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos)))
                {

                    if (!boardPosition.isOccupied || (boardPosition.occupantColor == pieceColor || boardPosition.occupantColor == PieceColor.Empty)) 
                        captureMoves.Remove(newPos);
                    //check if spot behind the capture spot is an enpassantable spot
                    
                    if (BoardManager.GetBoardTile(new Position(xPos, yPos - sign)).isEnPassantableSpot && pieceGameObject.GetComponentInParent<ColorPiecesManager>().hasEnpassantable)
                    {
                        captureMoves.Add(newPos);
                        newPos.SetAsEnPassantCaptureSpot();
                    }
                }
            }
        }

        //Other movable spots
        if (moves == 0)// first pawn move
        {
            for(int i = 0; i < 2; i++)
            {
                Position newPos = new Position(piecePosition.xPos, piecePosition.yPos + ((i + 1) * sign));

                //check if spot is on board
                int xPos = BoardManager.PosCharToInt(newPos.xPos),
                    yPos = newPos.yPos;

                if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
                {
                    maxMoves.Add(newPos);
                    //When a pawn moves two steps and has a pawn beside it, set the spot as an enpassantable spot
                    if(xPos > 1)
                    {
                        BoardPosition positionAtLeft = BoardManager.GetBoardTile(new Position((xPos - 1), yPos));
                        if (i == 1 && (positionAtLeft.isOccupied && positionAtLeft.occupantColor != pieceColor && positionAtLeft.occupantType == PiecesType.Pawn))
                        {
                            newPos.SetAsEnPassantSpot();
                            //pieceGameObject.GetComponentInParent<ColorPiecesManager>().HasEnpassantSpot(true);
                        }

                    }
                    if(xPos < 8)
                    {
                        BoardPosition positionAtRight = BoardManager.GetBoardTile(new Position((xPos + 1), yPos));
                        if (i == 1 && (positionAtRight.isOccupied && positionAtRight.occupantColor != pieceColor && positionAtRight.occupantType == PiecesType.Pawn))
                        {
                            newPos.SetAsEnPassantSpot();
                            //pieceGameObject.GetComponentInParent<ColorPiecesManager>().HasEnpassantSpot(true);
                        }

                    }
                }

                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied))
                {
                    maxMoves.Remove(newPos);
                }
            }
        }
        else //normal pawn move
        {
            Position newPos = (new Position(piecePosition.xPos, piecePosition.yPos + (1 * sign)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so, remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied))
                {
                    maxMoves.Remove(newPos);
                }

                //Promote pawn
                if(yPos == 1 || yPos == 8)
                {
                    maxMoves.Remove(newPos);
                    specialMoves.Add(newPos);
                }
            }
        }


        simulateMoveWithException = null;
        Move newMove = new Move(maxMoves.ToArray(), captureMoves.ToArray(), specialMoves.ToArray());
        return newMove;
    }

    private Move rookMove()
    {
        List<Position> maxMoves = new List<Position>();
        List<Position> captureMoves = new List<Position>();
        List<Position> specialMoves = new List<Position>();

        bool upEnd, leftEnd, rightEnd, downEnd;
        upEnd = leftEnd = rightEnd = downEnd = false;

        //Check possible moves

        //Check up movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (upEnd)
                break;
            Position newPos = new Position(piecePosition.xPos, (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    upEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }

            
        }
        //Check down movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (downEnd)
                break;
            Position newPos = new Position(piecePosition.xPos, (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    downEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }

            
        }
        //Check left movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), piecePosition.yPos);

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }
        //Check right movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), piecePosition.yPos);

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }       
        }


        // Castle

        simulateMoveWithException = simulateMoveWithOccupiedPosition = null;
        Move newMove = new Move(maxMoves.ToArray(), captureMoves.ToArray(), specialMoves.ToArray());
        return newMove;
    }

    private Move knightMove()
    {
        List<Position> maxMoves = new List<Position>();
        List<Position> captureMoves = new List<Position>();
        List<Position> specialMoves = new List<Position>();

        //Check possible moves
        //Knight Find moves algorithm
        for(int x = 0, sign = 0; x < 2; x++)
        {
            switch (x)
            {
                case 0:
                    sign = -1;
                    break;
                case 1:
                    sign = 1;
                    break;
                default:
                    break;
            }
            for (int i = -2, j = 0; i < 3; i++)
            {
                if (i == 0)
                    continue;

                switch(Mathf.Abs(i))
                {
                    case 1:
                        j = 2;
                        break;
                    case 2:
                        j = 1;
                        break;
                    default:
                        break;
                }


                Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + i), (piecePosition.yPos + (sign * (i/i)) * j));

                //check if spot is on board
                int xPos = BoardManager.PosCharToInt(newPos.xPos),
                    yPos = newPos.yPos;

                if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
                {
                    maxMoves.Add(newPos);
                    //check if spot is occupied. if so remove it
                    BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                    if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                         && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                    {
                        maxMoves.Remove(newPos);
                        if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                            captureMoves.Add(newPos);
                    }
                }


            }

        }

        simulateMoveWithException = simulateMoveWithOccupiedPosition = null;
        Move newMove = new Move(maxMoves.ToArray(), captureMoves.ToArray(), specialMoves.ToArray());
        return newMove;
    }

    private Move bishopMove()
    {
        List<Position> maxMoves = new List<Position>();
        List<Position> captureMoves = new List<Position>();
        List<Position> specialMoves = new List<Position>();

        bool leftUpEnd, rightUpEnd, leftDownEnd, rightDownEnd;
        leftUpEnd = rightUpEnd = leftDownEnd = rightDownEnd = false;

        //Check possible moves

        //Check left up movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftUpEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftUpEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //right down movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightDownEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightDownEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //Check right up movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightUpEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightUpEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }
        //Check left down movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftDownEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftDownEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        simulateMoveWithException = simulateMoveWithOccupiedPosition = null;
        Move newMove = new Move(maxMoves.ToArray(), captureMoves.ToArray(), specialMoves.ToArray());
        return newMove;
    }

    private Move queenMove()
    {
        List<Position> maxMoves = new List<Position>();
        List<Position> captureMoves = new List<Position>();
        List<Position> specialMoves = new List<Position>();

        bool upEnd, leftEnd, rightEnd, downEnd;
        upEnd = leftEnd = rightEnd = downEnd = false;

        //Check possible moves

        //Check up movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (upEnd)
                break;
            Position newPos = new Position(piecePosition.xPos, (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    upEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }
        //Check down movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (downEnd)
                break;
            Position newPos = new Position(piecePosition.xPos, (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    downEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //Check left movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), piecePosition.yPos);

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }
        //Check right movement horizontal and vertical
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), piecePosition.yPos);

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        bool leftUpEnd, rightUpEnd, leftDownEnd, rightDownEnd;
        leftUpEnd = rightUpEnd = leftDownEnd = rightDownEnd = false;

        //Check possible moves

        //Check up movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftUpEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftUpEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //right down movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightDownEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightDownEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }
        //Check right up movement
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightUpEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightUpEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        //Check right movement diagonal
        for (int i = 0; i < 8; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftDownEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                     && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftDownEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        simulateMoveWithException = simulateMoveWithOccupiedPosition =  null;
        Move newMove = new Move(maxMoves.ToArray(), captureMoves.ToArray(), specialMoves.ToArray());
        return newMove;
    }

    private Move kingMove()
    {
        List<Position> maxMoves = new List<Position>();
        List<Position> captureMoves = new List<Position>();
        List<Position> specialMoves = new List<Position>();

        bool upEnd, leftEnd, rightEnd, downEnd;
        upEnd = leftEnd = rightEnd = downEnd = false;

        //Check possible moves

        //Check up movement
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (upEnd)
                break;
            Position newPos = new Position(piecePosition.xPos, (piecePosition.yPos + (i + 1)));
            
            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    upEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        //Check down movement
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (downEnd)
                break;
            Position newPos = new Position(piecePosition.xPos, (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    downEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //Check left movement
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), piecePosition.yPos);

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }
        //Check right movement horizontal and vertical
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), piecePosition.yPos);

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        bool leftUpEnd, rightUpEnd, leftDownEnd, rightDownEnd;
        leftUpEnd = rightUpEnd = leftDownEnd = rightDownEnd = false;

        //Check possible moves

        //Check up movement
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftUpEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos))) 
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftUpEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //right down movement
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightDownEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos))) 
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightDownEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }


        }
        //Check right up movement
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (rightUpEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos + (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos))) 
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    rightUpEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        //Check left movement diagonal
        for (int i = 0; i < 1; i++)
        {
            //If a spot in it's way is occupied stop checking for spots beyond
            if (leftDownEnd)
                break;
            Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos - (i + 1)));

            //check if spot is on board
            int xPos = BoardManager.PosCharToInt(newPos.xPos),
                yPos = newPos.yPos;

            if (!(xPos < 1 || xPos > 8 || yPos < 1 || yPos > 8))
            {
                maxMoves.Add(newPos);
                //check if spot is occupied. if so remove it
                BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                    && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                {
                    leftDownEnd = true;
                    maxMoves.Remove(newPos);
                    if (boardPosition.occupantColor != pieceColor || (simulateMoveWithOccupiedPosition != null && (simulateMoveWithOccupiedPosition.Equals(newPos))))
                        captureMoves.Add(newPos);
                }
            }
        }

        //Castle
        if (moves == 0 && !(controller.transform.parent.GetComponent<ColorPiecesManager>().isOnCheck))
        {
            List<Position> castlePositions = new List<Position>();
            Position kingPosition = piecePosition;
            BoardPosition rook1Position = BoardManager.GetBoardTile(new Position(1, kingPosition.yPos));//assuming 1 is always on the left
            BoardPosition rook2Position = BoardManager.GetBoardTile(new Position(8, kingPosition.yPos));//assuming 8 is always on the right
            //left castling
            if (rook1Position.isOccupied && rook1Position.occupantColor == pieceColor && rook1Position.occupantType == PiecesType.Rook 
                                         &&  rook1Position.pieceOccupant.GetMovesAmount() == 0)
            {
                bool isCastlable = true;
                for(int i = 0; i < 3; i++)
                {
                    if (!isCastlable)
                        break;
                    Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) - (i + 1)), (piecePosition.yPos));
                    castlePositions.Add(newPos);
                    if(i != 1)
                    {
                        castlePositions.Remove(newPos);
                    }
                    BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                    if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                        && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                    {
                        castlePositions.Remove(newPos);
                        isCastlable = false;
                    }
                }
                if (isCastlable)
                {
                    foreach(Position pos in castlePositions)
                    {
                        maxMoves.Add(pos);
                        pos.SetAsCastleSpot();
                    }
                }

            }
            //right castling
            if (rook2Position.isOccupied && rook2Position.occupantColor == pieceColor && rook2Position.occupantType == PiecesType.Rook
                                         && rook2Position.pieceOccupant.GetMovesAmount() == 0)
            {
                bool isCastlable = true;
                for (int i = 0; i < 2; i++)
                {
                    if (!isCastlable)
                        break;
                    Position newPos = new Position((BoardManager.PosCharToInt(piecePosition.xPos) + (i + 1)), (piecePosition.yPos));
                    castlePositions.Add(newPos);
                    if (i != 1)
                    {
                        castlePositions.Remove(newPos);
                    }
                    BoardPosition boardPosition = BoardManager.GetBoardTile(newPos);
                    if ((boardPosition.isOccupied || (simulateMoveWithOccupiedPosition != null && simulateMoveWithOccupiedPosition.Equals(newPos)))
                        && (simulateMoveWithException == null || (!simulateMoveWithException.Equals(newPos))))
                    {
                        castlePositions.Remove(newPos);
                        isCastlable = false;
                    }
                }
                if (isCastlable)
                {
                    foreach (Position pos in castlePositions)
                    {
                        maxMoves.Add(pos);
                        pos.SetAsCastleSpot();
                    }
                }

            }


        }

        simulateMoveWithException = simulateMoveWithOccupiedPosition = null;
        Move newMove = new Move(maxMoves.ToArray(), captureMoves.ToArray(), specialMoves.ToArray());
        return newMove;
    }

    public Check IsCheckingOpponent()
    {
        Move newMove = SimulateMove();
        Position[] positions = newMove.captureMoves;
        //Check if the piece can capture the opponents king
        for(int i = 0; i < positions.Length; i++)
        {
            BoardPosition boardPos = BoardManager.GetBoardTile(positions[i]);
            if (boardPos.occupantType == PiecesType.King && boardPos.occupantColor != pieceColor)
                return new Check(positions[i], true);
        }
        return new Check(null, false);
    }
}

public struct Check
{
    public bool isChecking;
    public Position checkPosition;

    public Check(Position _checkPosition, bool _isChecking)
    {
        isChecking = _isChecking;
        checkPosition = _checkPosition;
    }
}


public struct Move
{
    public Position[] movablePositions,
               captureMoves,
               specialMoves;
    public Move(Position[] _movablePositions, Position[] _captureMoves, Position[] _specialMoves)
    {
        movablePositions = _movablePositions;
        captureMoves = _captureMoves;
        specialMoves = _specialMoves;
    }

}

public enum SpecialMoveType
{
    EnPassant, Castle, Promotion
}
public struct SpecialMove
{
    public SpecialMoveType specialMoveType;
    Position specialMovablePosition;
}