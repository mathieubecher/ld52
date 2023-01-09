using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailFish : MonoBehaviour
{
    [SerializeField] private GameObject noFishLog;
    [SerializeField] private GameObject breakLog;
    [SerializeField] private GameObject fishLeaveLog;
    private Animator m_animator;
    
    private static readonly int Fail = Animator.StringToHash("Fail");
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void DrawFailFish(FishingCap.PullUpLog _log)
    {
        noFishLog.SetActive(false);
        breakLog.SetActive(false);
        fishLeaveLog.SetActive(false);
        switch (_log)
        {
            case FishingCap.PullUpLog.BREAK :
                breakLog.SetActive(true);
                break;
            case FishingCap.PullUpLog.NOFISH :
                noFishLog.SetActive(true);
                break;
            case FishingCap.PullUpLog.FISHLEAVE :
                fishLeaveLog.SetActive(true);
                break;
        }
        m_animator.ResetTrigger(Fail);
        m_animator.SetTrigger(Fail);
        
        
    }
}
