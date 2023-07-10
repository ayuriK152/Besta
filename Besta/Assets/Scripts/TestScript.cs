using UnityEngine;
using static Define;

public class TestScript : MonoBehaviour
{
    AudioSource _music;
    long _previousTime = -1;
    void Start()
    {
        Managers.Input.MouseAction -= MouseEventTestMethod;
        Managers.Input.MouseAction += MouseEventTestMethod;
        _music = GameObject.Find("@Manager").GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    void MouseEventTestMethod(MouseEvent mouseEvent, MousePointer mousePointer)
    {
        if (mousePointer == MousePointer.Right)
            _music.Play();
        else
            Debug.Log(_music.timeSamples);
    }
}
