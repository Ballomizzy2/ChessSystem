using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPromotionMenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject blackIcons;
    [SerializeField]
    private GameObject whiteIcons;
    [SerializeField]
    private GameObject iconsHolder;

    public Position promotionSpot { get; private set; }
    public Position prevPos { get; private set; }


    [SerializeField]
    private Transform[] iconPositions = new Transform[4];

    private BoardManager boardManager;
    private GameManager gameManager;
    private Vector3 originalForwardDirection;

    private void Awake()
    {
        boardManager = FindObjectOfType<BoardManager>();
        //originalForwardDirection = transform.forward;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        HideMenu();
    }
    public void ShowMenu(PieceColor pieceColor, Position position, Position _prevPos)
    {
        promotionSpot = position;
        prevPos = _prevPos;
        iconsHolder.SetActive(true);
        gameManager.MakeAllPiecesUnSelectable(true);
        /*Vector3 vector3 = boardManager.GetBoardTilePos(position);
        transform.position = new Vector3(vector3.x, .25f, vector3.z);
        int iconOrdering = 1;
        if(position.yPos == 8)//if it is at board top edge arrange properly
        {
            transform.forward = -originalForwardDirection;
            iconOrdering = -1;
        }
        if(position.yPos == 1)//if it is at board bottom edge arrange properly
        {
            transform.forward = originalForwardDirection;
            iconOrdering = 1;
        }*/

        if(pieceColor == PieceColor.Black)
        {
            for(int i = 0; i < blackIcons.transform.childCount; i++)
            {
                GameObject gameObject = blackIcons.transform.GetChild(i).gameObject;
                gameObject.SetActive(true);
                //gameObject.transform.up = originalForwardDirection;
                //gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, iconPositions[i].transform.position.z);
                GameObject gameObject2 = whiteIcons.transform.GetChild(i).gameObject;
                gameObject2.SetActive(false);
            }
        }
        if (pieceColor == PieceColor.White)
        {
            for (int i = 0; i < whiteIcons.transform.childCount; i++)
            {
                GameObject gameObject2 = whiteIcons.transform.GetChild(i).gameObject;
                gameObject2.SetActive(true);
                //gameObject2.transform.up = originalForwardDirection;
                //gameObject2.transform.position = new Vector3(gameObject2.transform.position.x, gameObject2.transform.position.y, iconPositions[i].transform.position.z);
                GameObject gameObject = blackIcons.transform.GetChild(i).gameObject;
                gameObject.SetActive(false);
            }
        }
    }

    public void HideMenu()
    {
        iconsHolder.SetActive(false);
        gameManager.MakeAllPiecesUnSelectable(false);
        /*for (int i = 0; i < whiteIcons.transform.childCount; i++)
        {
            GameObject gameObject = whiteIcons.transform.GetChild(i).gameObject;
            if(gameObject)
                gameObject.SetActive(false);
        }
        for (int i = 0; i < blackIcons.transform.childCount; i++)
        {
            GameObject gameObject = blackIcons.transform.GetChild(i).gameObject;
            if(gameObject)
                blackIcons.transform.GetChild(i).gameObject.SetActive(false);
        }*/
    }
}
