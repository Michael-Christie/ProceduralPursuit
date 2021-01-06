using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raise : MonoBehaviour
{
    public float timeToWait;
    public bool canMove = false;

    public void StartRaise()
    {
        StartCoroutine("Rise");
    }

    IEnumerator Rise()
    {
        yield return new WaitForSeconds(timeToWait);
        canMove = true;
    }

    private void FixedUpdate()
    {
        if (canMove)
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, 0, transform.position.z), Time.deltaTime);

        if (transform.position.y > -.05f)
            RemoveComponent(RandomNumber.Range(0,1f));
    }

    IEnumerator RemoveComponent(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(this);
    }
}
