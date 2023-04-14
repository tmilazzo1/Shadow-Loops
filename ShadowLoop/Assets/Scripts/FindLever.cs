using UnityEngine;

public class FindLever : MonoBehaviour
{
    Collider2D col;
    [SerializeField] ContactFilter2D filter;
    Collider2D[] results = new Collider2D[3];
    int colliderCount;
    Lever lever;
    bool buttonDown;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    public GameObject getClosestLever()
    {
        colliderCount = col.OverlapCollider(filter, results);
        if (colliderCount == 0) return null;

        GameObject closest = results[0].gameObject;
        if (colliderCount == 1)
        {
            return closest;
        }

        float closestDistance = Mathf.Sqrt((transform.position.x - results[0].transform.position.x) * (transform.position.x - results[0].transform.position.x) +
            (transform.position.y - results[0].transform.position.y) * (transform.position.y - results[0].transform.position.y));
        for (int i = 1; i < colliderCount; i++)
        {
            float newDistance = Mathf.Sqrt((transform.position.x - results[i].transform.position.x) * (transform.position.x - results[i].transform.position.x) +
            (transform.position.y - results[i].transform.position.y) * (transform.position.y - results[i].transform.position.y));
            if (newDistance < closestDistance)
            {
                closest = results[i].gameObject;
                closestDistance = newDistance;
            }
        }

        return closest;
    }

    private void Update()
    {
        if(getClosestLever())
        {
            if(!lever) lever = getClosestLever().GetComponent<Lever>();
            lever.GetComponent<Lever>().highlight(true);
            if (Input.GetAxisRaw("Fire1") == 1 && !buttonDown)
            {
                buttonDown = true;
                lever.flip();
            }else if(Input.GetAxisRaw("Fire1") != 1)
            {
                buttonDown = false;
            }
        }else if(lever)
        {
            lever.GetComponent<Lever>().highlight(false);
            lever = null;
        }
    }
}
