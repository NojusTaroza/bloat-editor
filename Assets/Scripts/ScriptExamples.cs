using BloatEditor;
using UnityEngine;
using BloatEditor.Editor;

public class ScriptExamples : MonoBehaviour
{
    public enum EnumSwitchTest
    {
        One,
        Two,
        Three
    }

    [SerializeField] private EnumSwitchTest enumSwitch = EnumSwitchTest.One;
    [SerializeField, ShowIf("enumSwitch == EnumSwitchTest.One")] private float ground = 0.55f;
    [SerializeField, ShowIf("enumSwitch == EnumSwitchTest.Two")] private float flat = 0.5f;
    [SerializeField, ShowIf("enumSwitch == EnumSwitchTest.Three")] private float elevated = 1.0f;

    [Space(20)]
    [Header("Boolean Testing")]
    public bool bool1;
    [SerializeField, ShowIf("bool1")] private int show1 = 100;
    [SerializeField, ShowIf("bool1 == false")] private int show2 = 200;
    [SerializeField, ShowIf("bool1 == true")] private int show3 = 300;

    [Space(20)]
    [Header("Advanced Testing")]
    public bool enableAdvanced;
    [SerializeField, ShowIf("enableAdvanced")] private bool subOption1;
    [SerializeField, ShowIf("enableAdvanced")] private bool subOption2;

    // Nested conditions
    [SerializeField, ShowIf("subOption1")] private string conditionalText = "This shows when subOption1 is true";
    [SerializeField, DisableIf("subOption2 == false")]
    private float readOnlyValue = 42.0f;

    [Space(20)]
    [Header("Inverted Conditions")]
    public bool invertTest;
    [SerializeField, ShowIf("invertTest", Invert = true)]
    private string hiddenWhenTrue = "This hides when invertTest is true";

    private void Start()
    {
        Debug.Log("ShowIf Tester initialized! Check the inspector to test conditional fields.");
    }
}
