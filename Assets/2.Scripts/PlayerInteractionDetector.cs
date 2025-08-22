using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionDetector : MonoBehaviour
{
    public UIBtnType uiBtnType;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag != "Player")
            return;

        GameManager.instance.uiManager.SetInteractBtn(uiBtnType);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag != "Player")
            return;

        GameManager.instance.uiManager.OffInteractBtn();
    }

}
