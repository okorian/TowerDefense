using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubscriber<T>
{
    void OnSignalReceived(T signal);
}
