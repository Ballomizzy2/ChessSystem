using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSettings : MonoBehaviour
{
    [SerializeField]
    private StartColor startColor;
    [SerializeField]
    public int gameMinutes = 20;

    
    public GameState gameState { get; private set; }

    public void ChangeGameState(GameState _gameState)
    {
        gameState = _gameState;
    }
    private void Awake()
    {
        switch (startColor)
        {
            case StartColor.White:
                firstMoveColor = PieceColor.White;
                break;
            case StartColor.Black:
                firstMoveColor = PieceColor.Black;
                break;
            case StartColor.Random:
                float rand = Random.Range(0.0f, 1.0f);
                if(rand > 0.5f)
                    firstMoveColor = PieceColor.Black;
                else
                    firstMoveColor = PieceColor.White;
                break;
        }
    }
    public PieceColor firstMoveColor { get; private set; } = PieceColor.Black;
}

public enum StartColor { White, Black, Random}
public enum GameState { PreState, PlayState, PostState}
