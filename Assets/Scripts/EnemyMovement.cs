using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float speed=1f;

    Rigidbody2D body;
    BoxCollider2D flipCollider;

    bool flipped = false;
    // Start is called before the first frame update
    void Start()
    {
        this.body = GetComponent<Rigidbody2D>();
        this.flipCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector2(speed, 0f);

       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (!flipped)
        //{
        //    flipped = true;
        //    speed *= -1;
        //    transform.localScale = new Vector2(-transform.localScale.x, 1f);
        //}
        //else
        //    flipped = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        flipped = true;
        speed *= -1;
        transform.localScale = new Vector2(-transform.localScale.x, 1f);
    }
}
