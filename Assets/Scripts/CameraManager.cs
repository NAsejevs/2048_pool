using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject referenceUIObject;

    public GameObject table;
    public Sprite tableSprite;

    void Start()
    {
        tableSprite = table.GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        this.GetComponent<Camera>().orthographicSize = 6.183526f / this.GetComponent<Camera>().aspect;
    }
}
