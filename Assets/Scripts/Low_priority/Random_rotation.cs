using UnityEngine;

public class Random_rotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var current_rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(current_rot.x, current_rot.y, Random.Range(0f, 360f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
