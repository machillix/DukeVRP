using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace DUKE.Controls
{

    ///<summary><typeparamref name="UnityEvent"/> passing a <typeparamref name="float"/> parameter.</summary>
    [System.Serializable] public class FloatEvent : UnityEvent<float> { }

    ///<summary><typeparamref name="UnityEvent"/> passing a <typeparamref name="Vector3"/> parameter.</summary>
    [System.Serializable] public class Vector3Event : UnityEvent<Vector3> { }

    ///<summary><typeparamref name="UnityEvent"/> passing a <typeparamref name="Quaternion"/> parameter.</summary>
    [System.Serializable] public class QuaternionEvent : UnityEvent<Quaternion> { }



}


