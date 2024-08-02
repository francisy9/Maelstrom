using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player hostPlayer;
    [SerializeField] private Player clientPlayer;
    [SerializeField] private Button endTurnButton;

    private int turn;
    private bool isHostsTurn;

    private void Awake() {
        Instance = this;
        isHostsTurn = Random.Range(0, 2) == 0;
        turn = 0;
    }

    private void Start() {
        endTurnButton.onClick.AddListener(() => {
            EndTurn();
        });
        hostPlayer.StartGame(isHostsTurn);
        clientPlayer.StartGame(!isHostsTurn);
        GetCurrentPlayerTurn().StartTurn();
    }

    public bool IsHostsTurn() {
        return isHostsTurn;
    }

    public Player GetCurrentPlayerTurn() {
        return isHostsTurn ? hostPlayer : clientPlayer;
    }

    private void EndTurn() {
        GetCurrentPlayerTurn().ResetCardAttacks();
        isHostsTurn = !isHostsTurn;
        Player playerNextInTurn = GetCurrentPlayerTurn();
        playerNextInTurn.StartTurn();
        turn += 1;
    }
}
