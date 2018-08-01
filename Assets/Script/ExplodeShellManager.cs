using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeShellManager : MonoBehaviour
{
    public static ExplodeShellManager instance;

    [SerializeField]
    ExplodeShell original;

    private void Awake()
    {
        instance = this;
    }

    public ExplodeShell GetOne()
    {
        var obj = Instantiate(original);
        obj.gameObject.SetActive(true);
        return obj;
    }
}
