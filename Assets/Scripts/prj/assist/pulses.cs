using System.Collections.Generic;
using UnityEngine;

public class pulses : MonoBehaviour
{
    static pulses _inst;
    Queue<pulse> _pulses = new Queue<pulse>();
    void Awake() { _inst = this; Enq(pulseType.TrapI); }
    public static void Clear() { _inst._pulses.Clear(); Enq(pulseType.TrapI); }
    public static void Enq(pulseType type) { _inst._pulses.Enqueue(new pulse(type)); }

    public static void Init()
    {
        pulses.Enq(pulseType.Straight);
        pulses.Enq(pulseType.SafeBeginZ);
        pulses.Enq(pulseType.Ballers);
    }

    void Update()//Fixed
    {
        pulse msg = _pulses.Dequeue();
        switch (msg.type) {
            case pulseType.Straight: cells.StraightOnPulse(); break;
            case pulseType.SafeBeginZ: area.SafeBeginZOnPulse(); break;
            case pulseType.Ballers: ballers.SpawnOnPulse(); break;
            case pulseType.TrapI: break;

            case pulseType.End:
            flows.Change<flowPlay>();
            break;
        }
        _pulses.Enqueue(msg);
    }
    
    struct pulse { public pulseType type; public pulse(pulseType type_) { type = type_; } }
}

public enum pulseType : byte
{
    Straight = 0, SafeBeginZ, Ballers, TrapI, 
    
    DmgMat, Shield,
    
    Tnt,

    End,
}