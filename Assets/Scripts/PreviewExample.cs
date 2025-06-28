using BloatEditor;
using UnityEngine;

public class PreviewExample : MonoBehaviour
{
    [Title("Sprite Previews", FontSize = 16, Color = "cyan")]

    [Preview]
    public Sprite characterSprite;

    [Preview(80, 80)]
    public Sprite weaponIcon;

    [Preview(Width = 150, Height = 100, ShowType = false)]
    public Sprite backgroundSprite;

    [Gap(20, DrawLine = true, Color = "blue")]
    [Title("GameObject Previews", Color = "green")]

    [Preview]
    public GameObject playerPrefab;

    [Preview(100, 100)]
    public GameObject enemyPrefab;

    [Gap(20, DrawLine = true, Color = "yellow")]
    [Title("Material & Texture Previews", Color = "magenta")]

    [Preview]
    public Material playerMaterial;

    [Preview(BackgroundColor = "darkgray", ShowBorder = true)]
    public Texture2D uiTexture;

    [Gap(20, DrawLine = true, Color = "red")]
    [Title("Advanced Preview Options", Color = "white")]

    [Preview(Width = 200, Height = 120, ShowName = true, ShowType = true, BackgroundColor = "white")]
    public Sprite largePreviewSprite;

    [Preview(Width = 60, Height = 60, ShowName = false, ShowType = false, Padding = 2f)]
    public Sprite compactPreview;

    [Gap(15)]
    [Title("Conditional Previews", FontSize = 12, Color = "yellow")]
    public bool showWeaponPreview = true;

    [ShowIf("showWeaponPreview")]
    [Preview(90, 90)]
    public Sprite conditionalWeaponSprite;

    private void Start()
    {
        Debug.Log("Preview attribute example loaded! Check the inspector to see visual previews.");
    }
}