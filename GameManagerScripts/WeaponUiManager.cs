using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponUiManager : MonoBehaviour
{
    public List<AmmoWidget> widget = new List<AmmoWidget>();
    public PlayerWeaponManager weaponManager;
    public Color selectedWeaponColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        CurrentSelectedWeapon(weaponManager.GetActiveWeaponList());
       
    }

    public void AddingWeapon(GameObject obj)
    {
        for(int i = 0; i < widget.Count; i++)
        {
            if (widget[i].NoWeaponAssigned)
            {
                widget[i].weapon = obj.GetComponent<Weapon>();
                widget[i].gameObject.SetActive(true);
                //Debug.Log("Added a weapon at" + widget[i].gameObject.name);
                return;
            }
        }
    }

    public void RemoveWeapon(GameObject obj)
    {
        for(int i = 0 ; i < widget.Count; i++)
        {
            if (!widget[i].NoWeaponAssigned)
            {
                if (widget[i].weapon.gameObject == obj)
                {
                    widget[i].weapon = null;
                }
            }
        }
    }

    public void CurrentSelectedWeapon(GameObject weapon)
    {
        if (weapon == null)
            return;

        for (int i = 0; i < widget.Count; i++)
        {
            if (widget[i].weapon == weapon.GetComponent<Weapon>())
            {
                widget[i].weaponImage.color = selectedWeaponColor;
            }
            else
            {
                widget[i].weaponImage.color = widget[i].defaultColor;
            }
        }
    }
}
