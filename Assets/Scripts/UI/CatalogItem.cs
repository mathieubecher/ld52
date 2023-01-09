using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatalogItem : MonoBehaviour
{
    private Fish m_fish;
    [SerializeField] private TextMeshProUGUI m_fishNameRef;
    [SerializeField] private Image m_fishSpriteRef;
    [SerializeField] private TextMeshProUGUI m_nbCatchRef;
    
    void Awake()
    {
        GameManager.OnCatchFish += UpdateFish;
    }

    private void OnDestroy()
    {
        GameManager.OnCatchFish -= UpdateFish;
    }

    public void SetFish(Fish _fish)
    {
        m_fish = _fish;
        if (m_fish.masked) return;
        
        m_fishNameRef.text = _fish.fishName;
        m_fishSpriteRef.sprite = _fish.fishSprite;
        m_nbCatchRef.text = "";
    }

    public void UpdateFish(Fish _fish, int _nbCatch)
    {
        if (_fish.fishName == m_fish.fishName)
        {
            if (_nbCatch >= 1)
            {
                m_fishNameRef.text = _fish.fishName;
                m_fishSpriteRef.sprite = _fish.fishSprite;
                m_nbCatchRef.text = "";
            }
            SetNbCatch(_nbCatch);
        }
    }

    public void SetNbCatch(int _nbCatch)
    {
        if (_nbCatch == 0) return;
        m_nbCatchRef.text = ""+_nbCatch;
    }
}
