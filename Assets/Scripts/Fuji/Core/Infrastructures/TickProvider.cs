using UnityEngine;
using System.Collections.Generic;

public class TickProvider : MonoBehaviour
{
    private readonly List<ITickable> _tickables = new();

    public void Register(ITickable tickable)
    {
        if (!_tickables.Contains(tickable)) _tickables.Add(tickable);
    }
    
    public void Unregister(ITickable tickable) => _tickables.Remove(tickable);

    private void Update()
    {
        for (int i = 0; i < _tickables.Count; i++)
        {
            _tickables[i].Tick(Time.deltaTime);
        }
    }
}
