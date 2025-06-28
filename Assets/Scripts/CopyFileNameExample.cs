using BloatEditor;
using UnityEngine;

public class CopyFileNameExample : MonoBehaviour
{
    [Title("Basic CopyFileName Examples", FontSize = 16, Color = "cyan")]

    [CopyFileName]
    public string basicFileName = "";

    [CopyFileName(includeExtension: true)]
    public string fileNameWithExtension = "";

    [Gap(15)]
    [Title("Advanced Configuration", Color = "green")]

    [CopyFileName(DropZoneColor = "green", HelpText = "Drop script files here")]
    public string scriptFileName = "";

    [CopyFileName(IncludeExtension = false, ToPascalCase = true, Suffix = "Controller")]
    public string controllerName = "";

    [CopyFileName(ToCamelCase = true, Prefix = "my", ShowDropZone = false)]
    public string camelCaseField = "";

    [Gap(15)]
    [Title("File Type Filtering", Color = "orange")]

    [CopyFileName(
        AllowedExtensions = new[] { ".cs", ".js" },
        DropZoneColor = "purple",
        HelpText = "Scripts only (.cs, .js)"
    )]
    public string scriptFile = "";

    [CopyFileName(
        AllowedExtensions = new[] { ".png", ".jpg", ".jpeg" },
        DropZoneColor = "magenta",
        HelpText = "Images only"
    )]
    public string imageFile = "";

    [CopyFileName(
        AllowedExtensions = new[] { ".prefab" },
        DropZoneColor = "yellow",
        ToPascalCase = true,
        HelpText = "Prefabs only"
    )]
    public string prefabName = "";

    [Gap(15)]
    [Title("Custom Styling", Color = "red")]

    [CopyFileName(
        DropZoneHeight = 30f,
        DropZoneColor = "red",
        HelpText = "Large drop zone for any file",
        ToPascalCase = true
    )]
    public string largeDropZone = "";

    [CopyFileName(
        ShowDropZone = false,
        ToCamelCase = true,
        Prefix = "handle"
    )]
    public string noDropZone = "";

    [Gap(20, DrawLine = true)]
    [Title("Real-World Usage Examples", Color = "white")]

    [Header("Game Object References")]
    [CopyFileName(ToPascalCase = true, Suffix = "Prefab", AllowedExtensions = new[] { ".prefab" })]
    public string playerPrefab = "";

    [CopyFileName(ToPascalCase = true, Suffix = "Prefab", AllowedExtensions = new[] { ".prefab" })]
    public string enemyPrefab = "";

    [Gap(10)]
    [Header("Script References")]
    [CopyFileName(ToPascalCase = true, AllowedExtensions = new[] { ".cs" })]
    public string playerController = "";

    [CopyFileName(ToPascalCase = true, AllowedExtensions = new[] { ".cs" })]
    public string gameManager = "";

    [Gap(10)]
    [Header("Asset References")]
    [CopyFileName(ToCamelCase = true, AllowedExtensions = new[] { ".png", ".jpg" })]
    public string characterTexture = "";

    [CopyFileName(ToCamelCase = true, AllowedExtensions = new[] { ".mat" })]
    public string characterMaterial = "";

    [Gap(10)]
    [Header("Audio References")]
    [CopyFileName(ToCamelCase = true, AllowedExtensions = new[] { ".wav", ".mp3", ".ogg" })]
    public string backgroundMusic = "";

    [CopyFileName(ToCamelCase = true, AllowedExtensions = new[] { ".wav", ".mp3" })]
    public string jumpSound = "";

    private void Start()
    {
        Debug.Log("CopyFileName Examples loaded!");
        Debug.Log("Try dragging files from the Project window onto the string fields above!");
        Debug.Log("Each field has different configurations for various use cases.");

        LogCurrentValues();
    }

    private void LogCurrentValues()
    {
        if (!string.IsNullOrEmpty(basicFileName))
            Debug.Log($"Basic file: {basicFileName}");

        if (!string.IsNullOrEmpty(controllerName))
            Debug.Log($"Controller: {controllerName}");

        if (!string.IsNullOrEmpty(scriptFile))
            Debug.Log($"Script: {scriptFile}");

        if (!string.IsNullOrEmpty(playerPrefab))
            Debug.Log($"Player Prefab: {playerPrefab}");
    }

    [ContextMenu("Clear All Fields")]
    private void ClearAllFields()
    {
        basicFileName = "";
        fileNameWithExtension = "";
        scriptFileName = "";
        controllerName = "";
        camelCaseField = "";
        scriptFile = "";
        imageFile = "";
        prefabName = "";
        largeDropZone = "";
        noDropZone = "";
        playerPrefab = "";
        enemyPrefab = "";
        playerController = "";
        gameManager = "";
        characterTexture = "";
        characterMaterial = "";
        backgroundMusic = "";
        jumpSound = "";

        Debug.Log("All CopyFileName fields cleared!");
    }
}