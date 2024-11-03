using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class HealthVar : MonoBehaviour

{
    public Slider slider;
    public void Update()
    {
        GameObject CharModel = GameObject.Find("Player");
        PlayerMovement CharControl = CharModel.GetComponent<PlayerMovement>();
        float health = CharControl.Health;
        slider.value = health;
    }

}
