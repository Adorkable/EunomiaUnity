using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eunomia;
using EunomiaUnity;
using UnityEngine;

public abstract class Sequencer<T> : AutoProcessor
{
    [SerializeField]
    private T[] items;
    [SerializeField]
    private bool randomizeOrder;
    private List<int> remainingRandomIndices;

    [SerializeField]
    private bool loop = false;

    private int currentIndex = -1;

    private bool _hasStarted = false;
    public override bool HasStarted => _hasStarted;
    private bool _hasFinished = false;
    public override bool HasFinished => _hasFinished;

    public class IndexReachedEnd : Exception { }

    abstract protected void StartCurrentItem();

    protected T GetCurrentItem()
    {
        return items[currentIndex];
    }

    protected virtual void Reset()
    {
        _hasStarted = false;
        _hasFinished = false;
    }

    // TODO: move to AutoProcessor
    public void ManuallyStart()
    {
        AutoProcess();
    }

    override protected void AutoProcess()
    {
        remainingRandomIndices = CreateRandomIndices(items);

        currentIndex = nextIndex;

        StartCurrentItem();
        _hasStarted = true;
        _hasFinished = false;
    }

    private static List<int> CreateRandomIndices(IEnumerable<T> forItems)
    {
        var result = forItems.Select((item, index) =>
        {
            return index;
        }).ToList();
        result.Randomize();
        return result;
    }

    protected void HandleCurrentItemFinished()
    {
        try
        {
            currentIndex = nextIndex;
            StartCurrentItem();
        }
        catch (IndexReachedEnd)
        {
            _hasFinished = true;
        }
    }

    private int nextIndex
    {
        get
        {
            if (randomizeOrder)
            {
                if (remainingRandomIndices.Count == 0)
                {
                    if (loop)
                    {
                        remainingRandomIndices = CreateRandomIndices(items);
                    }
                    else
                    {
                        throw new IndexReachedEnd();
                    }
                }
                var result = remainingRandomIndices.First();
                remainingRandomIndices.RemoveAt(0);
                return result;
            }
            else
            {
                var result = currentIndex + 1;
                if (result >= items.Length)
                {
                    if (loop)
                    {
                        result = 0;
                        remainingRandomIndices = CreateRandomIndices(items);
                    }
                    else
                    {
                        throw new IndexReachedEnd();
                    }
                }
                else
                {
                    remainingRandomIndices.Remove(result);
                }
                return result;
            }
        }
    }
}
