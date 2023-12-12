using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class StateGame : MonoBehaviour, IStateMenu
{
    public ButtonEvent ButtonFinishedDrawing;
    public ButtonEvent ButtonSendDrawing;

    public ButtonEvent ButtonEraser;
    public ButtonEvent ButtonBrush;
    public ButtonEvent ButtonSpray;
    public ButtonEvent ButtonFeltPen;

    public ButtonEvent ButtonHeartsTexture;
    public ButtonEvent ButtonStarsTexture;
    public ButtonEvent ButtonPositionMarksTexture;
    public PaletteButton PaletteButton;

    public event Action<Type> SetState;
    public ColoringCS Coloring;

    public Texture2D MaskBrush;
    public Texture2D MaskSpray;
    public Texture2D MaskEraser;
    public Texture2D MaskFeltPen;

    public int SizeMaskBrush = 10;
    public int SizeMaskSpray = 10;
    public int SizeMaskEraser = 10;
    public int SizeMaskFeltPen = 10;

    public Texture2D HeartsTexture;
    public Texture2D StarsTexture;
    public Texture2D PositionMarksTexture;

    private bool IsDrawTexture = false;
    private TypeBrush _typeBrush;
    private TypeBrush typeBrush
    {
        get => _typeBrush; set
        {
            _typeBrush = value;
            switch (value)
            {
                case TypeBrush.Brush:
                    SetBrush();
                    break;
                case TypeBrush.Eraser:
                    SetEraser();
                    break;
                case TypeBrush.Spray:
                    SetSpray();
                    break;
                case TypeBrush.FeltPen:
                    SetFeltPen();
                    break;
            }
        }
    }
    public void EndState()
    {
        PaletteButton.ColorChange -= PaletteButton_ColorChange;

        ButtonEraser.Down -= ButtonEraser_Down;
        ButtonBrush.Down -= ButtonBrush_Down;
        ButtonSpray.Down -= ButtonSpray_Down;
        ButtonFeltPen.Down -= ButtonFeltPen_Down;

        ButtonHeartsTexture.Down -= ButtonHeartsTexture_Down;
        ButtonStarsTexture.Down -= ButtonStarsTexture_Down;
        ButtonPositionMarksTexture.Down -= ButtonPositionMarksTexture_Down;
    }

    public void StartState()
    {
        PaletteButton.ColorChange += PaletteButton_ColorChange;

        ButtonEraser.Down += ButtonEraser_Down;
        ButtonBrush.Down += ButtonBrush_Down;
        ButtonSpray.Down += ButtonSpray_Down;
        ButtonFeltPen.Down += ButtonFeltPen_Down;

        ButtonHeartsTexture.Down += ButtonHeartsTexture_Down;
        ButtonStarsTexture.Down += ButtonStarsTexture_Down;
        ButtonPositionMarksTexture.Down += ButtonPositionMarksTexture_Down;

        typeBrush = TypeBrush.Brush;
    }

    private void ButtonPositionMarksTexture_Down()
    {
        IsDrawTexture = true;
        Coloring.IsDrawTexture = IsDrawTexture;
        Coloring.TextureDraw = PositionMarksTexture;
        Coloring.SetTexture();
    }

    private void ButtonStarsTexture_Down()
    {
        IsDrawTexture = true;
        Coloring.IsDrawTexture = IsDrawTexture;
        Coloring.TextureDraw = StarsTexture;
        Coloring.SetTexture();
    }

    private void ButtonHeartsTexture_Down()
    {
        IsDrawTexture = true;
        Coloring.IsDrawTexture = IsDrawTexture;
        Coloring.TextureDraw = HeartsTexture;
        Coloring.SetTexture();
    }

    private void SetFeltPen()
    {
        Coloring.MaskBrush = MaskFeltPen;
        Coloring.SetMask();
        Coloring.ActiveColor = PaletteButton.ActiveColor;
        Coloring.SizeBrush = SizeMaskFeltPen;
        Coloring.IsDrawTexture = IsDrawTexture;
    }

    private void SetSpray()
    {
        Coloring.MaskBrush = MaskSpray;
        Coloring.SetMask();
        Coloring.ActiveColor = PaletteButton.ActiveColor;
        Coloring.SizeBrush = SizeMaskSpray;
        Coloring.IsDrawTexture = IsDrawTexture;
    }

    private void SetBrush()
    {
        Coloring.MaskBrush = MaskBrush;
        Coloring.SetMask();
        Coloring.ActiveColor = PaletteButton.ActiveColor;
        Coloring.SizeBrush = SizeMaskBrush;
        Coloring.IsDrawTexture = IsDrawTexture;
    }

    private void SetEraser()
    {
        Coloring.MaskBrush = MaskEraser;
        Coloring.SetMask();
        Coloring.ActiveColor = Color.white;
        Coloring.SizeBrush = SizeMaskEraser;
        Coloring.IsDrawTexture = false;
    }

    private void ButtonFeltPen_Down() => typeBrush = TypeBrush.FeltPen;
    private void ButtonSpray_Down() => typeBrush = TypeBrush.Spray;
    private void ButtonBrush_Down() => typeBrush = TypeBrush.Brush;
    private void ButtonEraser_Down() => typeBrush = TypeBrush.Eraser;

    private void PaletteButton_ColorChange(Color color)
    {
        if (typeBrush != TypeBrush.Eraser)
            Coloring.ActiveColor = color;
        Coloring.IsDrawTexture = false;
    }

    private enum TypeBrush { Brush, FeltPen, Spray, Eraser }
}
