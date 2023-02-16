using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private FpsGun ammoNumber;

    void Update()
    {
        text.text = Mathf.Clamp(ammoNumber.ammo, 0, 30).ToString() + "/30";
    }
}
