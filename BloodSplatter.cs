using UnityEngine;
using System.Collections;

public class BloodSplatter : MonoBehaviour
{
    bool toStartDestroyingBlood;
    void Start()
    {
        StartCoroutine(DestroyingBlood());
    }

    IEnumerator DestroyingBlood()
    {
        yield return new WaitForSeconds(5);
        toStartDestroyingBlood = true;
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (toStartDestroyingBlood)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0, 0), 3 * Time.deltaTime);
        }
    }
}
