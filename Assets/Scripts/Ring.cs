using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterAnim());
    }

    private IEnumerator DestroyAfterAnim()
    {
        yield return new WaitForSeconds(2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
