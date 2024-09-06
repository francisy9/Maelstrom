using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.MultipleMatch
{
    public class CellGUI : MonoBehaviour
    {
        public MatchController matchController;
        public CellValue cellValue;

        [Header("GUI References")]
        public Image cellImage;
        public Button cellButton;

        [Header("Diagnostics")]
        [ReadOnly, SerializeField] internal NetworkIdentity playerIdentity;

        public void Awake()
        {
            matchController.MatchCells.Add(cellValue, this);
        }

        [ClientCallback]
        public void MakePlay()
        {
            if (matchController.currentPlayer.isLocalPlayer)
                matchController.CmdMakePlay(cellValue);
        }

        [ClientCallback]
        public void SetPlayer(NetworkIdentity playerIdentity)
        {
            if (playerIdentity != null)
            {
                this.playerIdentity = playerIdentity;
                cellImage.color = this.playerIdentity.isLocalPlayer ? Color.blue : Color.red;
                cellButton.interactable = false;
            }
            else
            {
                this.playerIdentity = null;
                cellImage.color = Color.white;
                cellButton.interactable = true;
            }
        }
    }
}