using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InputStrings : MonoBehaviour
{
    public static InputStrings Instance;
    public inputRefClass inputRef;
    [System.Serializable]
    public class inputRefClass
    {
        public TMP_SpriteAsset SA_keyboard;
        public TMP_SpriteAsset SA_xbox;
        public TMP_SpriteAsset SA_ps;
        public TMP_SpriteAsset SA_switch;
        public TMP_SpriteAsset SA_steamDeck;
    }
    public PlayerInput playerInput;
    [HideInInspector] public string s_inputType = "";
    [HideInInspector] public bool b_isGamepad = false;
    [HideInInspector] public string[] inputs = new string[0];
    public enum inputActions
    {
        Move,
        Look,
        Attack,
        Interact,
        Crouch,
        Jump,
        Previous,
        Next,
        Sprint,
        GraffitiLine_1,
        GraffitiLine_2,
        GraffitiLine_3,
        GraffitiLine_4,
        GraffitiLine_5,
        GraffitiLine_6
    }
    public enum enumType { interact, talk, landing, combine, combineReverse, input}
    public static string[] interactText =
    {
        "[0] interact\n<b>[1]</b>",
        "[0] talk\n<b>[1]</b>",
        "[0] land\n<b>[1]</b>",
        "[0] [1]",
        "[1] [0]",
        "[0]"
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        Setup_InputStrings();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Setup_InputStrings()
    {
        if (!playerInput)
            playerInput = GetComponent<PlayerInput>();

        enumType _E = enumType.input;
        List<string> _list = new List<string>();
        string[] _names = System.Enum.GetNames(typeof(inputActions));
        for (int i = 0; i < _names.Length; i++)
        {
            string _input = "".ToString_InputSetup((inputActions)i, _E);
            _list.Add(_input);
        }
        inputs = _list.ToArray();
    }
    
    public void Input_ChangedInput(PlayerInput input)
    {
            s_inputType = input.devices[0].displayName;
            b_isGamepad = input.currentControlScheme == "Gamepad";
            Setup_InputStrings();
    }
}
