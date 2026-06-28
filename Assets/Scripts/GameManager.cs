using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Playing,
    Won,
    Lost
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.Playing;
    [SerializeField] private List<Entity> players;
    [SerializeField] private Entity currentBoss;
    [SerializeField] GameObject winText;
    [SerializeField] GameObject loseText;

    void Start()
    {
        winText.SetActive(false);
        loseText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameState();
    }

    private void CheckGameState()
    {
        if (currentGameState != GameState.Playing) return;

        if (players[0].currentHealth <= 0 && players[1].currentHealth <= 0)
        {
            SetLoseState();
        }
        else if (currentBoss.currentHealth <= 0)
        {
            SetWinState();
        }
    }

    private void SetWinState()
    {
        currentGameState = GameState.Won;

        winText.SetActive(true);

        FreezeGame();
    }

    private void SetLoseState()
    {
        currentGameState = GameState.Lost;

        loseText.SetActive(true);

        FreezeGame();
    }

    private void FreezeGame()
    {
        Time.timeScale = 0f;
    }
}
