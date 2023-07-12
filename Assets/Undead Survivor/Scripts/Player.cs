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
        // fixedDeltaTime: ���� ������ �ϳ��� �Һ��� �ð�
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;

        // ��ġ �̵� (���� ��ġ + ���� ��ġ)
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        // ���ο� ��ǲ �ý����� �̿��� �Է�
        // �� ����� ���� fixedUpdate���� normailze�� �� �ʿ䰡 ����(�Ʒ� �� �ٷ� �� �ȴ�)
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
