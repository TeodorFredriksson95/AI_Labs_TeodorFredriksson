using System;
using UnityEngine;

public class RendezvousCoordinator : MonoBehaviour
{
    public bool HelperReady { get; private set; }
    public bool TRBReady { get; private set; }

    public bool BothReady => HelperReady && TRBReady;

    public event Action OnBothReady;

    public void SetHelperReady()
    {
        HelperReady = true;
        Check();
    }

    public void SetTRBReady()
    {
        TRBReady = true;
        Check();
    }

    private void Check()
    {
        if (BothReady)
            OnBothReady?.Invoke();
    }
}
