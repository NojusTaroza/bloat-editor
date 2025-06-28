using BloatEditor;
using UnityEngine;

public class MinimalTabTest : MonoBehaviour
{
    [Title("Enhanced Tab System Demo", FontSize = 16, Color = "cyan")]

    // Player System - Red color inheritance
    [Tab("Player", "Stats", Color = "red")]
    public string playerName = "Hero";

    [Tab("Player", "Stats")] // Inherits red color
    public float health = 100f;

    [Tab("Player", "Stats")] // Still red
    public int level = 1;

    [Tab("Player", "Combat", Color = "blue")] // Changes to blue
    public float attackDamage = 25f;

    [Tab("Player", "Combat")] // Inherits blue
    public float attackSpeed = 1.2f;

    [Tab("Player", "Combat")] // Still blue
    public bool canCriticalHit = true;

    [Tab("Player", "Movement", Color = "green")] // Changes to green
    public float walkSpeed = 5f;

    [Tab("Player", "Movement")] // Inherits green from Movement tab
    public float runSpeed = 10f;

    [Gap(20, DrawLine = true)]

    // Game Settings - Color inheritance within same group
    [Tab("Settings", "Graphics", Color = "purple")]
    public bool enableShadows = true;

    [Tab("Settings", "Graphics")] // Inherits purple
    [ShowIf("enableShadows")]
    public int shadowQuality = 2;

    [Tab("Settings", "Graphics")] // Still purple
    public bool enablePostProcessing = true;

    [Tab("Settings", "Audio", Color = "orange")] // Changes to orange
    public float masterVolume = 1.0f;

    [Tab("Settings", "Audio")] // Inherits orange
    [DisableIf("masterVolume <= 0")]
    public float musicVolume = 0.8f;

    [Tab("Settings", "Audio", Color = "yellow")] // Changes Audio tab to yellow
    public float sfxVolume = 1.0f;

    [Tab("Settings", "Controls")] // Inherits yellow (last color used)
    public float mouseSensitivity = 2.0f;

    [Tab("Settings", "Controls")] // Still yellow
    public bool invertMouseY = false;

    [Gap(20, DrawLine = true)]

    // Debug Tools - Mixed colors for demonstration
    [Tab("Debug", "Performance", Color = "cyan")]
    public bool showFPS = false;

    [Tab("Debug", "Performance")] // Inherits cyan
    [ShowIf("showFPS")]
    public bool showMemoryUsage = false;

    [Tab("Debug", "Visual", Color = "magenta")] // Changes to magenta
    public bool showColliders = false;

    [Tab("Debug", "Visual")] // Inherits magenta
    public bool showWireframes = false;

    [Tab("Debug", "Logging")] // Inherits magenta (last color)
    public bool enableDebugLogs = true;

    [Tab("Debug", "Logging", Indent = true)] // Indented field, still magenta
    [DisableIf("enableDebugLogs == false")]
    public bool verboseLogging = false;

    [Gap(20, DrawLine = true)]

    // Weapon System - Custom styling
    [Tab("Weapons", "Primary", Color = "red", TabHeight = 32f, FontSize = 11)]
    public WeaponType primaryWeapon = WeaponType.Sword;

    [Tab("Weapons", "Primary")] // Inherits red and styling
    [ShowIf("primaryWeapon == WeaponType.Sword")]
    public float swordDamage = 15f;

    [Tab("Weapons", "Primary")] // Still red
    [ShowIf("primaryWeapon == WeaponType.Bow")]
    public int arrowCount = 30;

    [Tab("Weapons", "Secondary", Color = "blue")] // Changes to blue
    public WeaponType secondaryWeapon = WeaponType.None;

    [Tab("Weapons", "Secondary")] // Inherits blue
    [ShowIf("secondaryWeapon != WeaponType.None")]
    public float secondaryDamage = 10f;

    public enum WeaponType
    {
        None,
        Sword,
        Bow,
        Staff,
        Dagger
    }

    private void Start()
    {
        Debug.Log("Enhanced Tab System loaded! Colors inherit intelligently within each group.");
    }
}