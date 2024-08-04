using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Player playerOne;
    private bool p1Assigned = false;
    [SerializeField] private Player playerTwo;
    private bool p2Assigned = false;
    [SerializeField] private Button endTurnButton;

    private int turn;
    private bool isP1Turn;

    public override void OnStartServer() {
        Instance = this;
        isP1Turn = Random.Range(0, 2) == 0;
        turn = 0;
        base.OnStartServer();
        endTurnButton.onClick.AddListener(() => {
            EndTurn();
        });
    }

    public Player GetPlayer() {
        if (!p1Assigned) {
            p1Assigned = true;
            return playerOne;
        } else if (!p2Assigned) {
            p2Assigned = true;
            return playerTwo;
        } else {
            Debug.LogError("Too many attempted connections");
            return null;
        }
    }

    public void StartGame() {
        Debug.Log("start game");
        
        playerOne.CmdDrawCards(1);
        playerTwo.CmdDrawCards(3);
        // if (isP1Turn) {
        //     playerOne.CmdDrawCards(3);
        //     playerTwo.DrawCardsFromDeck(4);
        // } else {
        //     playerOne.CmdDrawCards(4);
        //     playerTwo.DrawCardsFromDeck(3);
        // }
        // GetCurrentPlayerTurn().StartTurn();
    }

    public bool IsP1Turn() {
        return isP1Turn;
    }

    public Player GetCurrentPlayerTurn() {
        return isP1Turn ? playerOne : playerTwo;
    }

    private void EndTurn() {
        Debug.Log("end turn called");
        GetCurrentPlayerTurn().ResetCardAttacks();
        isP1Turn = !isP1Turn;
        Player playerNextInTurn = GetCurrentPlayerTurn();
        playerNextInTurn.StartTurn();
        turn += 1;
    }
}
