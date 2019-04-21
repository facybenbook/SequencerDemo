using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DOTweenDelegateSequence : DOTweenSequence
{
    private Action<Sequence> _onStartCallback;

    public DOTweenDelegateSequence(Action<Sequence> onStartCallback)
    {
        _onStartCallback = onStartCallback;
    }

    protected override void InternalStart(Sequence sequence)
    {
        _onStartCallback.Invoke(sequence);
    }
}
