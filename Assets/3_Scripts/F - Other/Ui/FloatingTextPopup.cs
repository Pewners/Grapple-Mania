using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingTextPopup : MonoBehaviour
{
    public TextMeshProUGUI text_content;
    public float moveUpDistance;
    public float lifeTime;

    public void Setup(string content)
    {
        Setup(content, moveUpDistance, lifeTime);
    }

    public void Setup(string content, float _moveUpDistance, float _lifeTime)
    {
        text_content.SetText(content);
        moveUpDistance = _moveUpDistance;
        lifeTime = _lifeTime;

        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        Vector3 endPos = transform.position + transform.up * moveUpDistance;
        transform.DOMove(endPos, lifeTime);

        yield return new WaitForSeconds(lifeTime * 0.5f);

        text_content.DOFade(0, lifeTime * 0.5f);

        yield return new WaitForSeconds(lifeTime * 0.5f);

        Destroy(gameObject);
    }
}
