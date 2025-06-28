using BloatEditor;
using UnityEngine;

public class LayoutDemoScript : MonoBehaviour
{
    // Enum declarations (no attributes here)
    public enum WeaponType
    {
        None,
        Sword,
        Bow,
        Staff,
        Dagger
    }

    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard,
        Nightmare
    }

    public enum GraphicsQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    // Field declarations with attributes
    [Title("Character Configuration", FontSize = 16, Color = "cyan")]
    public string characterName = "Hero";

    [Gap(15)]
    public float health = 100f;

    [Title("Weapon System", Color = "yellow", DrawLine = true)]
    public WeaponType currentWeapon = WeaponType.Sword;

    [ShowIf("currentWeapon == WeaponType.Sword")]
    public float swordDamage = 25f;

    [ShowIf("currentWeapon == WeaponType.Bow")]
    public int arrowCount = 30;

    [Gap(20, DrawLine = true, Color = "blue")]
    [Title("Combat Stats", FontSize = 14, Bold = true, Color = "red")]
    public DifficultyLevel difficulty = DifficultyLevel.Normal;

    [DisableIf("difficulty == DifficultyLevel.Easy")]
    public float criticalHitChance = 0.15f;

    [DisableIf("difficulty == DifficultyLevel.Nightmare")]
    public bool allowRespawn = true;

    [Gap(25)]
    [Title("Advanced Settings", FontSize = 12, Color = "green", SpaceAbove = 5, SpaceBelow = 10)]
    public GraphicsQuality quality = GraphicsQuality.Medium;

    [ShowIf("quality == GraphicsQuality.Ultra")]
    [DisableIf("health <= 50")]
    public bool enableRayTracing = false;

    [Gap(30, DrawLine = true, Color = "magenta", LineThickness = 2)]
    [Title("Debug Information", FontSize = 18, Color = "white", Bold = false)]
    public bool debugMode = false;

    [ShowIf("debugMode")]
    public LogLevel currentLogLevel = LogLevel.Info;

    [ShowIf("debugMode")]
    [DisableIf("currentLogLevel == LogLevel.Info")]
    public bool verboseLogging = false;

    [Gap(15, DrawLine = true)]
    [Title("Performance Metrics", FontSize = 13, Color = "yellow")]
    public int targetFrameRate = 60;

    [DisableIf("targetFrameRate < 30")]
    public bool enableVSync = true;

    [DisableIf("targetFrameRate >= 120")]
    public float performanceBudget = 16.67f;

    private void Start()
    {
        Debug.Log("Layout Demo Script initialized with enhanced Title and Gap attributes!");
    }
}