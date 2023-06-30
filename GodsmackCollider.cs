using UnityEngine;

public class GodsmackCollider : MonoBehaviour
{
    Godsmack godsmack;
    GameObject mainCamera;
    [SerializeField] LayerMask obstacleLayers;
    private void Start()
    {
        godsmack = transform.parent.GetComponent<Godsmack>();
        mainCamera = Camera.main.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 || other.gameObject.layer == 20)
        {
            if (!Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, Vector3.Distance(godsmack.transform.position, other.transform.position), obstacleLayers))
            {
                // The While is used to get a parent with Target in case we hit its child
                Transform targetHolder = other.transform;
                Transform currentTargetHolder = other.transform;
                while (currentTargetHolder.parent != null)
                {
                    if (currentTargetHolder.parent.gameObject.GetComponent<Target>())
                    {
                        targetHolder = currentTargetHolder.parent;
                        break;
                    }
                    currentTargetHolder = currentTargetHolder.parent;
                }
                targetHolder.GetComponent<Target>().PerformAction(godsmack.damage);

                if (targetHolder.tag == "Enemy")
                {
                    DefaultEnemyClass defaultEnemyClass = targetHolder.GetComponent<DefaultEnemyClass>();
                    if (defaultEnemyClass.health <= 0)
                    {
                        defaultEnemyClass.FullyGib(targetHolder.position);
                    }
                }
            }            
        }
    }
}
