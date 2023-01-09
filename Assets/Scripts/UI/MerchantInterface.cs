using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantInterface : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_upgradeResistance;
    [SerializeField] private TextMeshProUGUI m_upgradeStrength;
    [SerializeField] private TextMeshProUGUI m_upgradeRodLength;

    private Animator m_animator;
    private float m_openTimer = 0.0f;
    
    private bool m_open;
    public bool open => m_open;
    private static readonly int Close = Animator.StringToHash("Close");
    private static readonly int Open = Animator.StringToHash("Open");
    
    public delegate void OpenMerchantDelegate();
    public static event OpenMerchantDelegate OpenMerchant;
    
    public void UpgradeResitance()
    {
        GameManager.playerReplicateData.UpgradeResistance();
    }
    
    public void UpgradeStrength()
    {
        GameManager.playerReplicateData.UpgradeStrength();
    }
    
    public void UpgradeRodLength()
    {
        GameManager.playerReplicateData.UpgradeRodeLength();
    }
    
    void Awake()
    {
        ReplicateData.OnUpdateResistance += UpdateResistance;
        ReplicateData.OnUpdateRodLength += UpdateRodLength;
        ReplicateData.OnUpdateStrength += UpdateStrength;
        Catalog.OpenCatalog += CloseMerchant;
        Dialog.DialogOpenMerchant += DialogOpenMerchant;
        Dialog.DialogCloseMerchant += DialogCloseMerchant;

        m_animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        ReplicateData.OnUpdateResistance -= UpdateResistance;
        ReplicateData.OnUpdateRodLength -= UpdateRodLength;
        ReplicateData.OnUpdateStrength -= UpdateStrength;
        Catalog.OpenCatalog -= CloseMerchant;
        Dialog.DialogOpenMerchant -= DialogOpenMerchant;
        Dialog.DialogCloseMerchant += DialogCloseMerchant;
    }

    void Update()
    {
        if (m_open)
        {
            m_openTimer += Time.deltaTime;
        }
    }
    private void UpdateResistance(float _resistance, float _price)
    {
        m_upgradeResistance.text = "Upgrade Resistance : " + (_price < 0.0f ? "MAX" : _price + " $") ;
    }
    
    private void UpdateRodLength(float _rodLength, float _price)
    {
        m_upgradeRodLength.text = "Upgrade Rod Length : " + (_price < 0.0f ? "MAX" : _price + " $");
    }
    
    private void UpdateStrength(float _strength, float _price)
    {
        m_upgradeStrength.text = "Upgrade Strength : " + (_price < 0.0f ? "MAX" : _price + " $");
    }

    private void DialogOpenMerchant()
    {
        Debug.Log("Open merchant");
        m_animator.ResetTrigger(Close);
        m_animator.ResetTrigger(Open);
        
        Controller.OpenUI();
        m_animator.SetTrigger(Open);
        m_open = true;
        OpenMerchant?.Invoke();
        m_openTimer = 0.0f;
    }
    private void DialogCloseMerchant()
    {
        m_openTimer = 0.0f;
        m_animator.ResetTrigger(Close);
        m_animator.ResetTrigger(Open);
            
        Controller.CloseUI();
        m_animator.SetTrigger(Close);
        m_open = false;
    }
    private void CloseMerchant()
    {
        if (m_open)
        {
            m_openTimer = 0.0f;
            m_animator.ResetTrigger(Close);
            m_animator.ResetTrigger(Open);
            
            Controller.CloseUI();
            m_animator.SetTrigger(Close);
            m_open = false;
        }
    }
}
