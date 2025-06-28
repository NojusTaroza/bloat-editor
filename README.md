# 🎨 BloatEditor - Unity Inspector Enhancement Package

**Transform your Unity inspector into a powerful, organized development tool with conditional attributes, tabbed interfaces, visual previews, and advanced layout controls.**

[![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![GitHub Release](https://img.shields.io/github/v/release/Pynis/BloatEditor)](https://github.com/Pynis/BloatEditor/releases)

---

## ✨ What is BloatEditor?

BloatEditor is a comprehensive Unity inspector enhancement package that provides developers with **powerful conditional attributes**, **tabbed organization**, **visual previews**, and **professional layout controls**. Say goodbye to cluttered inspectors and hello to clean, organized, context-aware interfaces.

### 🎯 Key Problems Solved

• **Cluttered Inspectors** - Fields that should only appear in specific contexts  
• **Poor Visual Organization** - No way to create clear sections and hierarchies  
• **Limited Conditional Logic** - Unity's basic attributes don't support complex conditions  
• **Enum Compatibility Issues** - Built-in Header/Space attributes don't work reliably with enums  
• **No Visual Previews** - Can't see sprites, textures, or GameObjects at a glance  
• **Overwhelming Large Scripts** - No way to organize related fields into logical groups

---

## 📦 Installation

### 🎯 Quick Install via Package Manager

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click **➕** and select **"Add package from git URL"**
3. Enter: `https://github.com/Pynis/BloatEditor.git?path=/Packages/com.noahsbloat.bloateditor`
4. Click **"Add"**

### 🔧 Manual Installation via manifest.json

Add this line to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.noahsbloat.bloateditor": "https://github.com/Pynis/BloatEditor.git?path=/Packages/com.noahsbloat.bloateditor"
  }
}
```

---

## 🚀 Core Features

### 🔍 Conditional Field Visibility

**ShowIf Attribute** - Show fields only when conditions are met
```csharp
[ShowIf("playerLevel > 10")]
public bool canUseAdvancedSkills;

[ShowIf("weaponType == WeaponType.Magic")]
public float manaRequired;
```

**DisableIf Attribute** - Disable field interaction based on conditions
```csharp
[DisableIf("health <= 0")]
public float moveSpeed;

[DisableIf("gameMode == GameMode.Tutorial")]
public bool allowPvP;
```

### 🖼️ Visual Object Previews

**Preview Attribute** - Display visual previews of sprites, textures, GameObjects, and materials
```csharp
[Preview]
public Sprite characterSprite;

[Preview(120, 80, ShowType = false)]
public GameObject playerPrefab;

[Preview(Width = 100, Height = 100, BackgroundColor = "darkgray")]
public Material weaponMaterial;
```

### 📁 Tabbed Organization

**Tab Attribute** - Organize related fields into tabbed groups
```csharp
[Tab("Combat", "Weapons", Color = "red")]
public float swordDamage;

[Tab("Combat", "Weapons", Color = "red")]
public int arrowCount;

[Tab("Combat", "Defense", Color = "blue")]
public float armor;

[Tab("Settings", "Graphics", Color = "green")]
public bool enableShadows;
```

### 🎨 Advanced Layout Controls

**Title Attribute** - Professional section headers with styling
```csharp
[Title("Combat System", FontSize = 16, Color = "red")]
public float attackDamage;

[Title("Debug Options", Color = "gray", DrawLine = false)]
public bool debugMode;
```

**Gap Attribute** - Precise spacing control with optional dividers
```csharp
[Gap(30, DrawLine = true, Color = "blue")]
public float nextSection;

[Gap(15)]
public int spacedProperty;
```

---

### 📋 Requirements

• **Unity 2020.3** or later  
• **Git** installed on your system  
• **.NET Standard 2.1** compatible project

---

## 📊 Supported Condition Types

| Condition Type | Example | Description |
|---------------|---------|-------------|
| **Boolean** | `"isEnabled"` | Simple boolean check |
| **Boolean Comparison** | `"isActive == false"` | Explicit boolean comparison |
| **Enum Comparison** | `"state == GameState.Playing"` | Enum value matching |
| **Numeric Greater** | `"level > 5"` | Greater than comparison |
| **Numeric Less** | `"health <= 25"` | Less than or equal comparison |
| **Numeric Equal** | `"score == 1000"` | Exact value matching |
| **Inverted Logic** | `DisableIf("condition", Invert = true)` | Reverse condition logic |

---

## 🎮 Quick Start Guide

### 🔰 Basic Example

```csharp
using BloatEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Title("Player Configuration", FontSize = 14, Color = "cyan")]
    public string playerName = "Hero";
    public float health = 100f;
    
    [Gap(20, DrawLine = true)]
    [Title("Combat Settings", Color = "red")]
    public bool combatEnabled = true;
    
    [ShowIf("combatEnabled")]
    public float attackDamage = 25f;
    
    [ShowIf("combatEnabled")]
    [DisableIf("health <= 20")]
    public bool canUseSpecialAttacks = true;
    
    [Gap(15)]
    [Preview]
    public Sprite playerSprite;
}
```

### 🎯 Advanced Tabbed Interface

```csharp
public class GameManager : MonoBehaviour
{
    [Title("Game Configuration", FontSize = 16, Color = "white")]
    
