using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour, ISequenceAggregator
{
    public bool IsPlayingSequence => _currentSequence != null || _sequenceQueue.Count > 0;

    private ISequence _currentSequence;
    private Queue<ISequence> _sequenceQueue;
    private Stack<ISequenceAggregator> _sequenceAggregators;

    private void Awake()
    {
        _sequenceQueue = new Queue<ISequence>();
        _sequenceAggregators = new Stack<ISequenceAggregator>();
    }

    private void Update()
    {
        Debug.Assert(_sequenceAggregators.Count == 0);

        if (_currentSequence != null && _currentSequence.IsComplete)
            _currentSequence = null;

        if (_currentSequence == null && _sequenceQueue.Count > 0)
        {
            _currentSequence = _sequenceQueue.Dequeue();
            _currentSequence.Start();
        }
    }

    public void Complete()
    {
        if (_currentSequence != null)
            _currentSequence.Complete();

        while (_sequenceQueue.Count > 0)
        {
            var sequence = _sequenceQueue.Dequeue();
            sequence.Start();
            sequence.Complete();
        }

        _currentSequence = null;
        _sequenceQueue.Clear();
    }

    public void EnqueueSequence(ISequence sequence)
    {
        ISequenceAggregator aggregator = null;

        if (_sequenceAggregators.Count > 0)
            aggregator = _sequenceAggregators.Peek();

        if (aggregator == null || !aggregator.CanAggregateSequence(sequence))
            aggregator = this;

        aggregator.EnqueueSequence(sequence);
    }

    bool ISequenceAggregator.CanAggregateSequence(ISequence sequence)
    {
        return true;
    }

    void ISequenceAggregator.EnqueueSequence(ISequence sequence)
    {
        _sequenceQueue.Enqueue(sequence);
    }

    void IDisposable.Dispose()
    {
        // no-op
    }

    public IDisposable Aggregate<TSequence>() where TSequence : ISequence
    {
        var aggregator = new ParralelSequence<TSequence>(this);
        _sequenceAggregators.Push(aggregator);
        return aggregator;
    }

    private class ParralelSequence<TSequence> : ISequence, ISequenceAggregator where TSequence : ISequence
    {
        private Sequencer _sequencer;
        private List<TSequence> _sequences;

        public ParralelSequence(Sequencer sequencer)
        {
            _sequencer = sequencer;
            _sequences = new List<TSequence>();
            _sequencer.EnqueueSequence(this);
        }

        bool ISequence.IsComplete
        {
            get
            {
                _sequences.RemoveAll(x => x.IsComplete);
                return _sequences.Count == 0;
            }

        }

        void ISequence.Start()
        {
            foreach (var sequence in _sequences)
                sequence.Start();
        }

        void ISequence.Complete()
        {
            foreach (var sequence in _sequences)
                sequence.Complete();
        }

        bool ISequenceAggregator.CanAggregateSequence(ISequence sequence)
        {
            return sequence is TSequence;
        }

        void IDisposable.Dispose()
        {
            var aggregator = _sequencer._sequenceAggregators.Pop();
            System.Diagnostics.Debug.Assert(aggregator == this);
        }

        void ISequenceAggregator.EnqueueSequence(ISequence sequence)
        {
            _sequences.Add((TSequence)sequence);
        }
    }
}
