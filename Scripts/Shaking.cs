using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaking : MonoBehaviour {
    public Transform _cam;
    public float pauseTime = 1f;
    float times = 1f;
    float pause = 1f;
	
	void OnEnable () {
        CancelInvoke();
        pause = 0f;
        Shot();
        Invoke("Block", pauseTime);
    }

    //Sheyk Effect
    void Shot()
    {
        if (times != pause)
        {
            Quaternion rot = Quaternion.Euler (Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            _cam.transform.localRotation = Quaternion.Slerp(_cam.transform.localRotation, rot, 0.5f);
            Invoke("Shot", 0.01f);
        } else
        {
            _cam.transform.localRotation = Quaternion.identity;
        }
    }

    void Block()
    {
        pause = pauseTime;
        times = pauseTime;
        enabled = false;
    }
}
