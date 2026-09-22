using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 degreesPerSecond = new Vector3(0f, 180f, 0f);
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(degreesPerSecond * Time.deltaTime);
    }
}
