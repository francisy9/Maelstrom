using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants.Constants;

public enum RequestStatus
{
    ACKNOWLEDGED,
    NEVER_RECEIVED_ACKNOWLEDGEMENT,
    FAILED,
    SENT,
}

public class RequestManager: MonoBehaviour
{
    private Dictionary<string, RequestStatus> requestStatuses;
    private ResponseManager responseManager;

    public void Initialize(ResponseManager responseManager)
    {
        this.responseManager = responseManager;
        requestStatuses = new Dictionary<string, RequestStatus>();
    }

    public RequestStatus GetRequestStatus(string requestId)
    {
        if (requestStatuses.ContainsKey(requestId))
        {
            return requestStatuses[requestId];
        }
        Debug.LogError("RequestManager: Request status not found for requestId: " + requestId);
        return RequestStatus.NEVER_RECEIVED_ACKNOWLEDGEMENT;
    }

    public void SetRequestStatus(string requestId, RequestStatus status) {
        if (requestStatuses.ContainsKey(requestId)) {
            requestStatuses[requestId] = status;
        } else {
            Debug.LogError("RequestManager: Request status not found for requestId: " + requestId);
        }
    }

    public void SendRequestAndCreateResponseEntry(string requestId, Action requestCall, Action onTimeout) {
        if (requestStatuses.ContainsKey(requestId)) {
            Debug.LogError("RequestManager: Request already exists for requestId: " + requestId);
            return;
        }
        requestStatuses[requestId] = RequestStatus.SENT;
        requestCall();
        StartCoroutine(CheckAcknowledgement(requestId, requestCall, onTimeout));
        RequestStatus status = GetRequestStatus(requestId);
        HandleFinalStatus(requestId, status, onTimeout);
    }

    private IEnumerator CheckAcknowledgement(string requestId, Action resendRequest, Action onTimeout)
    {
        const int maxRetries = MAX_RETRIES;
        float timeoutPerAttempt = TIMEOUT_FOR_ACKNOWLEDGEMENT / maxRetries;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            float startTime = Time.time;
            while (Time.time - startTime < timeoutPerAttempt)
            {
                RequestStatus requestStatus = GetRequestStatus(requestId);
                if (requestStatus == RequestStatus.SENT) {
                    yield return new WaitForSeconds(0.1f);
                } else {
                    yield break;
                }
            }

            if (attempt < maxRetries - 1)
            {
                Debug.Log($"Retry attempt {attempt + 1} for requestId: {requestId}");
                resendRequest();
            }
        }

        Debug.LogWarning($"Request {requestId} failed after {maxRetries} attempts");
        SetRequestStatus(requestId, RequestStatus.NEVER_RECEIVED_ACKNOWLEDGEMENT);
    }

    private void HandleFinalStatus(string requestId, RequestStatus status, Action onTimeout) {
        switch (status)
        {
            case RequestStatus.NEVER_RECEIVED_ACKNOWLEDGEMENT:
                Debug.LogError($"Request {requestId} never received acknowledgement");
                onTimeout();
                break;
            case RequestStatus.ACKNOWLEDGED:
                responseManager.CreateResponseEntry(requestId, onTimeout);
                break;
            case RequestStatus.SENT:
                Debug.LogError($"Request {requestId} is still in SENT status - coroutine didn't set status to failed");
                break;
            case RequestStatus.FAILED:
                Debug.LogError($"Request {requestId} failed");
                onTimeout();
                break;
            default:
                Debug.LogError($"Unknown status {status} for request {requestId}");
                onTimeout();
                break;
        }
    }
}