using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DOTweenSequence : ISequence
{
    private bool _isComplete;
    protected Sequence Sequence { get; private set; }

    bool ISequence.IsComplete => _isComplete;

    void ISequence.Start()
    {
        Sequence = DOTween.Sequence();
        InternalStart(Sequence);
        Sequence.AppendCallback(() => { _isComplete = true; });
    }

    void ISequence.Complete()
    {
        Sequence.Complete();
    }

    protected abstract void InternalStart(Sequence sequence);
}
