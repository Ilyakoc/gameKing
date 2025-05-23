using UnityEngine;
using System.Collections.Generic;

public class Construction : MonoBehaviour
{
    [SerializeField] private GameObject buildObject;
    [SerializeField] private HitHandler loadArea;
    [SerializeField] private HitHandler buildArea;

    [SerializeField, Space(5)]
    private List<string> requiredToBuild;
    private bool prepareBuild;
    private bool playerBuilding;

    private List<Collider2D> finishedItems;

    private void Start()
    {
        buildObject.SetActive(false);

        finishedItems = new List<Collider2D>();
    }

    private void Update()
    {
        if (prepareBuild && playerBuilding) buildObject.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_FadeAmount"), buildObject.GetComponent<SpriteRenderer>().material.GetFloat(Shader.PropertyToID("_FadeAmount")) - Time.deltaTime);
    }

    private void OnEnable()
    {
        loadArea.hitTarget += LoadAreaChanged;
        buildArea.hitTarget += BuildAreaChanged;
    }

    private void OnDisable()
    {
        loadArea.hitTarget -= LoadAreaChanged;
        buildArea.hitTarget -= BuildAreaChanged;
    }

    private void LoadAreaChanged(GameObject item, bool stay)
    {
        string token = item.GetComponent<BuildItem>().Token;

        if (stay)
        {
            if (requiredToBuild.Contains(token))
            {
                requiredToBuild.Remove(token);
                finishedItems.Add(item.GetComponent<Collider2D>());
            }
        }
        else
        {
            requiredToBuild.Add(token);
            finishedItems.Remove(item.GetComponent<Collider2D>());
        }

        if (requiredToBuild.Count == 0) prepareBuild = true;
        else prepareBuild = false;
    }

    private void BuildAreaChanged(GameObject player, bool enter)
    {
        playerBuilding = enter;
        if (prepareBuild && playerBuilding)
        {
            buildObject.SetActive(true);
            buildObject.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_FadeAmount"), 1);

            finishedItems.ForEach((Collider2D col) => col.enabled = false);
            OnDisable();
        }
    }
}