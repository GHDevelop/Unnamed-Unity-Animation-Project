using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Enum.GetNames requires using System
public class EnumCounter
{
    public static int GetEnumLength(System.Type theEnum)
    {
        return Enum.GetNames(theEnum).Length;
    }

    public static Array GetEnumValues(System.Type theEnum)
    {
        return Enum.GetValues(theEnum);
    }
}
