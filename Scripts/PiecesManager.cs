using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    [SerializeField]
    public Transform BlackPiecesContainer, WhitePiecesContainer;
    [Space]
    public GameObject BlackJailGO;
    public GameObject WhiteJailGO;

    [SerializeField]
    private GameObject pawnBlackGO, pawnWhiteGO, 
                       rookBlackGO, rookWhiteGO,
                       knightBlackGO, knightWhiteGO,
                       bishopBlackGO, bishopWhiteGO,
                       queenBlackGO, queenWhiteGO,
                       kingBlackGO, kingWhiteGO;

    private const float pieceYOffset = 0.1f;

    [SerializeField]
    List<Piece> BlackPieces = new List<Piece>();

    [Space]
    [SerializeField]
    List<Piece> WhitePieces = new List<Piece>();

    private BoardManager boardManager;

    private void Awake()
    {
        boardManager = FindObjectOfType<BoardManager>();
    }

    private void Start()
    {
        InitPieces();
        //SetPieces();
    }

    private void InitPieces()
    {
        //init black pieces
        PieceColor pieceColor;
        pieceColor = PieceColor.Black;
        BlackPieces.Clear();
        for(int i = 0; i < 16; i++)
        {
            Piece newPiece = new Piece();
            PieceController pieceController;
            GameObject pieceToInstantiate = null;
            BlackPieces.Add(newPiece);
            switch (i)
            {
                case 0:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("a2"));
                    break;                                          
                case 1:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("b2"));
                    break;                                          
                case 2:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("c2"));
                    break;                                         
                case 3:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("d2"));
                    break;                                          
                case 4:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("e2"));
                    break;                                         
                case 5:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("f2"));
                    break;                                         
                case 6:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("g2"));
                    break;                                         
                case 7:
                    pieceToInstantiate = Instantiate(pawnBlackGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("h2"));
                    break;                                          
                case 8:
                    pieceToInstantiate = Instantiate(rookBlackGO, transform);
                    newPiece.SetValues(PiecesType.Rook, pieceColor, pieceToInstantiate, new Position("a1"));
                    break;                                          
                case 9:
                    pieceToInstantiate = Instantiate(rookBlackGO, transform);
                    newPiece.SetValues(PiecesType.Rook, pieceColor, pieceToInstantiate, new Position("h1"));
                    break;
                case 10:
                    pieceToInstantiate = Instantiate(knightBlackGO, transform);
                    newPiece.SetValues(PiecesType.Knight, pieceColor, pieceToInstantiate, new Position("b1"));
                    break;
                case 11:
                    pieceToInstantiate = Instantiate(knightBlackGO, transform);
                    newPiece.SetValues(PiecesType.Knight, pieceColor, pieceToInstantiate, new Position("g1"));
                    break;
                case 12:
                    pieceToInstantiate = Instantiate(bishopBlackGO, transform);
                    newPiece.SetValues(PiecesType.Bishop, pieceColor, pieceToInstantiate, new Position("c1"));
                    break;
                case 13:
                    pieceToInstantiate = Instantiate(bishopBlackGO, transform);
                    newPiece.SetValues(PiecesType.Bishop, pieceColor, pieceToInstantiate, new Position("f1"));
                    break;
                case 14:
                    pieceToInstantiate = Instantiate(queenBlackGO, transform);
                    newPiece.SetValues(PiecesType.Queen, pieceColor, pieceToInstantiate, new Position("d1"));
                    break;
                case 15:
                    pieceToInstantiate = Instantiate(kingBlackGO, transform);
                    newPiece.SetValues(PiecesType.King, pieceColor, pieceToInstantiate, new Position("e1"));
                    break;
                default:
                    break;
            }
            pieceToInstantiate.transform.SetParent(BlackPiecesContainer);
            if(pieceToInstantiate.GetComponent<PieceController>() != null)
            {
                pieceController = pieceToInstantiate.GetComponent<PieceController>();
                pieceController.InitPiece(newPiece);
                newPiece.controller = pieceController;
            }
            
        }

        //init white pieces
        pieceColor = PieceColor.White;
        WhitePieces.Clear();
        for (int i = 0; i < 16; i++)
        {
            Piece newPiece = new Piece();
            PieceController pieceController;
            GameObject pieceToInstantiate = null;
            WhitePieces.Add(newPiece);
            switch (i)
            {
                case 0:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("a7"));
                    break;
                case 1:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("b7"));
                    break;
                case 2:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("c7"));
                    break;
                case 3:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("d7"));
                    break;
                case 4:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("e7"));
                    break;
                case 5:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("f7"));
                    break;
                case 6:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("g7"));
                    break;
                case 7:
                    pieceToInstantiate = Instantiate(pawnWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Pawn, pieceColor, pieceToInstantiate, new Position("h7"));
                    break;
                case 8:
                    pieceToInstantiate = Instantiate(rookWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Rook, pieceColor, pieceToInstantiate, new Position("a8"));
                    break;
                case 9:
                    pieceToInstantiate = Instantiate(rookWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Rook, pieceColor, pieceToInstantiate, new Position("h8"));
                    break;
                case 10:
                    pieceToInstantiate = Instantiate(knightWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Knight, pieceColor, pieceToInstantiate, new Position("b8"));
                    break;
                case 11:
                    pieceToInstantiate = Instantiate(knightWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Knight, pieceColor, pieceToInstantiate, new Position("g8"));
                    break;
                case 12:
                    pieceToInstantiate = Instantiate(bishopWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Bishop, pieceColor, pieceToInstantiate, new Position("c8"));
                    break;
                case 13:
                    pieceToInstantiate = Instantiate(bishopWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Bishop, pieceColor, pieceToInstantiate, new Position("f8"));
                    break;
                case 14:
                    pieceToInstantiate = Instantiate(queenWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Queen, pieceColor, pieceToInstantiate, new Position("d8"));
                    break;
                case 15:
                    pieceToInstantiate = Instantiate(kingWhiteGO, transform);
                    newPiece.SetValues(PiecesType.King, pieceColor, pieceToInstantiate, new Position("e8"));
                    break;
                default:
                    break;
            }
            pieceToInstantiate.transform.SetParent(WhitePiecesContainer);
            if (pieceToInstantiate.GetComponent<PieceController>() != null)
            {
                pieceController = pieceToInstantiate.GetComponent<PieceController>();
                pieceController.InitPiece(newPiece);
                newPiece.controller = pieceController;
            }
            SetPieces();
        }

    }
    private void SetPieces()
    {
        //Set Black Pieces
        for(int i = 0; i < BlackPieces.Count; i++)
        {
            BoardPosition boardTile = BoardManager.GetBoardTile(BlackPieces[i].startPosition);
            boardTile.OccupyBoardTile(PieceColor.Black, BlackPieces[i].pieceType, BlackPieces[i].controller);

            Vector3 boardTilePos = boardTile.transform.position;
            BlackPieces[i].pieceGameObject.transform.position = new Vector3(boardTilePos.x, boardTilePos.y + pieceYOffset, boardTilePos.z);
        }
        //Set White Pieces
        for(int i = 0; i < WhitePieces.Count; i++)
        {
            BoardPosition boardTile = BoardManager.GetBoardTile(WhitePieces[i].startPosition);
            boardTile.OccupyBoardTile(PieceColor.White, WhitePieces[i].pieceType, WhitePieces[i].controller);

            Vector3 boardTilePos = boardTile.transform.position;  
            WhitePieces[i].pieceGameObject.transform.position = new Vector3(boardTilePos.x, boardTilePos.y + pieceYOffset, boardTilePos.z);
        }
    }

    public void PromotePawn(BoardPosition positionOfPawn, PiecesType pieceToPromoteTo)
    {
        PieceColor pieceColor;
        pieceColor = positionOfPawn.occupantColor;
        GameObject previousPawnPiece = positionOfPawn.pieceOccupant.gameObject;
        if (pieceColor == PieceColor.Black)
        {
            BlackPieces.Remove(positionOfPawn.pieceOccupant.GetPieceClass());
            Piece newPiece = new Piece();
            PieceController pieceController;
            GameObject pieceToInstantiate = null;
            BlackPieces.Add(newPiece);
            Position position = positionOfPawn.GetBoardPosition();
            switch (pieceToPromoteTo)
            {
                case PiecesType.Queen:
                    pieceToInstantiate = Instantiate(queenBlackGO, transform);
                    newPiece.SetValues(PiecesType.Queen, pieceColor, pieceToInstantiate, position);
                    break;
                case PiecesType.Bishop:
                    pieceToInstantiate = Instantiate(bishopBlackGO, transform);
                    newPiece.SetValues(PiecesType.Bishop, pieceColor, pieceToInstantiate, position);
                    break;
                case PiecesType.Knight:
                    pieceToInstantiate = Instantiate(knightBlackGO, transform);
                    newPiece.SetValues(PiecesType.Knight, pieceColor, pieceToInstantiate, position);
                    break;
                case PiecesType.Rook:
                    pieceToInstantiate = Instantiate(rookBlackGO, transform);
                    newPiece.SetValues(PiecesType.Rook, pieceColor, pieceToInstantiate, position);
                    break;
                default:
                    break;
            }
            pieceToInstantiate.transform.SetParent(BlackPiecesContainer);
            if (pieceToInstantiate.GetComponent<PieceController>() != null)
            {
                pieceController = pieceToInstantiate.GetComponent<PieceController>();
                pieceController.InitPiece(newPiece);
                newPiece.controller = pieceController;
            }
            BoardPosition boardTile = BoardManager.GetBoardTile(position);
            boardTile.OccupyBoardTile(PieceColor.Black, newPiece.pieceType, newPiece.controller);

            Vector3 boardTilePos = boardTile.transform.position;
            newPiece.pieceGameObject.transform.position = new Vector3(boardTilePos.x, boardTilePos.y + 0.2f, boardTilePos.z);
        }
        if (pieceColor == PieceColor.White)
        {
            WhitePieces.Remove(positionOfPawn.pieceOccupant.GetPieceClass());
            Piece newPiece = new Piece();
            PieceController pieceController;
            GameObject pieceToInstantiate = null;
            WhitePieces.Add(newPiece);
            Position position = positionOfPawn.GetBoardPosition();
            switch (pieceToPromoteTo)
            {
                case PiecesType.Queen:
                    pieceToInstantiate = Instantiate(queenWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Queen, pieceColor, pieceToInstantiate, position);
                    break;
                case PiecesType.Bishop:
                    pieceToInstantiate = Instantiate(bishopWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Bishop, pieceColor, pieceToInstantiate, position);
                    break;
                case PiecesType.Knight:
                    pieceToInstantiate = Instantiate(knightWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Knight, pieceColor, pieceToInstantiate, position);
                    break;
                case PiecesType.Rook:
                    pieceToInstantiate = Instantiate(rookWhiteGO, transform);
                    newPiece.SetValues(PiecesType.Rook, pieceColor, pieceToInstantiate, position);
                    break;
                default:
                    break;

            }
            pieceToInstantiate.transform.SetParent(WhitePiecesContainer);
            if (pieceToInstantiate.GetComponent<PieceController>() != null)
            {
                pieceController = pieceToInstantiate.GetComponent<PieceController>();
                pieceController.InitPiece(newPiece);
                newPiece.controller = pieceController;
            }
            BoardPosition boardTile = BoardManager.GetBoardTile(position);
            boardTile.OccupyBoardTile(PieceColor.White, newPiece.pieceType, newPiece.controller);

            Vector3 boardTilePos = boardTile.transform.position;
            newPiece.pieceGameObject.transform.position = new Vector3(boardTilePos.x, boardTilePos.y + 0.2f, boardTilePos.z);
        }
        Destroy(previousPawnPiece);
    }
    
    public void SetColorOnCheck(PieceColor color)
    {
        switch (color)
        {
            case PieceColor.White:
                WhitePiecesContainer.GetComponent<ColorPiecesManager>().SetKingCheckStatus(true);
                break;
            case PieceColor.Black:
                BlackPiecesContainer.GetComponent<ColorPiecesManager>().SetKingCheckStatus(true);
                break;
            default:
                break;
        }
    }

}
public enum PiecesType
{
    Pawn, Rook, Knight, Bishop, Queen, King
};

public enum PieceColor {Black, White, Empty};
