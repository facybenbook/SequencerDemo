using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISequence
{
    void Start();
    bool IsComplete { get; }
    void Complete();
}
