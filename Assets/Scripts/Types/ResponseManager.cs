using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants.Constants;

public enum ResponseStatus
{
    AWAITING_RESPONSE,
    PROCESSING,
    PROCESSED,
    FAILED,
}

public class ResponseManager: MonoBehaviour
{
    private Dictionary<string, ResponseStatus> responseStatuses;

    public void Initialize()
    {
        responseStatuses = new Dictionary<string, ResponseStatus>();
    }

    public ResponseStatus GetResponseStatus(string requestId)
    {
        if (responseStatuses.ContainsKey(requestId))
        {
            return responseStatuses[requestId];
        }
        Debug.LogError("ResponseManager: Response status not found for requestId: " + requestId);
        return ResponseStatus.FAILED;
    }

    public void CreateResponseEntry(string requestId, Action onTimeout) {
        if (responseStatuses.ContainsKey(requestId)) {
            Debug.LogError("ResponseManager: Response already exists for requestId: " + requestId);
            return;
        }
        responseStatuses[requestId] = ResponseStatus.AWAITING_RESPONSE;
        StartCoroutine(CheckResponseProcess(requestId, onTimeout));
        ResponseStatus finalStatus = GetResponseStatus(requestId);
        HandleFinalStatus(requestId, finalStatus, onTimeout);
    }

    private IEnumerator CheckResponseProcess(string requestId, Action onTimeout) {
        float startTime = Time.time;
        while (Time.time - startTime < TIMEOUT_FOR_RESPONSE) {
            ResponseStatus status = GetResponseStatus(requestId);
            if (status == ResponseStatus.PROCESSING) {
                yield return new WaitForSeconds(0.1f);
            } else {
                yield break;
            }
        }
    }

    private void HandleFinalStatus(string requestId, ResponseStatus status, Action onTimeout)
    {
        switch (status)
        {
            case ResponseStatus.PROCESSED:
                break;
            case ResponseStatus.FAILED:
                Debug.LogWarning("ResponseManager: Response set to failed for requestId: " + requestId);
                onTimeout();
                break;
            case ResponseStatus.PROCESSING:
                Debug.LogError("ResponseManager: Response still processing for requestId: " + requestId);
                onTimeout();
                break;
            case ResponseStatus.AWAITING_RESPONSE:
                Debug.LogError("ResponseManager: Never received response for requestId: " + requestId);
                onTimeout();
                break;
            default:
                Debug.LogError("ResponseManager: Invalid response status for requestId: " + requestId);
                onTimeout();
                break;
        }
    }
}