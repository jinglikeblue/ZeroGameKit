using Jing.FixedPointNumber;
using UnityEngine;

public class FixedTrigonometricFunctionTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log($"Pi: {Math.PI} = {Mathf.PI}");
        //Debug.Log($"TwoPi: {Math.TwoPI} = {Mathf.PI * 2}");
        //Debug.Log($"HalfPi: {Math.HalfPI} = {Mathf.PI / 2}");

        //Debug.Log($"Deg2Rad: {Math.Deg2Rad} = {Mathf.Deg2Rad}");
        //Debug.Log($"Rad2Deg: {Math.Rad2Deg} = {Mathf.Rad2Deg}");

        //var a = Number.CreateFromDouble(1.2745678D);
        //Debug.Log(a);        
        //Debug.Log(Math.Round(a));
        //Debug.Log(Math.Floor(a));
        //Debug.Log(Math.Ceil(a));
        //Debug.Log(Math.Round(a,4));
        //Debug.Log(Math.Round(a, 3));
        //Debug.Log(Math.Round(a, 2));
        //Debug.Log(Math.Round(a, 1));

        //Math.Tan((Number)1);
        Debug.Log($"Sin60: {Math.Sin(60 * Math.Deg2Rad)} = {Mathf.Sin(60 * Mathf.Deg2Rad)}");
        Debug.Log($"Cos60: {Math.Cos(60 * Math.Deg2Rad)} = {Mathf.Cos(60 * Mathf.Deg2Rad)}");
        Debug.Log($"Tan45: {Math.Tan(45 * Math.Deg2Rad)} = {Mathf.Tan(45 * Mathf.Deg2Rad)}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
