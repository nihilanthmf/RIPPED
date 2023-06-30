using UnityEngine;

public class Button : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Press()
    {
        print("pressed");
    }
}
