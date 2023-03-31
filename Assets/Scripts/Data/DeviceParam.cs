using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DeviceParamType {
    ENone = 0,
    EFloat
};
public struct DeviceParam {
    public DeviceParamType paramType;
    public string name;
    public object min;
    public object max;
    public object value;
    public object target;
}