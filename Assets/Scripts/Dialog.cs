using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Dialog : MonoBehaviour
{
    public List<String> m_dialog;
    public TextMeshPro m_textRef;
    public GameObject m_backgroundRef;
    public GameObject m_boxRef;
    public GameObject m_tutorial;
    private Collider2D m_collider;
    public int i = 0;
    public Collider2D player;
    
    public delegate void OpenMerchantDelegate();
    public static event OpenMerchantDelegate DialogOpenMerchant;
    public static event OpenMerchantDelegate DialogCloseMerchant;
    void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        Controller.OnInteract += Action;
    }

    private void OnDestroy()
    {
        Controller.OnInteract -= Action;
    }

    private void Action()
    {
        if (player != null)
        {
            i++;
            if (i > m_dialog.Count) i = 0;
            if(i > 0) { 
                m_textRef.text = m_dialog[i - 1 ];
            }
            if (i == 0 || m_dialog[ i - 1 ] == "")
            {
                m_textRef.gameObject.SetActive(false);
                m_backgroundRef.SetActive(false);
            }
            else if (m_dialog[i - 1] == "[OPEN MERCHANT]")
            {
                m_tutorial.SetActive(false);
                m_textRef.gameObject.SetActive(false);
                m_backgroundRef.SetActive(false);
                DialogOpenMerchant?.Invoke();
            }
            else if (m_dialog[i - 1] == "[CLOSE MERCHANT]")
            {
                m_tutorial.SetActive(false);
                m_textRef.gameObject.SetActive(false);
                m_backgroundRef.SetActive(false);
                DialogCloseMerchant?.Invoke();
                Action();
                return;
            }
            else
            {
                m_tutorial.SetActive(false);
                m_textRef.gameObject.SetActive(true);
                m_backgroundRef.SetActive(true);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if ((m_collider.Distance(player).distance) > 0)
            {
                player = null;
                i = 0;
                m_textRef.gameObject.SetActive(false);
                m_backgroundRef.SetActive(false);
                m_tutorial.SetActive(false);
            }
        }

        if (m_textRef.text != "" && m_textRef.textBounds.size.x > 0.0f &&  m_textRef.textBounds.size.y > 0.0f)
        {
            m_backgroundRef.transform.localScale = new Vector3(m_textRef.textBounds.size.x + 0.4f, m_textRef.textBounds.size.y + 0.4f, 0);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.gameObject.name + " " + other.tag);
        if ((other.tag == "Player"))
        {
            player = other;
            m_tutorial.SetActive(true);
        }
    }   
}