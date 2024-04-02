using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimEnd : MonoBehaviour
{
    // Serializable and Public
    [SerializeField] Animator anim;

    // Private
    AnimatorStateInfo asi;

    // Static

    // Defined Function

    // System Function
    void Update()
    {
        asi = anim.GetCurrentAnimatorStateInfo(0);
        if (asi.normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
    }
}
