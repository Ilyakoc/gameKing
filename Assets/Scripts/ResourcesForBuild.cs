using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesForBuild : MonoBehaviour
{
    public GameObject build;
    public int countResType_1;
    public int countResType_2;

    private int countRes_1 = 0;
    private int countRes_2 = 0;
    private bool activated =false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (other.tag == "ResourceType_1")
        {
            countRes_1++;
            Debug.Log(countRes_1);
        }

        if (other.tag == "ResourceType_2")
        {
            countRes_2++;
            Debug.Log(countRes_2);
        }

        if (countRes_1 == countResType_1 && countRes_2 == countResType_2)
        {
            activated = true;
            build.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "ResourceType_1")
        {
            countRes_1--;
            activated = false;
        }

        if (other.tag == "ResourceType_2")
        {
            countRes_2--;
            activated = false;
        }
    }
}
