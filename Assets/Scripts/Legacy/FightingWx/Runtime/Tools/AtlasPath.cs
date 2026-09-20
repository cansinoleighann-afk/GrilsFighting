using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AtlasPath
{
    private static AtlasPath _self;
    public static AtlasPath self { get { if (_self == null) { _self = new AtlasPath(); } return _self; } }
    string sign = "AtlasPath";
    public AtlasPath()
    {
    }
    public string Com = "Atlas/Com";
}