    // Player Stats Tab
    [Tab("Game Systems", "Player", Color = "blue")]
    public string playerName = "Hero";
    
    [Tab("Game Systems", "Player", Color = "blue")]
    public float health = 100f;
    
    [Tab("Game Systems", "Player", Color = "blue")]
    [Preview(80, 80)]
    public Sprite playerAvatar;
    
    // Combat Tab
    [Tab("Game Systems", "Combat", Color = "red")]
    public float attackDamage = 25f;
    
    [Tab("Game Systems", "Combat", Color = "red")]
    public bool enableCriticalHits = true;
    
    [Tab("Game Systems", "Combat", Color = "red")]
    [DisableIf("enableCriticalHits == false")]
    public float criticalMultiplier = 2.0f;
    
    // Graphics Tab  
    [Tab("Settings", "Graphics", Color = "green")]
    public bool enableShadows = true;
    
    [Tab("Settings", "Graphics", Color = "green")]
    [ShowIf("enableShadows")]
    public int shadowQuality = 2;
    
    [Tab("Settings", "Graphics", Color = "green")]
    [Preview(100, 60)]
    public Material skyboxMaterial;
}
```

---

## 📚 Complete API Reference

### 🔍 ShowIf Attribute

**Purpose:** Conditionally show/hide fields in the inspector

**Parameters:**
• `condition` (string) - The evaluation expression  
• `Invert` (bool) - Reverse the condition logic  
• `DisablingType` (enum) - Action when condition is false (`DontDraw` or `ReadOnly`)

**Syntax Examples:**
```csharp
[ShowIf("boolField")]
[ShowIf("boolField == true")]
[ShowIf("enumField == EnumType.Value")]
[ShowIf("intField > 10")]
[ShowIf("floatField <= 5.5")]
[ShowIf("condition", Invert = true)]
[ShowIf("condition", DisablingType = DisablingType.ReadOnly)]
```

### 🚫 DisableIf Attribute

**Purpose:** Conditionally disable field interaction

**Parameters:**
• `condition` (string) - The evaluation expression  
• `Invert` (bool) - Reverse the condition logic

**Syntax Examples:**
```csharp
[DisableIf("isGameOver")]
[DisableIf("health <= 0")]
[DisableIf("state != GameState.Playing")]
[DisableIf("level < 10", Invert = true)]
```

### 🖼️ Preview Attribute

**Purpose:** Display visual previews of Unity objects

**Parameters:**
• `Width` (float) - Preview width in pixels (default: 120)  
• `Height` (float) - Preview height in pixels (default: 120)  
• `ShowName` (bool) - Show object name (default: true)  
• `ShowType` (bool) - Show object type (default: true)  
• `BackgroundColor` (string) - Background color (default: "gray")  
• `ShowBorder` (bool) - Show preview border (default: true)  
• `Padding` (float) - Internal padding (default: 5)

**Supported Objects:** Sprite, Texture2D, GameObject, Material, and any object with AssetPreview support

**Preview Examples:**
```csharp
[Preview]
[Preview(80, 80)]
[Preview(Width = 150, Height = 100)]
[Preview(BackgroundColor = "darkgray", ShowBorder = false)]
[Preview(ShowName = false, ShowType = false, Padding = 2f)]
```

### 📁 Tab Attribute

**Purpose:** Organize fields into tabbed groups

**Parameters:**
• `GroupName` (string) - Name of the tab group  
• `TabName` (string) - Name of the individual tab  
• `Color` (string) - Tab color theme (default: "default")  
• `StartExpanded` (bool) - Start expanded (default: true)  
• `Icon` (string) - Unity icon name (optional)  
• `Order` (int) - Tab order within group (default: 0)

**Available Colors:** `"default"`, `"blue"`, `"green"`, `"red"`, `"yellow"`, `"purple"`, `"orange"`

**Tab Examples:**
```csharp
[Tab("Systems", "Combat")]
[Tab("Systems", "Audio", Color = "green")]
[Tab("Settings", "Graphics", Color = "blue", Icon = "Settings")]
[Tab("Debug", "Logging", Order = 1)]
```

### 🎨 Title Attribute

**Purpose:** Create styled section headers

**Parameters:**
• `text` (string) - Title text to display  
• `FontSize` (int) - Font size in pixels (default: 14)  
• `Bold` (bool) - Bold text styling (default: true)  
• `Color` (string) - Text color (default: "white")  
• `DrawLine` (bool) - Show underline separator (default: true)  
• `SpaceAbove` (float) - Space above title (default: 10)  
• `SpaceBelow` (float) - Space below title (default: 5)

**Available Colors:** `"white"`, `"black"`, `"gray"`, `"red"`, `"green"`, `"blue"`, `"yellow"`, `"cyan"`, `"magenta"`

**Styling Examples:**
```csharp
[Title("Basic Header")]
[Title("Large Header", FontSize = 18, Color = "cyan")]
[Title("Subtle Section", Bold = false, DrawLine = false)]
[Title("Spaced Title", SpaceAbove = 25, SpaceBelow = 15)]
```

### 📏 Gap Attribute

**Purpose:** Add precise spacing between fields

**Parameters:**
• `size` (float) - Gap size in pixels (default: 20)  
• `DrawLine` (bool) - Display separator line (default: false)  
• `Color` (string) - Line color (default: "gray")  
• `LineThickness` (float) - Line thickness in pixels (default: 1)

**Spacing Examples:**
```csharp
[Gap(30)]
[Gap(15, DrawLine = true)]
[Gap(40, DrawLine = true, Color = "blue", LineThickness = 2)]
```

---

## 🎯 Real-World Usage Patterns

### 🏥 Complete Player System

```csharp
[Title("Player Character System", FontSize = 18, Color = "cyan")]

