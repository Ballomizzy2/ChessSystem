using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class MoveLogsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject logPrefab;

    [HideInInspector]
    public MoveLogUI moveLogUI;
    private GameObject newLogGO;

    private int noOfMoves = 0;
    private bool isFirstMove = true;

    [HideInInspector]
    public TextMeshProUGUI lastEdittedLogText;
    [HideInInspector]
    public MoveLogInfo lastInfo;
    [SerializeField]
    private ScrollView scrollView;

    public void MakeMoveLog(MoveLogInfo info)
    {
        noOfMoves++;
        lastInfo = info;
        if(noOfMoves % 2 == 1)
        {
            newLogGO = Instantiate(logPrefab, transform);
            moveLogUI = newLogGO.GetComponent<MoveLogUI>();
            isFirstMove = true;
        }
        else
            isFirstMove = false;

        if (isFirstMove)
        {
            moveLogUI.SetLogDetails(Mathf.CeilToInt((float)noOfMoves / 2), MoveLogText(info), "");
            lastEdittedLogText = moveLogUI.FirstMoveLog;
        }
        else
        {
            moveLogUI.SetLogDetails(Mathf.CeilToInt((float)noOfMoves / 2), moveLogUI.FirstMoveLog.text, MoveLogText(info));
            lastEdittedLogText = moveLogUI.SecondMoveLog;
        }

        newLogGO.transform.SetAsFirstSibling();
        //scrollView.ScrollTo(newLogGO.GetComponent<UIDocume>());
        //scrollView.ScrollTo(.ElementAt(scrollView.childCount -1));

    }

    public class MoveLogInfo
    {
        public PiecesType piece, promotePiece;
        public Position oldPos;
        public Position newPos;
        public bool isCheck, isCheckMate, isCapture, isPromote, isKingSideCastle, isQueenSideCastle;
        
        public MoveLogInfo(PiecesType piece, Position oldPos, Position newPos, bool isCheck, bool isCheckMate, bool isCapture, bool isPromote, PiecesType promotePiece, bool isKingSideCastle, bool isQueenSideCastle)
        {
            this.piece = piece;
            this.oldPos = oldPos;
            this.newPos = newPos;
            this.isCheck = isCheck;
            this.isCapture = isCapture;
            this.isCheckMate = isCheckMate;
            this.isPromote = isPromote;
            this.promotePiece = promotePiece;
            this.isKingSideCastle = isKingSideCastle;
            this.isQueenSideCastle = isQueenSideCastle;
        }
    }

    public string MoveLogText(MoveLogInfo info)
    {
        string promoteCode, captureCode, checkCode, checkMateCode, castleCode;
        promoteCode = captureCode = checkCode = checkMateCode = castleCode = "";
        if (info.isPromote)
        {
            string c = "";
            switch (info.promotePiece)
            {
                case PiecesType.Queen:
                    c = "Q";
                    break;
                case PiecesType.Bishop:
                    c = "B";
                    break;
                case PiecesType.Knight:
                    c = "N";
                    break;
                case PiecesType.Rook:
                    c = "R";
                    break;
            }
            promoteCode = ("=" + c);
        }
        if (info.isCapture)
            captureCode = "x";
        if(info.isCheck)
            checkCode = "+";
        if (info.isKingSideCastle)
            castleCode = "O-O";
        if (info.isQueenSideCastle)
            castleCode = "O-O-O";


        switch (info.piece)
        {
            case PiecesType.Pawn:
                if(info.isCapture)
                    return ("" + info.oldPos.xPos + captureCode + info.newPos.ToString() + promoteCode + checkCode);
                else
                    return ("" + info.newPos.ToString() + promoteCode + checkCode);
            case PiecesType.King:
                if(castleCode == "")
                    return("K" + captureCode + info.newPos.ToString() + checkCode);
                else
                    return(castleCode);
            case PiecesType.Queen:
                return ("Q" + captureCode + info.newPos.ToString() + checkCode);
            case PiecesType.Bishop:
                return ("B" + captureCode + info.newPos.ToString() + checkCode);
            case PiecesType.Knight:
                return("N" + captureCode + info.newPos.ToString() + checkCode);
            case PiecesType.Rook:
                return ("R" + captureCode + info.newPos.ToString() + checkCode);
            default:
                return null;
                 
        }
    }
}
