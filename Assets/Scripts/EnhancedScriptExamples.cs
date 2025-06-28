//using BloatEditor;
//using UnityEngine;

//public class EnhancedScriptExamples : MonoBehaviour
//{
//    [Header("Enum-Based Conditions")]
//    public enum WeaponType
//    {
//        Sword,
//        Bow,
//        Magic,
//        Unarmed
//    }
    
//    [SerializeField] private WeaponType currentWeapon = WeaponType.Sword;
    
//    // Show/Hide based on enum values
//    [SerializeField, ShowIf("currentWeapon == WeaponType.Sword")] 
//    private float swordDamage = 10f;
    
//    [SerializeField, ShowIf("currentWeapon == WeaponType.Bow")] 
//    private int arrowCount = 30;
    
//    [SerializeField, ShowIf("currentWeapon == WeaponType.Magic")] 
//    private float manaRequired = 25f;

//    [Space(15)]
//    [Header("DisableIf - Boolean Conditions")]
//    public bool isPlayerDead = false;
//    public bool godModeEnabled = false;
    
//    // Disable when boolean is true
//    [SerializeField, DisableIf("isPlayerDead")] 
//    private float moveSpeed = 5f;
    
//    [SerializeField, DisableIf("isPlayerDead")] 
//    private float jumpHeight = 2f;
    
//    // Disable when boolean is false (using == false)
//    [SerializeField, DisableIf("godModeEnabled == false")] 
//    private bool takeDamage = true;

//    [Space(15)]
//    [Header("DisableIf - Numeric Conditions")]
//    public int playerLevel = 1;
//    public float health = 100f;
//    public int experience = 0;
    
//    // Disable when level is too low
//    [SerializeField, DisableIf("playerLevel < 5")] 
//    private bool canUseMagic = false;
    
//    [SerializeField, DisableIf("playerLevel <= 10")] 
//    private bool canEquipLegendaryItems = false;
    
//    // Disable when health is too high/low
//    [SerializeField, DisableIf("health > 75")] 
//    private bool needsHealing = false;
    
//    [SerializeField, DisableIf("health >= 100")] 
//    private float healingPotionEffectiveness = 1.0f;
    
//    // Disable when experience reaches certain thresholds
//    [SerializeField, DisableIf("experience == 1000")] 
//    private bool canGainExperience = true;
    
//    [SerializeField, DisableIf("experience != 0")] 
//    private string newPlayerTip = "Welcome to the game!";

//    [Space(15)]
//    [Header("Mixed Conditions")]
//    public bool advancedMode = false;
//    public float difficultyMultiplier = 1.0f;
    
//    // Combine ShowIf and DisableIf
//    [SerializeField, ShowIf("advancedMode")] 
//    [DisableIf("difficultyMultiplier > 2.0")] 
//    private float expertModeBonus = 1.5f;
    
//    // Inverted conditions
//    [SerializeField, DisableIf("playerLevel > 5", Invert = true)] 
//    private string beginnerHint = "Try the tutorial first!";

//    [Space(15)]
//    [Header("Complex Example - Combat System")]
//    public enum CombatState
//    {
//        Idle,
//        Attacking,
//        Defending,
//        Stunned
//    }
    
//    public CombatState combatState = CombatState.Idle;
//    public float stamina = 100f;
//    public bool isBlocking = false;
    
//    [SerializeField, DisableIf("combatState == CombatState.Stunned")] 
//    private bool canMove = true;
    
//    [SerializeField, DisableIf("stamina <= 20")] 
//    private bool canSprint = true;
    
//    [SerializeField, ShowIf("combatState == CombatState.Defending")] 
//    [DisableIf("stamina < 10")] 
//    private float blockEffectiveness = 0.8f;
    
//    [SerializeField, DisableIf("isBlocking")] 
//    [DisableIf("combatState == CombatState.Attacking")] 
//    private float dodgeChance = 0.3f;

//    private void Start()
//    {
//        Debug.Log("Enhanced DisableIf system loaded! Check the inspector to test all condition types.");
        
//        // Demonstrate runtime changes
//        InvokeRepeating(nameof(SimulateGameplay), 2f, 3f);
//    }
    
//    private void SimulateGameplay()
//    {
//        // Simulate some gameplay changes to see the conditions in action
//        experience += Random.Range(10, 50);
//        health = Mathf.Max(0, health - Random.Range(0, 25));
//        stamina = Mathf.Max(0, stamina - Random.Range(5, 20));
        
//        if (Random.value > 0.7f)
//        {
//            playerLevel++;
//            Debug.Log($"Level up! Now level {playerLevel}");
//        }
        
//        if (Random.value > 0.8f)
//        {
//            currentWeapon = (WeaponType)Random.Range(0, 4);
//            Debug.Log($"Weapon changed to: {currentWeapon}");
//        }
//    }
//}