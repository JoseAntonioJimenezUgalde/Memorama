using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barril : MonoBehaviour
{
    public GameObject panelDeWin;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ActivarPanel());
        }
    }

    IEnumerator ActivarPanel()
    {
        yield return new WaitForSeconds(1);
        panelDeWin.SetActive(true);
    }
}
