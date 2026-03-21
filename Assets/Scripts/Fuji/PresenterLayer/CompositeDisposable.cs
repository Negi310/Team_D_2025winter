using System;
using System.Collections.Generic;

public class CompositeDisposable : IDisposable
{
    private readonly List<IDisposable> _disposables = new List<IDisposable>();

    public void Add(IDisposable disposable)
    {
        if (disposable != null) _disposables.Add(disposable);
    }

    public void Dispose()
    {
        foreach (var d in _disposables)
        {
            d.Dispose();
        }
        _disposables.Clear();
    }
}