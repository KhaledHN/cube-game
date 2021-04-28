using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwich3 : MonoBehaviour {

    void OnTriggerEnter(Collider other) 
    {
        SceneManager.LoadScene(1);
    }

}
