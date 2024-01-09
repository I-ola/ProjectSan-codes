using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerWeaponManager;

public class AmmoWidget : MonoBehaviour
{
    [HideInInspector] public TMP_Text ammoText;
    [HideInInspector] public TMP_Text availableAmmo;
    public Image weaponImage;
    [HideInInspector] public Weapon weapon;
    public PlayerWeaponManager activeWeapon;
    [HideInInspector] public Color defaultColor;

    public bool NoWeaponAssigned
    {
        get
        {
            return weapon == null;
        }
    }
    //this is function is used to show the weapon and ammo Ui
    private void Start()
    {
        defaultColor = weaponImage.color;
    }

    private void Update()
    {
        if (weapon == null)
        {
            this.gameObject.SetActive(false);
            //Debug.Log(this.gameObject.name + "has no weapon");
            return;
        }
        else if(weapon != null)
        {
            this.gameObject.SetActive(true);
            Refresh();
            //Debug.Log(this.gameObject.name + "has  weapon");
        }

           
    }
    public void Refresh()
    {
        ammoText.text = weapon.ammoCount.ToString() + " / " + weapon.magazineSize.ToString();
        availableAmmo.text = TotalAmmoToShow().ToString();
        weaponImage.sprite = weapon.weaponSprite;
    }

    int TotalAmmoToShow()
    {
        if (weapon.isAssault)
            return activeWeapon.assaultWeaponAmmo;
        if (weapon.isPistol)
            return activeWeapon.pistolWeaponAmmo;
        if (weapon.isSMG)
            return activeWeapon.smgWeaponAmmo;
        return 0;
    }



}