[Tab("Character", "Identity", Color = "blue")]
public string characterName = "Hero";

[Tab("Character", "Identity", Color = "blue")]
[Preview(80, 80)]
public Sprite characterAvatar;

[Tab("Character", "Stats", Color = "blue")]
public float maxHealth = 100f;

[Tab("Character", "Stats", Color = "blue")]
public float currentHealth = 100f;

[Tab("Character", "Stats", Color = "blue")]
[ShowIf("currentHealth > 0")]
public float healthRegenRate = 2f;

[Tab("Weapons", "Primary", Color = "red")]
public WeaponType primaryWeapon = WeaponType.Sword;

[Tab("Weapons", "Primary", Color = "red")]
[ShowIf("primaryWeapon == WeaponType.Sword")]
public float swordDamage = 15f;

[Tab("Weapons", "Primary", Color = "red")]
[ShowIf("primaryWeapon == WeaponType.Bow")]
public int arrowCount = 30;

[Tab("Weapons", "Secondary", Color = "red")]
[Preview(60, 60)]
public Sprite weaponIcon;

[Tab("Settings", "Graphics", Color = "green")]
public bool enableShadows = true;

[Tab("Settings", "Graphics", Color = "green")]
[ShowIf("enableShadows")]
[DisableIf("Application.isMobilePlatform")]
public int shadowQuality = 2;

