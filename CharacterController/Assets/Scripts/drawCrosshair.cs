using System;
using System.Collections;
using UnityEngine;


public class drawCrosshair : MonoBehaviour
{
    public Texture2D crosshair;
    [Range(0.1f, 2f)] public float crosshairScale = 1f;
    private Rect screen;
    private static bool originalOn = true;


    private void Start()
    {
        var crosshairSizeX = crosshair.width * crosshairScale;
        var crosshairSizeY = crosshair.height * crosshairScale;
        screen = new Rect((Screen.width - crosshairSizeX) / 2, (Screen.height - crosshairSizeY) / 2, crosshairSizeX,
            crosshairSizeY);
    }

    private void OnGUI()
    {
        if (originalOn)
        {
            GUI.DrawTexture(screen, crosshair);
        }
    }
}