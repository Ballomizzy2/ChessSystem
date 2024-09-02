using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPromotionIconController : MonoBehaviour
{
    [SerializeField]
    private PiecesType pieceType;

    private PiecesManager pieceManager;
    private PawnPromotionMenuScript pawnPromotionMenuScript;
    private MoveLogsManager moveLogsManager;

    private void Awake()
    {
        pieceManager = FindObjectOfType<PiecesManager>();
        pawnPromotionMenuScript = FindObjectOfType<PawnPromotionMenuScript>();
        moveLogsManager = FindObjectOfType<MoveLogsManager>();
    }

    public void Promote()
    {
        pieceManager.PromotePawn(BoardManager.GetBoardTile(pawnPromotionMenuScript.promotionSpot), pieceType);
        MoveLogsManager.MoveLogInfo info = new MoveLogsManager.MoveLogInfo(PiecesType.Pawn, pawnPromotionMenuScript.prevPos, pawnPromotionMenuScript.promotionSpot, false, false, false, true, PiecesType.King, false, false);
        moveLogsManager.MakeMoveLog(info);
        pawnPromotionMenuScript.HideMenu();
    }
}
