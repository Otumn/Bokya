using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingScript : MonoBehaviour
{
    public Camera cam;
    public Camera hiddenCam;
    public RenderTexture targetRender;

    public string[] possibleKanjis;
    public Text renderedText;
    public Text hintText;
    public int texSize = 32;
    private Texture2D tex;

    public int brushSize = 3;
    public Color[] brushColors;

    public Renderer drawingRenderer;
    public Renderer patronRenderer;
    public Renderer debugRenderer;

    private void Start()
    {
        SetupTexture();
    }

    private void Update()
    {
        DrawingManager();
    }

    private void DrawingManager()
    {
        /*if (Input.touchCount < 1)
            return;*/
        if (!Input.GetMouseButton(0)) return;

        RaycastHit hit;
        if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            return;

        Renderer rend = hit.transform.GetComponent<Renderer>();
        MeshCollider meshCollider = hit.collider as MeshCollider;

        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        tex.SetPixels((int)pixelUV.x, (int)pixelUV.y, brushSize, brushSize, brushColors);
        tex.Apply();
        rend.material.mainTexture = tex;
    }

    private void SetupTexture()
    {
        tex = new Texture2D(texSize, texSize);
        Color32 resetColor = new Color32(255, 255, 255, 0);
        Color32[] resetColorArray = tex.GetPixels32();

        for (int i = 0; i < resetColorArray.Length; i++)
        {
            resetColorArray[i] = resetColor;
        }

        tex.SetPixels32(resetColorArray);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        LoadNewKanji();
    }

    [ContextMenu("Test texture compare")]
    public void CompareTextures()
    {
        // Create texture2D from the hiddenRenderTexture
        //RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = targetRender;

        Texture2D patronTexture = new Texture2D(texSize, texSize);
        patronTexture.ReadPixels(new Rect(0, 0, targetRender.width, targetRender.height), 0, 0);
        patronTexture.Apply();
        debugRenderer.material.mainTexture = patronTexture;

        // Compare both the texture

        Color[] drawnPix = tex.GetPixels();
        Color[] patronPix = patronTexture.GetPixels();

        int result = 0;
        for (int i = 0; i < drawnPix.Length; i++)
        {
            if(drawnPix[i].grayscale < 1f && patronPix[i].grayscale < 1f)
            {
                result++;
            }
        }
        hintText.text = result.ToString();
        StartCoroutine(ResetCoroutine());
    }

    private IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(5f);
        SetupTexture();
    }

    private void LoadNewKanji()
    {
        int c = Random.Range(0, possibleKanjis.Length);
        renderedText.text = possibleKanjis[c];
        hintText.text = possibleKanjis[c];
    }

    //Debug method for the brush colors
    [ContextMenu("Make color array")]
    public void MakeColorArray()
    {
        List<Color> tempColors = new List<Color>();

        for (int i = 0; i < brushSize * brushSize; i++)
        {
            float value = Mathf.InverseLerp(0, brushSize * brushSize, i);
            tempColors.Add(new Color(0, 0, 0));
        }

        brushColors = tempColors.ToArray();
    }
}
