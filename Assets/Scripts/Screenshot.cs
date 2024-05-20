using UnityEngine;

public class Screenshot : MonoBehaviour{
    void Update(){
        if (Input.GetKeyDown(KeyCode.K)) {
            ScreenCapture.CaptureScreenshot("Screenshot.png", 2);
        }
    }
}
