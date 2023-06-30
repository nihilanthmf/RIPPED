using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void OpeningClosing()
    {
        anim.SetBool("toOpen", !anim.GetBool("toOpen"));
    }
}
