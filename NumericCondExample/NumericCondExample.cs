using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace NumericCondExample
{
    public class NumericCondExample : SmartContract
    {
        public static Object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return true;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "isFinalized")
                {
                    return true;
                }
                else if (operation == "getOutcome")
                {
                    return 10;
                }
            }
            return false;
        }
    }
}
