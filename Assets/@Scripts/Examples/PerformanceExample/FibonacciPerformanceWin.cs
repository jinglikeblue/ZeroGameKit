using UnityEngine;

namespace Example
{
    /// <summary>
    /// 斐波拉契数列计算
    /// </summary>
    public class FibonacciPerformanceWin : BasePerformanceWin
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);

            inputFieldCount.text = "1000000";
            inputFieldCount.characterLimit = 10;
        }

        protected override void StartTest(long count)
        {
            long amount = 0;
            base.StartTest(count);
            amount += FibonacciSequence(count);
            base.EndTest();
            if (amount <= 0)
            {
                Debug.Log(amount);    
            }
        }

        int FibonacciSequence(long count)
        {
            int result = 0;
            int last = 1;
            int lastnext = 1;

            for (int x = 2; x < count; x++)
            {
                result = last + lastnext;
                lastnext = last;
                last = result;
            }

            return result;
        }

    }
}