using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class BgController : MonoBehaviour
{
    [SerializeField]
    private float _bgSpeed = 1.0f;

    [SerializeField]
    private float _bgPosX = -56.0f;

    private Camera _mainCamera;
    private float _backgroundWidth;

    private void Awake()
    {
        _mainCamera = Camera.main;

        // ����� �ʺ� ����մϴ�.
        //_backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * _bgSpeed * Time.deltaTime);
        // ���⼭  _mainCamera.orthographicSize * _mainCamera.aspect) �� ī�޶��� ���λ���� ���Ѵ�.
        if (transform.position.x < _bgPosX)
        {

            // ����� ���������� ��� �ʺ��� �� �踸ŭ �̵�
            transform.transform.localPosition = Vector3.zero;
          // transform.position = new Vector3(transform.position.x + _backgroundWidth * 3, transform.position.y, transform.position.z);
        }


    }
}
