using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button collectionsButton;

    private void Awake () 
    {
        Debug.Log("initializing main menu ui");
        playButton.onClick.AddListener(PlayClick);
        collectionsButton.onClick.AddListener(CollectionsClick);
    }

    private void PlayClick()
    {
        Loader.Load(Loader.Scene.DeckScene);
    }

    private void CollectionsClick()
    {
        Loader.Load(Loader.Scene.CollectionsScene);
    }
}
