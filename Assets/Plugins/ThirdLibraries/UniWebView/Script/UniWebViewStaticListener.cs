using UnityEngine;

public class UniWebViewStaticListener {
    public static void DebugLog(string value) {
        var payload = JsonUtility.FromJson<UniWebViewNativeResultPayload>(value);
        switch (payload.resultCode) {
            case "0":
                Debug.Log(payload.data);
                break;
            case "1":
                Debug.Log(payload.data);
                break;
            case "2":
                Debug.LogWarning(payload.data);
                break;
            case "3":
                Debug.LogError(payload.data);
                break;
            case "4":
                Debug.LogError(payload.data);
                break;
            default:
                Debug.Log(payload.data);
                break;
        }
    }
}