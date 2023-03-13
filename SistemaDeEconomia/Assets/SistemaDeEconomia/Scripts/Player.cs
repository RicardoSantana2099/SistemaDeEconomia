using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 1f;
    Rigidbody2D rb;
    bool isMoving = false;
    float x, y;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        isMoving = (x != 0f || y != 0f);
    }

    private void FixedUpdate()
    {
        if(isMoving)
        {
            rb.position += new Vector2(x, y) * speed * Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.collider.tag;

        if(tag.Equals("coin"))
        {
            //Añade monedas

            GameDataManager.AddCoins(32);

          #if UNITY_EDITOR
            if(Input.GetKey(KeyCode.C))
                GameDataManager.AddCoins(179);
#endif

            Destroy(other.gameObject);
        }
    }
}
