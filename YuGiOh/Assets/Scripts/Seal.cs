using UnityEngine;
using System.Collections;

public class Seal : MonoBehaviour
{
    bool expand = false;
    int count = 0;

    GameObject TheSeal;

    // Use this for initialization
    void Start () {
	
	}

    IEnumerator OnSelect()
    {
        yield return new WaitForSeconds(2);

        TheSeal = (GameObject)Instantiate(GameObject.Find("Seal"), new Vector3(0, -2f, 0), GameObject.Find("Seal").transform.rotation);

        for (float i = 0.1f; i < 50f; i += 0.1f)
        {
            Vector3 temp = new Vector3(i, 0.00001f, i);
            TheSeal.transform.localScale = temp;
        }

    }

    void Update()
    {
        if(expand == true)
        {
            if(TheSeal.transform.localScale.x > 5f)
            {
                expand = false;
            }

            TheSeal.transform.localScale += new Vector3(0.1f, 0.0001f, 0.1f);
        }
    }
}
