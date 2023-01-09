    using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Catalog : MonoBehaviour
{
    public GameObject m_itemPrefab;
    public GameObject m_wrapper;
    public float m_margin = 10f;
    public float m_sizeWidht = 40f;
    public float m_sizeHeight = 40f;
    public int m_maxItemByLine = 4;
    
    private Animator m_animator;
    private bool m_open = false;

    void Start()
    {
        Debug.Log("catalog");
        DrawCatalog();
    }

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        Controller.OnEscape += EnableCatalog;
        MerchantInterface.OpenMerchant += OpenMerchant;
    }

    private void OnDestroy()
    {
        Controller.OnEscape -= EnableCatalog;
        MerchantInterface.OpenMerchant -= OpenMerchant;
    }

    private static readonly int Close = Animator.StringToHash("Close");
    private static readonly int Open = Animator.StringToHash("Open");
    
    public delegate void OpenCatalogDelegate();
    public static event OpenCatalogDelegate OpenCatalog;
    private void EnableCatalog()
    {
        m_animator.ResetTrigger(Close);
        m_animator.ResetTrigger(Open);
        if (m_open)
        {
            Controller.CloseUI();
            m_animator.SetTrigger(Close);
            m_open = false;
        }
        else
        {
            Controller.OpenUI();
            m_animator.SetTrigger(Open);
            m_open = true;
            OpenCatalog?.Invoke();
        }
    }
    private void OpenMerchant()
    {
        m_animator.ResetTrigger(Close);
        m_animator.ResetTrigger(Open);
        if (m_open)
        {
            Controller.CloseUI();
            m_animator.SetTrigger(Close);
            m_open = false;
        }
    }

    public void DrawCatalog()
    {
        List<Fish> fishes = GameManager.instance.fishes;
        for (int i = 0; i < fishes.Count; ++i)
        {
            GameObject item = Instantiate(m_itemPrefab, m_wrapper.transform);
            var rect = item.GetComponent<RectTransform>();
            float width = m_sizeWidht;
            float height = rect.rect.height * m_sizeWidht / rect.rect.width;
            var position = new Vector2((m_margin + width) * (i % m_maxItemByLine), -(m_margin + height) * math.floor((float)i / m_maxItemByLine));
            position += new Vector2(width / 2f + m_margin, -height / 2f - m_margin);
            rect.localScale = rect.localScale * width / rect.rect.width;
            
            rect.anchoredPosition = position;
            item.GetComponent<CatalogItem>().SetFish(fishes[i]);
        }
    }
}
