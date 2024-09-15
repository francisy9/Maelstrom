using UnityEngine;
using System.Collections.Generic;
using System;
public abstract class ProjectileAnimationBase : MonoBehaviour
{
    protected List<GameObject> currentAnimationObjects = new List<GameObject>();
    protected Vector3 startPosition;
    protected Vector3[] targetPositions;
    protected float animationStartTime;
    protected float animationDuration = 1f;
    protected GameObject projectilePrefab;
    protected Action onAnimationFinished;

    protected virtual void Start()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (currentAnimationObjects.Count > 0)
        {
            float elapsedTime = Time.time - animationStartTime;
            float t = elapsedTime / animationDuration;
            for (int i = 0; i < currentAnimationObjects.Count; i++)
            {
                if (t < 1f)
                {
                    UpdateAnimation(t, i);
                }
                else
                {
                    FinishAnimations();
                }
            }
        }
    }

    public virtual void PlayAnimation(Vector3 originPosition, Vector3[] affectedPositions, Action onAnimationFinished)
    {
        gameObject.SetActive(true);
        startPosition = originPosition;
        targetPositions = affectedPositions;
        animationStartTime = Time.time;
        this.onAnimationFinished = onAnimationFinished;

        for (int i = 0; i < targetPositions.Length; i++)
        {
            GameObject projectile = Instantiate(GetProjectilePrefab());
            projectile.SetActive(true);
            projectile.transform.position = startPosition;
            currentAnimationObjects.Add(projectile);
        }
    }

    protected abstract GameObject GetProjectilePrefab();
    protected abstract GameObject CreateProjectilePrefab();

    protected virtual void UpdateAnimation(float t, int i)
    {
        currentAnimationObjects[i].transform.position = Vector3.Lerp(startPosition, targetPositions[i], t);
    }

    protected virtual void FinishAnimations()
    {
        foreach (GameObject projectile in currentAnimationObjects)
        {
            Destroy(projectile);
        }
        currentAnimationObjects.Clear();
        onAnimationFinished?.Invoke();
        gameObject.SetActive(false);
    }
}
