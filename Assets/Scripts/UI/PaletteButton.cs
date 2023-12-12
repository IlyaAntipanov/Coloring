using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/PaletteButton", 30)]
public class PaletteButton : MonoBehaviour
{
    public Color ActiveColor;
    public event Action<Color> ColorChange;
    private Image Image;
    private ButtonEvent Button;
    private List<Vector2Int> PositionCircle;
    private Texture2D Texture;
    private void Start()
    {
        Image = GetComponent<Image>();
        Button = GetComponent<ButtonEvent>();
        Button.Drag += () =>
        {
            ActiveColor = GetColor();
            UpdateCircleColor();
            OnColorChange(ActiveColor);
        };
        Texture = Image.sprite.texture;
        PositionCircle = GetCircleCenter();
        UpdateCircleColor();
    }

    private void OnColorChange(Color color) => ColorChange?.Invoke(color);

    private void UpdateCircleColor()
    {
        for (int i = 0; i < PositionCircle.Count; i++)
        {
            var vec = PositionCircle[i];
            Texture.SetPixel(vec.x, vec.y, ActiveColor);
        }
        Texture.Apply();
    }

    private List<Vector2Int> GetCircleCenter()
    {
        var texture = Image.sprite.texture;
        Vector2Int center = new Vector2Int(texture.width / 2, texture.height / 2);
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>(); 
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        var colorFiller = Color.white;
        var alphaThreshold = 0.01f;
        void check()
        {
            var vec = stack.Pop();
            var x = vec.x;
            var y = vec.y;
            positions.Add(vec);
            if (x < texture.width - 1 && !positions.Contains(new Vector2Int(x + 1, y)))
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x + 1, y), colorFiller);
                if (percentTmp > alphaThreshold) stack.Push(new Vector2Int(x + 1, y));
            }
            if (x > 0 && !positions.Contains(new Vector2Int(x - 1, y)))
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x - 1, y), colorFiller);
                if (percentTmp > alphaThreshold) stack.Push(new Vector2Int(x - 1, y));
            }
            if (y < texture.height - 1 && !positions.Contains(new Vector2Int(x, y + 1)))
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x, y + 1), colorFiller);
                if (percentTmp > alphaThreshold) stack.Push(new Vector2Int(x, y + 1));
            }
            if (y > 0 && !positions.Contains(new Vector2Int(x, y - 1)))
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x, y - 1), colorFiller);
                if (percentTmp > alphaThreshold) stack.Push(new Vector2Int(x, y - 1));
            }
        }
        stack.Push(center);
        while (stack.Count > 0)
        {
            check();
        }
        return positions.ToList();
    }

    private Color GetColor()
    {
        var mousePosition = ((Vector2)Input.mousePosition - (Vector2)Image.rectTransform.position + Image.rectTransform.sizeDelta / 2)
            / Image.rectTransform.sizeDelta * new Vector2(Image.sprite.texture.width, Image.sprite.texture.height);
        var color = Image.sprite.texture.GetPixel((int)mousePosition.x, (int)mousePosition.y);
        if (color.a > 0.1 && HelperColoring.ColorComparison(color, Color.white) > 0.1)
            return color;
        return ActiveColor;
    }
}
