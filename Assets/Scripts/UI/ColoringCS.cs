using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public struct Cell
{
    public int Region;
    public float Percent;
};

public class ColoringCS : MonoBehaviour
{
    private RenderTexture RTMask;
    private RenderTexture RTTexture;
    private RenderTexture RTPrewDraw;
    private RenderTexture TextureRender;
    private Vector2 PositionMouseInColoring;
    private Vector2 PositionMouseInColoringPrev;

    public ComputeShader ShaderTest;
    public Color ColorFiller;
    public Color LineColor;
    public int SizeBrush = 10;
    public Color ActiveColor;
    public bool IsDrawTexture;
    public Texture2D MaskBrush;
    public Texture2D TextureDraw;

    private RawImage RawImage;
    private Cell[] Cells;
    private int CountRegions;
    private int IdKernelDraw;
    private int IdKernelClear;
    private int IdKernelDrawPrew;
    private int RegionActive;
    private ComputeBuffer CellsBuffer;

    void Start()
    {
        RawImage = GetComponent<RawImage>();
        TextureRender = new RenderTexture(RawImage.texture.width, RawImage.texture.height, 0, GraphicsFormat.R32G32B32A32_SFloat);
        TextureRender.enableRandomWrite = true;
        TextureRender.Create();
        Cells = GetCanvasIndex(RawImage.texture.ToTexture2D());
        RawImage.texture = TextureRender;
        InitShader();
    }

    public void SetTexture()
    {
        RTTexture = new RenderTexture(TextureDraw.width, TextureDraw.height, 0);
        RTTexture.enableRandomWrite = true;
        RenderTexture.active = RTTexture;
        Graphics.Blit(TextureDraw, RTTexture);
        ShaderTest.SetTexture(IdKernelDraw, "TextureDraw", RTTexture);
        ShaderTest.SetTexture(IdKernelDrawPrew, "TextureDraw", RTTexture);
        ShaderTest.SetVector("TextureSize", new Vector2(RTTexture.width, RTTexture.height));
    }

    private void SetPrewDraw()
    {
        RTPrewDraw = new RenderTexture(TextureRender.width, TextureRender.height, 0);
        RTPrewDraw.enableRandomWrite = true;
        ShaderTest.SetTexture(IdKernelDraw, "ResultPrew", RTPrewDraw);
        ShaderTest.SetTexture(IdKernelClear, "ResultPrew", RTPrewDraw);
        ShaderTest.SetTexture(IdKernelDrawPrew, "ResultPrew", RTPrewDraw);
    }

    public void SetMask()
    {
        RTMask = new RenderTexture(MaskBrush.width, MaskBrush.height, 0);
        RTMask.enableRandomWrite = true;
        RenderTexture.active = RTMask;
        Graphics.Blit(MaskBrush, RTMask);
        ShaderTest.SetTexture(IdKernelDraw, "Mask", RTMask);
        ShaderTest.SetTexture(IdKernelDrawPrew, "Mask", RTMask);
        ShaderTest.SetVector("MaskSize", new Vector2(RTMask.width, RTMask.height));
    }

    private void InitShader()
    {
        IdKernelDraw = ShaderTest.FindKernel("CSDraw");
        IdKernelClear = ShaderTest.FindKernel("CSClear");
        IdKernelDrawPrew = ShaderTest.FindKernel("CSDrawPrew");
        ShaderTest.SetTexture(IdKernelDraw, "Result", TextureRender);
        ShaderTest.SetTexture(IdKernelClear, "Result", TextureRender);
        ShaderTest.SetTexture(IdKernelDrawPrew, "Result", TextureRender);
        SetMask();
        SetPrewDraw();
        SetTexture();
        ShaderTest.SetFloat("Width", TextureRender.width);
        ShaderTest.SetFloat("Height", TextureRender.height);
        ShaderTest.SetFloat("SizeBrush", SizeBrush);
        ShaderTest.SetBool("IsDrawTexture", IsDrawTexture);
        ShaderTest.SetVector("MousePosition", PositionMouseInColoring);
        var totalSize = sizeof(int) + sizeof(float);
        ShaderTest.SetVector("LineColor", LineColor);
        CellsBuffer = new ComputeBuffer(Cells.Length, totalSize);
        CellsBuffer.SetData(Cells);
        ShaderTest.SetBuffer(IdKernelDraw, "Cells", CellsBuffer);
        ShaderTest.SetBuffer(IdKernelClear, "Cells", CellsBuffer);
        ShaderTest.SetBuffer(IdKernelDrawPrew, "Cells", CellsBuffer);

        ShaderTest.Dispatch(IdKernelClear, TextureRender.width / 8, TextureRender.height / 8, 1);
        RenderTexture.active = TextureRender;
    }

