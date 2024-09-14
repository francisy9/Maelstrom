using UnityEngine;

public abstract class AnimationBase : MonoBehaviour
{
    protected GameObject currentAnimationObject;
    protected Vector3 startPosition;
    protected Vector3 targetPosition;
    protected float animationStartTime;
    protected float animationDuration = 1f;

    protected virtual void Start()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (currentAnimationObject != null)
        {
            float elapsedTime = Time.time - animationStartTime;
            float t = elapsedTime / animationDuration;

            if (t < 1f)
            {
                UpdateAnimation(t);
            }
            else
            {
                FinishAnimation();
            }
        }
    }

    public virtual void PlayAnimation(Vector3 originPosition, Vector3[] affectedPositions)
    {
        gameObject.SetActive(true);
        startPosition = originPosition;
        targetPosition = affectedPositions[0];
        animationStartTime = Time.time;
        SetupAnimation();
    }

    protected abstract void SetupAnimation();

    protected virtual void UpdateAnimation(float t)
    {
        currentAnimationObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
    }

    protected virtual void FinishAnimation()
    {
        Destroy(currentAnimationObject);
        currentAnimationObject = null;
        gameObject.SetActive(false);
    }
}
