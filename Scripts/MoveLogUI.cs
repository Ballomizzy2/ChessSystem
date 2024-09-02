using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MoveLogUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI LogNumber;
    [SerializeField]
    public TextMeshProUGUI FirstMoveLog;
    [SerializeField]
    public TextMeshProUGUI SecondMoveLog;
    [SerializeField]
    private Image gameLogImage;
    [SerializeField]
    private Color32 darkColor = new Color32(100, 100, 100, 225),
                  lightColor = new Color32(70, 70, 70, 225);

    public void SetLogDetails(int logNumber, string firstMoverLog, string secondMoverLog)
    {
        LogNumber.text = logNumber.ToString() + ".";
        FirstMoveLog.text = firstMoverLog;
        SecondMoveLog.text = secondMoverLog;
        if (logNumber % 2 == 0)
            gameLogImage.color = darkColor;
        else
            gameLogImage.color = lightColor;
    }
}
