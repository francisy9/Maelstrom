using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public static LineController Instance { get; private set; }
    private LineRenderer lineRenderer;
    void Awake()
    {
        Instance = this;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start() {
        lineRenderer.positionCount = 2;
        Hide();
    }

    public void SetAttackLine(Vector3 fromCardPos, Vector3 toCardPos) {
        Debug.Log($"set attack line called{fromCardPos} {toCardPos}");
        lineRenderer.SetPosition(0, fromCardPos);
        lineRenderer.SetPosition(1, toCardPos);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}
