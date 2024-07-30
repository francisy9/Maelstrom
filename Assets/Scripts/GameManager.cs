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
        isHostsTurn = Random.Range(0, 1) == 0;
        turn = 0;
    }

    private void Start() {
        endTurnButton.onClick.AddListener(() => {
            EndTurn();
        });
        hostPlayer.DrawCard();
        hostPlayer.DrawCard();
        hostPlayer.DrawCard();
        clientPlayer.DrawCard();
        clientPlayer.DrawCard();
        clientPlayer.DrawCard();
        clientPlayer.DrawCard();
    }

    public bool IsHostsTurn() {
        return isHostsTurn;
    }

    private Player GetCurrentPlayerTurn() {
        return isHostsTurn ? hostPlayer : clientPlayer;
    }

    private void EndTurn() {
        isHostsTurn = !isHostsTurn;
        Player playerNextInTurn = GetCurrentPlayerTurn();
        playerNextInTurn.IncrementMaxMana();
        playerNextInTurn.StartTurn();
    }
}
