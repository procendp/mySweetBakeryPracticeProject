using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCollider : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Customer")) 
        {
            GameManager.instance.isThereCustomerInHall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Customer"))
        {
            GameManager.instance.isThereCustomerInHall = false;
        }
    }
}
