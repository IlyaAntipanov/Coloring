using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperColoring
{
    public static float ColorComparison(Color color1, Color color2)
    {
        var r = Mathf.Abs(color2.r - color1.r);
        var g = Mathf.Abs(color2.g - color1.g);
        var b = Mathf.Abs(color2.b - color1.b);
        var a = Mathf.Abs(color2.a - color1.a);
        return Math.Max(Math.Max(r, g), Math.Max(b, a));
    }

    public static Color ColorTrimming(Color color,float range)
    {
        var r = MathF.Floor(color.r / range) * range + range / 2f;
        var g = MathF.Floor(color.g / range) * range + range / 2f;
        var b = MathF.Floor(color.b / range) * range + range / 2f;
        return new Color(r, g, b);
    }

    public static float ColorDifference(Color color1, Color color2)
    {
        var r = Mathf.Abs(color2.r - color1.r);
        var g = Mathf.Abs(color2.g - color1.g);
        var b = Mathf.Abs(color2.b - color1.b);
        var a = Mathf.Abs(color2.a - color1.a);
        return r + g + b + a;
    }

    public static float Convolution(float[,] W, float[,] brightness)
    {
        return brightness[0, 0] * W[0, 0] + brightness[1, 0] * W[1, 0] + brightness[2, 0] * W[2, 0]
        + brightness[0, 1] * W[0, 1] + brightness[1, 1] * W[1, 1] + brightness[2, 1] * W[2, 1]
        + brightness[0, 2] * W[0, 2] + brightness[1, 2] * W[1, 2] + brightness[2, 2] * W[2, 2];
    }

    public static float ColorBrightness(Color color)
    {
        var Kr = 0.2627f;
        var Kb = 0.0593f;
        var Y = Kr * color.r + (1 - Kr - Kb) * color.g + Kb * color.b;
        return Y;
    }

    public static Texture2D ToTexture2D(this Texture mainTexture)
    {
        Texture2D texture2D = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        RenderTexture renderTexture = new RenderTexture(mainTexture.width, mainTexture.height, 32);
        Graphics.Blit(mainTexture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        return texture2D;
    }
}
