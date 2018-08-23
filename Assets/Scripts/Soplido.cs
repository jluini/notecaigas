using UnityEngine;
using System.Collections;

public class Soplido : MonoBehaviour {

    public float initialRadius = 1f;
    public float growthSpeed = 1f;

    float radius;

    void Start()
    {
        radius = initialRadius;
        transform.localScale = Scale(radius);
    }

    Vector3 Scale(float radius)
    {
        return new Vector3(radius, radius, 1f);
    }

    void Update()
    {
        if(growthSpeed != 0f)
        {
            radius += growthSpeed * Time.deltaTime;
            transform.localScale = Scale(radius);
        }
    }

}
