using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject referenceUIObject;

    public GameObject table;
    public Sprite tableSprite;

    private int screenshot = 0;

    void Start()
    {
        tableSprite = table.GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.ScrollLock)) {
            string mainFolder = "screenshots/";
            string screenshotName = screenshot.ToString() + ".png";

            ScreenCapture.CaptureScreenshot(mainFolder + screenshot.ToString() + ".png");
            screenshot++;
        }

        if (Input.GetKeyDown(KeyCode.Pause)) {
            if (Time.timeScale == 0.0f) {
                Time.timeScale = 1.0f;
            } else {
                Time.timeScale = 0.0f;
            }
        }

        this.GetComponent<Camera>().orthographicSize = 6.183526f / this.GetComponent<Camera>().aspect;
    }
}
