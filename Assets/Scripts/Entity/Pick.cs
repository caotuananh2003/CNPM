using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Pick
{
    public int Index { get; set; }

    public int Ammo { get; set; }
    public bool State { get; set; }
    public Pick(){}
    public Pick(int _index,int _ammo,bool _state)
    {
        this.Index = _index;
        this.State = _state;
        this.Ammo = _ammo;
    }
}

