using System;
using System.Collections.Generic;
using Chaos.Core.Definitions;
using Chaos.Observers.Interfaces;

namespace Chaos.Containers.Interfaces;

public interface IPanel<T> : IEnumerable<T>
{
    T? this[byte slot] { get; }
    bool IsFull { get; }
    PanelType PaneType { get; }
    void AddObserver(IPanelObserver<T> observer);
    bool Contains(T obj);
    bool IsValidSlot(byte slot);
    bool Remove(byte slot);
    bool Remove(string name);
    bool TryAdd(byte slot, T obj);
    bool TryAddToNextSlot(T obj);
    bool TryGetObject(byte slot, out T? obj);
    bool TryGetRemove(byte slot, out T? obj);
    bool TrySwap(byte slot1, byte slot2);
    void Update(byte slot, Action<T> action);
}