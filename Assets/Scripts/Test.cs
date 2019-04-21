using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Transform _squareTransform; 
    [SerializeField]
    private Transform _circleTransform;

    private Sequencer _sequencer;

    void Start()
    {
        var go = new GameObject();
        _sequencer =  go.AddComponent<Sequencer>();

        _sequencer.EnqueueSequence(new DOTweenDelegateSequence(x =>
        {
            x.Append(_squareTransform.DOBlendableMoveBy(new Vector2(1, 0), 1f));
        }));

        using (_sequencer.Aggregate<DOTweenDelegateSequence>())
        {
            _sequencer.EnqueueSequence(new DOTweenDelegateSequence(x =>
            {
                x.Append(_squareTransform.DOBlendableMoveBy(new Vector2(-1, 0), 1f));
            }));

            _sequencer.EnqueueSequence(new DOTweenDelegateSequence(x =>
            {
                x.Append(_squareTransform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f));
            }));
            _sequencer.EnqueueSequence(new DOTweenDelegateSequence(x =>
            {
                x.Append(_circleTransform.DOBlendableMoveBy(new Vector2(-1, 0), 1f));
                x.Append(_circleTransform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f));
            }));

            using (_sequencer.Aggregate<DOTweenDelegateSequence>())
            {
                _sequencer.EnqueueSequence(new DOTweenDelegateSequence(x =>
                {
                    x.Append(_circleTransform.GetComponent<SpriteRenderer>().DOColor(Color.red, 1f));
                    x.Append(_squareTransform.GetComponent<SpriteRenderer>().DOColor(Color.blue, 1f));
                }));
            }
        }

        _sequencer.EnqueueSequence(new DOTweenDelegateSequence(x =>
        {
            x.Append(_circleTransform.DOBlendableMoveBy(new Vector2(2, 0), 1f));
        }));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _sequencer.Complete();
    }
}
