using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.UI;

public class Enemy : CharacterScript
{
    // Start is called before the first frame update

    //�ʿ���  ������Ʈ
    protected float dir;

    protected bool _isDead = false;
    protected int randomPatrol = 0;
    protected float _patrolTime = 0.0f;
    protected float _renewTime = 3.0f;
    protected float _serachTargetDistance = 3.0f;
    protected float _attackTargetDistance = 1.5f;
    protected GameObject _target;
    protected EnemyState _state = EnemyState.PATROL;
    protected Image _hpBar_front;

    protected bool _isRight = false;
    protected bool _isTurn = false;



    // hp�� 
    public GameObject _prefab_HpBar;    
    RectTransform _hpBar;
    [SerializeField]
    protected float height = -1.0f;
    [SerializeField]
    protected float width = 0.03f;




    protected enum EnemyState
    {
        PATROL,
        TRACE,
        ATTACK,
        DEAD,
        HIT
    }

    //������Ʈ �ٲ��ִ� �Լ�.
    protected void SetState(int stateNum)
    {
        _state = (EnemyState)stateNum;
    }



    private void Start()
    {
        _target = GameManager.Instance.Player;
        // ü�¹� �����ϴºκ�
        GameObject hpObj = Instantiate(_prefab_HpBar, GameManager.Instance.HpBarPanel);
        _hpBar = hpObj.GetComponent<RectTransform>();
        _curHp = _maxHp*0.3f;
        _hpBar_front = hpObj.transform.GetChild(0).GetComponent<Image>();
    }

    protected override void Update()
    {

        Debug.Log("STATE: " + _state);
        CheckTargetDistance();

        FollowHpBar();

        StateUpdate();
        base.Update();

    }
    protected void CheckTargetDistance()
    {


        if (_curHp < 0.1f)
        {
            SetState(EnemyState.DEAD);
            if (!_isDead)
            {
                Vector3 newPosition = transform.position;
                newPosition.y -= 0.4f;
                transform.position = newPosition;
                _velocity.y = 0;
            }
            _velocity.y = 0;
            return;
        }

        if (Vector3.Distance(_target.transform.position, transform.position) > _serachTargetDistance)
            SetState(EnemyState.PATROL);

        else if (Vector3.Distance(_target.transform.position, transform.position) < _serachTargetDistance)
        {

            if (Vector3.Distance(_target.transform.position, transform.position) < _attackTargetDistance )
            {
                if(!_isAttack)
                    SetState(EnemyState.ATTACK);
            }
            else
            {
                SetState(EnemyState.TRACE);
            }

        }
    }

    // player�� sendDamage�� ȣ���ϱ����� public���� �־���.
    public void HitControl()
    {
        if (_state == EnemyState.DEAD) return;
        SetState(EnemyState.HIT);
    }
    protected void PatrolControl()
    {
        if (_state == EnemyState.TRACE) return;
        if (_state == EnemyState.ATTACK) return;
        if (_state == EnemyState.DEAD) return;
        if (_state == EnemyState.HIT) return;
        if (_isAir) return;

        _patrolTime += Time.deltaTime;
        if (_patrolTime > _renewTime)
        {
            _patrolTime -= _renewTime;
            randomPatrol = Random.Range(0, 3);
            _isTurn = false;
        }

        Vector2 pos = _boxCollider2D.bounds.center;
        pos.x = (_isRight) ? _boxCollider2D.bounds.max.x : _boxCollider2D.bounds.min.x;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 1000.0f, _floorLayer);
        //������ ��������  ���׺��� ������ ���ư�����
        if (hit.point.y < _boxCollider2D.bounds.min.y - 0.1f)
        {
            _isTurn= true;
        }
        if(_isTurn)
        {
            if (randomPatrol == 0)
            {
                _isRight = false;
                _velocity.x = 0.1f;
            }
            else if (randomPatrol == 1)
            {
                _isRight = true;
                _velocity.x = -0.1f;
            }
        }
        else
        {
            //���� ���϶� ��/�� ���� �ϳ� �����ؼ� 1�� �Ȱ� Idle�� ����
            //0�ϋ� ��  / 1�ϋ� ��
            if (randomPatrol == 0)
            {
                _isRight = false;
                _velocity.x = -0.1f;

            }
            else if (randomPatrol == 1)
            {
                _isRight = true;
                _velocity.x = 0.1f;
            }
            else if (randomPatrol > 1)
            {
                _velocity.x = 0;
            }
        }

