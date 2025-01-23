using UnityEngine;

public class LoaderCallBack : MonoBehaviour
{
    private bool isFirstFrame = true;

    private void Update()
    {
        if (isFirstFrame)
        {
            isFirstFrame = false;
            Loader.LoaderCallBack();
        }
    }
}
