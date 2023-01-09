using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccessFish : MonoBehaviour
{
    [SerializeField] private Image m_fishSprite;
    [SerializeField] private TextMeshProUGUI m_fishName;
    [SerializeField] private TextMeshProUGUI m_fishPrice;
    [SerializeField] private GameObject m_newFish;
    private Animator m_animator;
    
    private static readonly int Skip = Animator.StringToHash("Skip");
    private static readonly int Catch = Animator.StringToHash("Catch");
    private bool m_canSkip = false;

    public void EnableCanSkip()
    {
        m_canSkip = true; 
    }
    
    public void DisableCanSkip()
    {
        m_canSkip = false;
    }

    public void FinishShowingResult()
    {
        GameManager.instance.isShowingResult = false;
    }
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        Controller.OnClick += ReceiveClickInput;
    }

    private void ReceiveClickInput()
    {
        if (m_canSkip)
        {
            m_canSkip = false;
            m_animator.SetTrigger(Skip);
        }
    }

    public void DrawSuccessFish(Fish _fish, int _nbCatch)
    {
        m_fishSprite.sprite = _fish.fishSprite;
        m_fishName.text = "You harvested a " + _fish.fishName + "!";
        m_fishPrice.text = _fish.price + "$";
        m_newFish.SetActive(_nbCatch == 1);
        m_animator.ResetTrigger(Catch);
        m_animator.ResetTrigger(Skip);
        m_animator.SetTrigger(Catch);
    }
}