[Tab("Settings", "Audio", Color = "green")]
public float masterVolume = 1.0f;

[Tab("Settings", "Audio", Color = "green")]
[DisableIf("masterVolume <= 0")]
public float musicVolume = 0.8f;
```

### 🔧 Advanced Debug System

```csharp
[Title("Debug & Development Tools", Color = "yellow")]

[Tab("Debug", "Performance", Color = "orange")]
public bool showFPS = false;

[Tab("Debug", "Performance", Color = "orange")]
[ShowIf("showFPS")]
public bool showMemoryUsage = false;

[Tab("Debug", "Visual", Color = "orange")]
public bool showColliders = false;

[Tab("Debug", "Visual", Color = "orange")]
[DisableIf("Application.isPlaying == false")]
public bool allowRuntimeEditing = false;

[Tab("Debug", "Logging", Color = "orange")]
public LogLevel currentLogLevel = LogLevel.Info;

[Tab("Debug", "Logging", Color = "orange")]
[DisableIf("currentLogLevel == LogLevel.Info")]
public bool verboseLogging = false;
```

---

## 🧪 Testing & Validation

### ✅ Compatibility Testing

**Supported Unity Versions:**
• Unity 2020.3 LTS ✅  
• Unity 2021.3 LTS ✅  
• Unity 2022.3 LTS ✅  
• Unity 2023.2+ ✅

**Supported Platforms:**
• Windows ✅  
• macOS ✅  
• Linux ✅

**Field Type Compatibility:**
• `bool`, `int`, `float`, `string` ✅  
• Enums of all types ✅  
• Unity Object references (GameObject, Sprite, Material, etc.) ✅  
• Arrays and Lists ✅  
• Custom Serializable classes ✅

### 🔍 Known Limitations

• **Static Field References** - Conditions can only reference fields in the same object  
• **Method Calls** - Cannot call methods in condition expressions  
• **Complex Expressions** - No support for AND/OR logical operators (yet)  
• **Nested Objects** - Limited support for deeply nested property paths  
• **Performance** - Very large numbers of tabs may impact inspector performance

---

## 🤝 Contributing

We welcome contributions from the Unity community! Here's how you can help:

### 🐛 Bug Reports

Found an issue? Please include:
• Unity version and platform  
• Complete error messages  
• Minimal reproduction case  
• Expected vs actual behavior

### ✨ Feature Requests

Have an idea? Tell us:
• Use case description  
• Proposed syntax/API  
• Why it would benefit the community

### 🔧 Development Setup

1. Fork this repository
2. Clone to your local machine
3. Open in Unity 2020.3+
4. Test with the provided examples
5. Submit pull requests with clear descriptions

### 📝 Code Standards

• Follow Unity C# coding conventions  
• Add XML documentation to public APIs  
• Include unit tests for new features  
• Update README for API changes

---

## 📄 License & Credits

### 📜 MIT License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### 🙏 Acknowledgments

• **Unity Technologies** - For the amazing game engine  
• **Odin Inspector** - Inspiration for conditional attributes  
• **Unity Community** - Feedback and feature suggestions  
• **Contributors** - Everyone who helped improve this package

### 💖 Support the Project

If BloatEditor has improved your development workflow:
• ⭐ Star this repository  
• 🐛 Report bugs and suggest features  
• 📢 Share with other Unity developers  
• 🤝 Contribute code improvements

---

## 🔗 Links & Resources

• **📋 Issues & Bug Reports:** [GitHub Issues](https://github.com/Pynis/BloatEditor/issues)  
• **💬 Discussions:** [GitHub Discussions](https://github.com/Pynis/BloatEditor/discussions)  
• **📦 Latest Release:** [Releases Page](https://github.com/Pynis/BloatEditor/releases)  
• **📚 Unity Package Manager Docs:** [Unity Documentation](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

---

**Made with ❤️ for the Unity community**

Transform your inspector workflow today with BloatEditor!
