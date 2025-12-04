using UnityEngine;

public class AfollowerB : MonoBehaviour
{
    public GameObject target;
    private Vector3 offset = new Vector3(0.06f,1.42f,0.57f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.transform.position + offset;
    }
}
