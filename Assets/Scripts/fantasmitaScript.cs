using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fantasmitaScript : MonoBehaviour
{
    public int team = 0;
    public float fadeTime = 1f;
    public bool flip;
    public float speed = 2f;
    public GameObject DeathExplosion;
    Animator anim;
    SpriteRenderer spr;
    // Start is called before the first frame update
    void Start(){
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        if (flip){
            spr.flipX = flip;
        }
        if (team == 0){
            anim.Play("muerteGato");
        } else {
            anim.Play("muerteCoco");
        }

        Instantiate(DeathExplosion, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update(){
        // estupidez para hacerlo ir para arriba y desaparecer.
        // Tal vez reemplazar por animación
        transform.position += (Vector3.up * speed + Vector3.right * Mathf.Sin(Time.time * 7f)) * Time.deltaTime;
        Color tmp = spr.color;
        tmp.a -= Time.deltaTime / fadeTime;
        spr.color = tmp;
    }
}
