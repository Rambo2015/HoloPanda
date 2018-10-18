using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Assertions;
//using HoloToolkit.Unity.Speech;

[RequireComponent(typeof(Animator))]
public class Panda : MonoBehaviour, IInputHandler/*, ISpeechHandler*/
{
    private Animator _animator;
    private Vector3 _originalPosition;
    [Tooltip("The GameObject that the Panda will eat")]
    [SerializeField] private GameObject _bamboo;
    [Tooltip("The Panda Paw to hold the food/bamboo")]
    [SerializeField] private Transform _pawToHoldBamboo;  // 'PANDA/new_Bone031' in the model

    // Animator Hashes, for efficiency
    private int _isEatingParameterHash = Animator.StringToHash("IsEating");
    private int _isLayingDownParameterHash = Animator.StringToHash("IsLayingDown");
    private int _isIdlingParameterHas = Animator.StringToHash("IsIdling");

    void Awake()
    {
        _animator = GetComponent<Animator>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(_bamboo);
        Assert.IsNotNull(_pawToHoldBamboo);
    }

    void IInputHandler.OnInputDown(InputEventData eventData)
    {
        Debug.Log("OnInputDown");        
        throw new System.NotImplementedException();
    }

    void IInputHandler.OnInputUp(InputEventData eventData)
    {
        Debug.Log("OnInputUp");
        SetEat();
        throw new System.NotImplementedException();
    }

    void Start ()
    {
        _originalPosition = this.transform.position;
        _bamboo.transform.SetParent(_pawToHoldBamboo);
        _bamboo.transform.localPosition = Vector3.zero;
	}

    #region Helper Methods
    private void SetEat()
    {
        _animator.SetBool(_isEatingParameterHash, true);
        _animator.SetBool(_isIdlingParameterHas, false);
        _animator.SetBool(_isLayingDownParameterHash, false);
    }

    private void SetIdle()
    {
        _animator.SetBool(_isEatingParameterHash, false);
        _animator.SetBool(_isIdlingParameterHas, true);
        _animator.SetBool(_isLayingDownParameterHash, false);
    }

    private void SetLayDown()
    {
        _animator.SetBool(_isEatingParameterHash, false);
        _animator.SetBool(_isIdlingParameterHas, false);
        _animator.SetBool(_isLayingDownParameterHash, true);
    }

    #endregion
}
