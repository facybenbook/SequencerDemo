using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISequenceAggregator : System.IDisposable
{
    void EnqueueSequence(ISequence sequence);
    bool CanAggregateSequence(ISequence sequence);
}