        _velocity.y = 0;
    }
    protected void TraceControl()
    {
        Vector2 pos = _boxCollider2D.bounds.center;
        pos.x = (_isRight) ? _boxCollider2D.bounds.max.x : _boxCollider2D.bounds.min.x;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 1000.0f, _floorLayer);
        //������ ��������  ���׺��� ������ ���ư�����
        if (hit.point.y < _boxCollider2D.bounds.min.y - 0.1f)
        {
            if(_velocity.x<0)
            {
                _velocity.x = 1.0f;
            }
            else if(_velocity.x>0)
            {
                _velocity.x = -1.0f;
            }
        }
        else
        {
            float distanceX = _target.transform.position.x - transform.position.x;
            if (distanceX != 0)
            {
                dir = Mathf.Sign(distanceX) * 0.5f;
                _velocity.x = dir;
            }
        }
        _velocity.y = 0;
    }
    protected void AttackControl()
    {
            _velocity.x = 0;
            _animator.SetTrigger("Attack");
        
    }
    protected void StateUpdate()
    {
        switch (_state)
        {
            case EnemyState.PATROL:
                {
                    PatrolControl();
                }
                break;
            case EnemyState.TRACE:
                {
                    TraceControl();
                }
                break;
            case EnemyState.ATTACK:
                {
                    if (!_isAttack)
                    {
                        _velocity.y= 0;
                        AttackControl();
                    }
                }
                break;
            case EnemyState.DEAD:
                {
                    Dead();
                }
                break;
            case EnemyState.HIT:
                {
                        _animator.SetTrigger("Hit");
                }
                break;

        }
    }

    protected void SetState(EnemyState state)
    {
        _state = state;
    }

    protected void EndAttack()
    {
        _isAttack = false;
        print("EndAttack");
    }
    protected void StartAttack()
    {
        _isAttack = true;
        print("StartAttack");
    }

    private void FollowHpBar()
    {
       

        if(_spriteRenderer.flipX)
        {
            width = -0.15f;
        }
        else if(!_spriteRenderer.flipX)
        {
            width = 0.05f;
        }
        else if(_velocity.x ==0)
        {
            width = 0.05f;
        }
        _hpBar_front.fillAmount = (float)_curHp / (float)_maxHp;
        Vector3 _hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x - width, transform.position.y + height, 0));
        _hpBar.position = _hpBarPos;


    }
    protected void Dead()
    {
        if(!_isDead)
        {
            _isDead = true;
            _animator.SetTrigger("Dead");
            GameManager.Instance.IncreaseKillCount();
            //gameObject.transform.position
        }
     
    }
    protected void EndDead()
    {
        gameObject.SetActive(false);
        _hpBar.gameObject.SetActive(false);
    }


    protected void OnDrawGizmos()
    {
        //���� �����ݶ��̴� �����
        base.OnDrawGizmos();

        
        if (_boxCollider2D == null) return;

        // ��������
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_boxCollider2D.bounds.center, _serachTargetDistance);

        //���ݹ���
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_boxCollider2D.bounds.center, _attackTargetDistance);

        //�������� üũ������
        Gizmos.color = Color.blue;
        Vector2 pos = _boxCollider2D.bounds.center;
        pos.x = (_isRight) ? _boxCollider2D.bounds.max.x : _boxCollider2D.bounds.min.x;

        Gizmos.DrawLine(pos, pos + Vector2.down * 10.0f);
    }

}
