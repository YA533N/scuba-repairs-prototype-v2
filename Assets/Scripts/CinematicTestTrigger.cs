using UnityEngine;

public class CinemaTest : MonoBehaviour
{
    public KeyCode testKey = KeyCode.C;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            CameraController.Instance.PlayCinematic(4f);
        }
        
    }
}
