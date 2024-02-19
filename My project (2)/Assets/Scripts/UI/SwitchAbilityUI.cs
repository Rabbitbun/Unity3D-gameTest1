using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SwitchAbilityUI : MonoBehaviour
{
    public int CurrentAbilityIndex = 0;

    int _fadeRate;
    
    // whole SwitchAbilityUI's transform
    public Transform StyleUITransform;
    
    public Transform Arrow;

    private void Awake()
    {
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.SwitchSkill.performed += SwitchSkillList;
    }

    private void Start()
    {
        Arrow.DOScale(1, 0.1f).SetRecyclable(true);
    }

    private void SwitchSkillList(InputAction.CallbackContext context)
    {
        CurrentAbilityIndex = ++CurrentAbilityIndex % 4;
        if (MasterManager.Instance.GameEventManager.IsgamePaused)
            return;

        print("Tab Press!");

        //if (StyleUITransform.localScale != Vector3.zero)
        //{
        //    //StyleUITransform.DOScale(1, 1);
        //    //StyleUITransform.gameObject.SetActive(false);
        //    Arrow.DORotate(new Vector3(0, 0, -90), 0.1f, RotateMode.Fast);
        //}
        //else
        //{
        //    //StyleUITransform.DOScale(0, 1);
        //    //StyleUITransform.DOPlayBackwards();
        //    //StyleUITransform.gameObject.SetActive(true);
        //    Arrow.DORotate(new Vector3(0, 0, 90), 0.1f, RotateMode.Fast);
        //}
        Arrow.DORotate(new Vector3(0, 0, -90 * CurrentAbilityIndex), 0.5f, RotateMode.Fast);
    }
}
