using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float BoardPositioningOffset = 0.6f,
                        BoardPositoningIntervalOffset = 1.2f;
    [SerializeField]
    GameObject boardPositionGameObject,
               tileSelectGameObject,
               tileHighlightGameObject,
               captureTileGameObject,
               checkTileGameObject,
               specialTileGameObject;
               
    Position lastSelectedTile;

    public GameObject selectedTile { get; private set; }
    public BoardPosition selectedTileCtrl { get; private set; }

    private List<GameObject> highlightedTiles = new List<GameObject>();
    private List<BoardPosition> highlightedTilesCtrl = new List<BoardPosition>();
    
    private List<GameObject> captureTiles = new List<GameObject>();
    private List<BoardPosition> captureTilesCtrl = new List<BoardPosition>();

    private GameObject checkTile;

    private bool tileSelected;

    [SerializeField]
    GameObject[] boardPositions = new GameObject[64];
    static BoardPosition[,] boardPositionCtrl = new BoardPosition[8,8];

    [SerializeField]
    Color darkTileColor = Color.black, 
          brightTileColor = Color.white;

    public Vector3 GetBoardTilePos(Position position)
    {
        Vector3 vect = (boardPositionCtrl[PosCharToInt(position.xPos) - 1, position.yPos - 1]).transform.position;
        return vect;
    }

    public void SelectTile(Position pos)
    {
        DeSelectTile();
        if(lastSelectedTile == pos)
        {
            lastSelectedTile = null;
            DeSelectTile();
        }
        else if (!tileSelected)
        {
            BoardPosition boardPosition = (boardPositionCtrl[PosCharToInt(pos.xPos) - 1, pos.yPos - 1]);
            Vector3 newPos = boardPosition.transform.position;
            selectedTileCtrl = boardPosition;
            selectedTile = Instantiate(tileSelectGameObject, newPos, Quaternion.identity);
            lastSelectedTile = pos;
            tileSelected = true;
        }
    }

    //Always deselect tile before selecting another
    public void DeSelectTile()
    {
        tileSelected = false;
        Destroy(selectedTile);
        DeHighlightTiles();
        //Destroy(checkTile);
    }

    public void HighlightTiles(Position[] tilePositions, Position[] capturePositions, Position[] specialPositions)
    {
        DeHighlightTiles();
        if (tileSelected)
        {
            for (int i = 0; i < tilePositions.Length; i++)
            {
                if(tilePositions[i] != null)
                {
                    BoardPosition newTile = boardPositionCtrl[PosCharToInt(tilePositions[i].xPos) - 1, tilePositions[i].yPos - 1];
                    newTile.HighlightTile(true);
                    Vector3 newPos = newTile.transform.position;
                    highlightedTiles.Add(Instantiate(tileHighlightGameObject, newPos, Quaternion.identity));
                    highlightedTilesCtrl.Add(newTile);
                    if (tilePositions[i].isCastlePosition)
                        newTile.SetBoardPositionAsCastleSpot(true);
                    if (tilePositions[i].isEnPassantableSpot)
                    {
                        newTile.SetTileAsEnPassantableSpot(true);
                        ColorPiecesManager[] colorPiecesManagers = FindObjectsOfType<ColorPiecesManager>();
                        foreach(ColorPiecesManager colorPieceManager in colorPiecesManagers)
                        {
                            colorPieceManager.HasEnpassantSpot(true);
                        }
                    }
                }
            }

            for (int i = 0; i < capturePositions.Length; i++)
            {
                if(capturePositions[i] != null)
                {
                    BoardPosition newTile = boardPositionCtrl[PosCharToInt(capturePositions[i].xPos) - 1, capturePositions[i].yPos - 1];
                    newTile.MarkCaptureTile(true);
                    newTile.HighlightTile(true);
                    Vector3 newPos = newTile.transform.position;
                    captureTiles.Add(Instantiate(captureTileGameObject, newPos, Quaternion.identity));
                    captureTilesCtrl.Add(newTile);
                    if (capturePositions[i].isEnPassantableCaptureSpot)
                    {
                        newTile.SetTileAsEnPassantCaptureSpot(true);
                        ColorPiecesManager[] colorPiecesManagers = FindObjectsOfType<ColorPiecesManager>();
                        foreach (ColorPiecesManager colorPieceManager in colorPiecesManagers)
                        {
                            colorPieceManager.HasEnpassantSpot(true);
                        }
                    }
                }
            }

            /*for (int i = 0; i < specialPositions.Length; i++)
            {
                if (specialPositions[i] != null)
                {
                    BoardPosition newTile = boardPositionCtrl[PosCharToInt(specialPositions[i].xPos) - 1, specialPositions[i].yPos - 1];
                    //newTile.MarkCaptureTile(true);
                    newTile.HighlightTile(true);
                    Vector3 newPos = newTile.transform.position;
                    captureTiles.Add(Instantiate(specialTileGameObject, newPos, Quaternion.identity));
                    captureTilesCtrl.Add(newTile);
                }
            }*/
        }
    }

    public void DeHighlightTiles()
    {   
        for(int i = 0; i < highlightedTiles.Count; i++)
        {
            BoardPosition boardPosition = highlightedTilesCtrl[i].GetComponent<BoardPosition>();
            boardPosition.HighlightTile(false);
            boardPosition.SetBoardPositionAsCastleSpot(false);
            //boardPosition.SetTileAsEnPassantableSpot(false);
            boardPosition.SetTileAsEnPassantCaptureSpot(false);
            ColorPiecesManager[] colorPiecesManagers = FindObjectsOfType<ColorPiecesManager>();
            /*foreach (ColorPiecesManager colorPieceManager in colorPiecesManagers)
            {
                colorPieceManager.HasEnpassantSpot(false);
            }*/


            Destroy(highlightedTiles[i]);
        }
        highlightedTiles.Clear();
        highlightedTilesCtrl.Clear();

        for (int i = 0; i < captureTiles.Count; i++)
        {
            BoardPosition boardPosition = captureTilesCtrl[i].GetComponent<BoardPosition>();
            boardPosition.MarkCaptureTile(false);
            boardPosition.HighlightTile(false);
            Destroy(captureTiles[i]);
        }

        captureTiles.Clear();
        captureTilesCtrl.Clear();
    }

    public void SetCheckTile(Position tilePosition, bool on)
    {
        if(checkTile)
            Destroy(checkTile);
        BoardPosition newTile = boardPositionCtrl[(PosCharToInt(tilePosition.xPos) - 1), (tilePosition.yPos) - 1];
        Vector3 newPos = newTile.transform.position + new Vector3(0, 0.01f, 0);
        if (on)
        {
            checkTile = Instantiate(checkTileGameObject, newPos, Quaternion.identity);
        }
        else
        {
            Destroy(checkTile);
        }
    }

    public void DestroyCheckTile()
    {
        if (checkTile)
            Destroy(checkTile);
    }

    public static BoardPosition GetBoardTile(Position position)
    {
        BoardPosition boardTile = (boardPositionCtrl[PosCharToInt(position.xPos) - 1, position.yPos - 1]);
        return boardTile;
    }

    static public int PosCharToInt(char c)
    {
        switch (c)
        {
            case 'a':
                return 1;
            case 'b':
                return 2;
            case 'c':
                return 3;
            case 'd':
                return 4;
            case 'e':
                return 5;
            case 'f':
                return 6;
            case 'g':
                return 7;
            case 'h':
                return 8;
            default:
                return 0;

        }
    }

    static public char PosIntToChar(int x)
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
        return c;
    }

    private void Awake()
    {
        for(int i = 0; i < boardPositions.Length; i++)
        {
            //Instantiate all the squares and position properly
            BoardPosition newPosition = Instantiate(boardPositionGameObject, this.transform).GetComponent<BoardPosition>();
            //Store them in the array
            boardPositions[i] = newPosition.gameObject;

        }

        float x = -5, y = -5;
        int boardCount = 1, boardCountX = 1, boardCountY = 1;
        for(x = -5; boardCountX <= 8; x += BoardPositoningIntervalOffset)
        {
            boardCountY = 1;
            for(y = -5; boardCountY <= 8; y += BoardPositoningIntervalOffset)
            {
                boardPositions[boardCount-1].transform.position = new Vector3(x + BoardPositioningOffset, 0,
                                                                              y + BoardPositioningOffset); 

                //change their name to their coordinates
                BoardPosition newBoardPosition = boardPositions[boardCount-1].GetComponent<BoardPosition>();
                newBoardPosition.SetPosition(boardCountX, boardCountY);
                newBoardPosition.gameObject.name = newBoardPosition.GetBoardPositionString();

                //Get theircontrollers and map to the controller array
                boardPositionCtrl[boardCountX - 1, boardCountY - 1] = newBoardPosition.GetBoardPositionController();
               
                boardCountY++;
                boardCount++;
            }
            boardCountX++;

        }

        //change color
        Color color1, color2;
        for(int i = 0; i < 8; i++)
        {
            if(i % 2 == 0)
            {
                color1 = brightTileColor;
                color2 = darkTileColor;
            }
            else
            {
                color1 = darkTileColor;
                color2 = brightTileColor;
            }
            for(int j = 0; j < 8; j++)
            {
                if(j % 2 == 0)
                    boardPositionCtrl[i,j].SetBoardPositionColor(color1);
                else
                    boardPositionCtrl[i, j].SetBoardPositionColor(color2);
            } 
        }

    }
}
