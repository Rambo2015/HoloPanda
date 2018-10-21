using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.Assertions;
//using HoloToolkit.Unity.Speech;

[RequireComponent(typeof(Animator))]
public class Panda : MonoBehaviour, IInputHandler/*, IFocusable, ISpeechHandler*/
{
    private Animator _animator;
    private Vector3 _originalPosition;
    private Vector3 _bambooInitialPosition;
    [Tooltip("The GameObject that the Panda will eat")]
    [SerializeField] private GameObject _bamboo;
    [Tooltip("The Panda Paw to hold the food/bamboo")]
    [SerializeField] private Transform _pawToHoldBamboo;  // 'PANDA/new_Bone009' in the model is the left paw
    [SerializeField] private AudioSource _audioSource;

    // Animator Hashes, for efficiency
    private int _isEatingParameterHash = Animator.StringToHash("IsEating");
    private int _isLayingDownParameterHash = Animator.StringToHash("IsLayingDown");
    private int _isIdlingParameterHas = Animator.StringToHash("IsIdling");

    private bool _isEating = false;
    private bool _isLayingDown = false;
    private bool _isIdling = false;

    [SerializeField] private AudioClip _bearBreath;
    [SerializeField] private AudioClip _bearSounds;
    [SerializeField] private AudioClip _chewing;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(_animator);
        Assert.IsNotNull(_bamboo);
        Assert.IsNotNull(_pawToHoldBamboo);
        Assert.IsNotNull(_audioSource);
        Assert.IsNotNull(_bearBreath);
        Assert.IsNotNull(_bearSounds);
        Assert.IsNotNull(_chewing);
    }

    void Start()
    {
        _bambooInitialPosition = _bamboo.transform.position;
        _originalPosition = this.transform.position;
        SetIdle();
    }

    public void SetBamboo(GameObject bamboo)
    {
        if (_bamboo != null)
        {
            _bamboo = bamboo;
        }
    }

    private void ShowBamboo()
    {
        _bamboo.gameObject.SetActive(true);
    }

    private void HideBamboo()
    {
        _bamboo.gameObject.SetActive(false);
    }

    void IInputHandler.OnInputDown(InputEventData eventData)
    {
        //Debug.Log("OnInputDown");                
    }

    void IInputHandler.OnInputUp(InputEventData eventData)
    {
        //Debug.Log("OnInputUp");
        if (_isIdling)
        {
            SetEat();            
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

    #region Helper Methods
    public void SetEat()
    {
        _isEating = true;
        _isIdling = false;
        _isLayingDown = false;
        SetAnimatorParameters();        
        StartCoroutine(ShowAndHideBamboo());
        StartCoroutine(PlayClip(_chewing, 6f));
    }

    IEnumerator ShowAndHideBamboo()
    {
        _bamboo.gameObject.SetActive(true);
        float animLength = 9.5f;
        yield return new WaitForSeconds(animLength);
        _bamboo.gameObject.SetActive(false);
    }

    public void SetIdle()
    {
        HideBamboo();
        _isEating = false;
        _isIdling = true;
        _isLayingDown = false;
        SetAnimatorParameters();
        StartCoroutine(PlayClip(_bearSounds));
    }

    public void SetLayDown()
    {
        HideBamboo();
        _isEating = false;
        _isIdling = false;
        _isLayingDown = true;
        SetAnimatorParameters();
        StartCoroutine(PlayClip(_bearBreath));
    }

    private void SetAnimatorParameters()
    {
        _animator.SetBool(_isEatingParameterHash, _isEating);
        _animator.SetBool(_isIdlingParameterHas, _isIdling);
        _animator.SetBool(_isLayingDownParameterHash, _isLayingDown);
    }

    IEnumerator PlayClip(AudioClip clip, float delayToStartInSeconds = 0f)
    {
        if (clip == null) yield break;

        _audioSource.Stop();
        yield return new WaitForSeconds(delayToStartInSeconds);
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    #endregion
}
