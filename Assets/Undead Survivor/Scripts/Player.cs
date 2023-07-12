using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;

    Rigidbody2D rigid;

    SpriteRenderer spriter;

    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        // fixedDeltaTime: 물리 프레임 하나가 소비한 시간
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;

        // 위치 이동 (현재 위치 + 다음 위치)
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        // 새로운 인풋 시스템을 이용한 입력
        // 이 방법을 쓰면 fixedUpdate에서 normailze를 할 필요가 없다(아래 한 줄로 다 된다)
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

}
