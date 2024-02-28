using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class KeyBindingFootBarHelper : MonoBehaviour
{
    [SerializeField] private TMP_Text content;

    public void Initialized(InputReader inputReader)
    {
        // 取得action map
        InputActionMap menuActionMap = inputReader.playerInput.asset.FindActionMap("Menu", true);
        // 取得 Menu 裡的 bindings
        IReadOnlyList<InputBinding> menuBindings = menuActionMap.bindings;

        foreach (InputBinding binding in menuBindings) 
        {
            if (binding.groups == null) 
                continue;

            if (binding.groups.Contains("Keyboard") || binding.groups.Contains("Mouse"))
            {
                //Debug.Log("翻譯前actionName: " + binding.action);
                string actionName = TranslateActionNameToChinese(binding.action);
                

                string keyName = binding.ToDisplayString();

                //Debug.Log($"{keyName} : {actionName}");
                // 把有動作關聯的顯示出來
                if (actionName != null)
                    content.text += $"{keyName} : {actionName}   ";
            }
            
        }
    }

    // Action 名稱轉換成 中文
    string TranslateActionNameToChinese(string actionName)
    {
        switch (actionName)
        {
            case "Unpause":
                return "返回遊戲";
            case "BackView":
                return "返回";

            default:
                return null;
        }
    }
}
//InputBinding.ToDisplayString(InputBinding.DisplayStringOptions, InputControl)