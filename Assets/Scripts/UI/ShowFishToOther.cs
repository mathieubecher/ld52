using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFishToOther : MonoBehaviour
{
    private SpriteRenderer m_renderer;

    private Animator m_animator;
    // Start is called before the first frame update
    void Start()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
    }

    public void ShowFish(string _fishName)
    {
        Fish fish = GameManager.instance.FindFish(_fishName);
        if (fish == null) return;
        
        var oldSize = m_renderer.sprite.bounds.size.y;
        var newSize = fish.fishSprite.bounds.size.y;
        m_renderer.sprite = fish.fishSprite;
        m_renderer.transform.localScale = (oldSize/newSize) * m_renderer.transform.localScale;
        m_animator.SetTrigger("Show");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
