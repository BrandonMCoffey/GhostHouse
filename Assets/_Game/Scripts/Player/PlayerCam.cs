using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float camSpeed = 0.5f;

    private void Update() {
        float xAxisValue = UserInput.Horizontal * -camSpeed;
        float zAxisValue = UserInput.Vertical * -camSpeed;

        //transform.position += new Vector3(transform.forward * xAxisValue, transform.forward * xAxisValue,

        transform.position += transform.right * -xAxisValue;
        transform.position += transform.up * -zAxisValue;
        //transform.position += new Vector3(transform.)
    }
}