using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Assertions;
//using HoloToolkit.Unity.Speech;

[RequireComponent(typeof(Animator))]
public class Panda : MonoBehaviour, IInputHandler, IFocusable/*, ISpeechHandler*/
{
    private Animator _animator;
    private Vector3 _originalPosition;
    [Tooltip("The GameObject that the Panda will eat")]
    [SerializeField] private GameObject _bamboo;
    [Tooltip("The Panda Paw to hold the food/bamboo")]
    [SerializeField] private Transform _pawToHoldBamboo;  // 'PANDA/new_Bone009' in the model is the left paw

    // Animator Hashes, for efficiency
    private int _isEatingParameterHash = Animator.StringToHash("IsEating");
    private int _isLayingDownParameterHash = Animator.StringToHash("IsLayingDown");
    private int _isIdlingParameterHas = Animator.StringToHash("IsIdling");

    private bool _isEating = false;
    private bool _isLayingDown = false;
    private bool _isIdling = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(_bamboo);
        Assert.IsNotNull(_pawToHoldBamboo);
    }

    void Start()
    {
        _originalPosition = this.transform.position;
        SetIdle();
    }

    private void GrabBamboo()
    {
        _bamboo.transform.SetParent(_pawToHoldBamboo);
        _bamboo.transform.localPosition = Vector3.zero;
    }

    void IInputHandler.OnInputDown(InputEventData eventData)
    {
        Debug.Log("OnInputDown");                
    }

    void IInputHandler.OnInputUp(InputEventData eventData)
    {
        Debug.Log("OnInputUp");

        if (_isIdling)
        {
            SetEat();
            GrabBamboo();
            return;
        }

        if (_isEating)
        {
            SetLayDown();
            return;
        }
        
        if (_isLayingDown)
        {
            SetIdle();
            return;
        }
    }

    void IFocusable.OnFocusEnter()
    {
        Debug.Log("OnFocusEnter");
    }

    void IFocusable.OnFocusExit()
    {
        Debug.Log("OnFocusExit");
    }


    #region Helper Methods
    public void SetEat()
    {
        _isEating = true;
        _isIdling = false;
        _isLayingDown = false;
        SetAnimatorParameters();
    }

    public void SetIdle()
    {
        _isEating = false;
        _isIdling = true;
        _isLayingDown = false;
        SetAnimatorParameters();
    }

    public void SetLayDown()
    {
        _isEating = false;
        _isIdling = false;
        _isLayingDown = true;
        SetAnimatorParameters();
    }

    private void SetAnimatorParameters()
    {
        _animator.SetBool(_isEatingParameterHash, _isEating);
        _animator.SetBool(_isIdlingParameterHas, _isIdling);
        _animator.SetBool(_isLayingDownParameterHash, _isLayingDown);
    }
    #endregion
}
