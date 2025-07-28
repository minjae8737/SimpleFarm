using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public GameObject marker;

    void Start()
    {

    }


    void Update()
    {

    }

    public void OnMarker()
    {
        marker.SetActive(true);
    }

    public void OffMarker()
    {
        marker.SetActive(false);
    }

}