    void Update()
    {
        PositionMouseInColoring = ((Vector2)Input.mousePosition - (Vector2)RawImage.rectTransform.position + RawImage.rectTransform.sizeDelta / 2)
            / RawImage.rectTransform.sizeDelta * new Vector2(RawImage.texture.width, RawImage.texture.height);
        if (Input.GetMouseButtonDown(0))
        {
            if (PositionMouseInColoring.x >= 0 && PositionMouseInColoring.y >= 0 &&
                PositionMouseInColoring.x < TextureRender.width && PositionMouseInColoring.y < TextureRender.height)
            {
                RegionActive = Cells[(int)PositionMouseInColoring.x + (int)PositionMouseInColoring.y * TextureRender.width].Region;
            }
            else
            {
                RegionActive = 0;
            }
        }
        else if (Input.GetMouseButton(0) && RegionActive != 0)
        {
            ShaderTest.SetVector("ActiveColor", ActiveColor);
            ShaderTest.SetFloat("SizeBrush", SizeBrush);
            ShaderTest.SetBool("IsDrawTexture", IsDrawTexture);

            ShaderTest.SetInt("RegionActive", RegionActive);
            ShaderTest.SetVector("MousePosition", PositionMouseInColoring);
            ShaderTest.Dispatch(IdKernelDraw, Mathf.CeilToInt(SizeBrush / 8f), Mathf.CeilToInt(SizeBrush / 8f), 1);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RegionActive = 0;
        }
        else
        {
            if (PositionMouseInColoring.x >= 0 && PositionMouseInColoring.y >= 0 &&
                PositionMouseInColoring.x < TextureRender.width && PositionMouseInColoring.y < TextureRender.height)
            {
                var region = Cells[(int)PositionMouseInColoring.x + (int)PositionMouseInColoring.y * TextureRender.width].Region;
                if (region != 0)
                {
                    ShaderTest.SetVector("ActiveColor", ActiveColor);
                    ShaderTest.SetFloat("SizeBrush", SizeBrush);
                    ShaderTest.SetBool("IsDrawTexture", IsDrawTexture);


                    ShaderTest.SetInt("RegionActive", region);
                    ShaderTest.SetVector("MousePosition", PositionMouseInColoring);
                    ShaderTest.SetVector("MousePositionPrev", PositionMouseInColoringPrev);
                    ShaderTest.Dispatch(IdKernelDrawPrew, Mathf.CeilToInt(SizeBrush / 8f), Mathf.CeilToInt(SizeBrush / 8f), 1);
                    PositionMouseInColoringPrev = PositionMouseInColoring;
                }
            }
        }
    }

    private Cell[] GetCanvasIndex(Texture2D texture)
    {
        var canvasIndex = new Cell[texture.width * texture.height];
        CountRegions = 0;
        Stack<(int x, int y, float percent)> stack = new Stack<(int, int, float)>();
        var alphaThreshold = 0.8f;
        void check(int index)
        {
            var (x, y, percent) = stack.Pop();
            canvasIndex[x + y * texture.width].Region = index;
            canvasIndex[x + y * texture.width].Percent = percent;
            if (x < texture.width - 1 && canvasIndex[x + 1 + y * texture.width].Region == 0)
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x + 1, y), ColorFiller);
                if (percentTmp < alphaThreshold) stack.Push((x + 1, y, percentTmp));
            }
            if (x > 0 && canvasIndex[x - 1 + y * texture.width].Region == 0)
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x - 1, y), ColorFiller);
                if (percentTmp < alphaThreshold) stack.Push((x - 1, y, percentTmp));
            }
            if (y < texture.height - 1 && canvasIndex[x + (y + 1) * texture.width].Region == 0)
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x, y + 1), ColorFiller);
                if (percentTmp < alphaThreshold) stack.Push((x, y + 1, percentTmp));
            }
            if (y > 0 && canvasIndex[x + (y - 1) * texture.width].Region == 0)
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x, y - 1), ColorFiller);
                if (percentTmp < alphaThreshold) stack.Push((x, y - 1, percentTmp));
            }
        }

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                var percentTmp = HelperColoring.ColorComparison(texture.GetPixel(x, y), ColorFiller);
                if (percentTmp < alphaThreshold && canvasIndex[x + y * texture.width].Region == 0)
                {
                    stack.Push((x, y, percentTmp));
                    CountRegions++;
                    while (stack.Count > 0)
                    {
                        check(CountRegions);
                    }
                }
            }
        }
        return canvasIndex;
    }

    private void OnDestroy()
    {
        CellsBuffer.Release();
    }
}
