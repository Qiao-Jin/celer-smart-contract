using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;

namespace RouterRegistry
{
    public class RouterRegistry : SmartContract
    {
        public struct RouterOperation
        {
            public byte Add;
            public byte Remove;
            public byte Refresh;
        }

        public static RouterOperation getRouterOperation()
        {
            RouterOperation ro = new RouterOperation();
            ro.Add = 0;
            ro.Remove = 1;
            ro.Refresh = 2;
            return ro;
        }

        private static readonly byte[] routerInfoPrefix = "routerInfo".AsByteArray();

        [DisplayName("update")]
        public static event Action<byte, byte[]> RouterUpdated;

        static Object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return true;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "registerRouter")
                {
                    BasicMethods.assert(args.Length == 1, "registerRouter parameter error");
                    byte[] invoker = (byte[])args[0];
                    return registerRouter(invoker);
                }
                if (operation == "deregisterRouter")
                {
                    BasicMethods.assert(args.Length == 1, "deregisterRouter parameter error");
                    byte[] invoker = (byte[])args[0];
                    return deregisterRouter(invoker);
                }
                if (operation == "refreshRouter")
                {
                    BasicMethods.assert(args.Length == 1, "refreshRouter parameter error");
                    byte[] invoker = (byte[])args[0];
                    return refreshRouter(invoker);
                }
            }
            return false;
        }

        [DisplayName("registerRouter")]
        public static bool registerRouter(byte[] invoker)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(invoker), "invoker is illegal");
            //BasicMethods.assert(Runtime.CheckWitness(invoker), "Checkwitness failed");
            //Pending check invoker

            BasicMethods.assert(Storage.Get(Storage.CurrentContext, routerInfoPrefix.Concat(invoker)).AsBigInteger() == 0, "Router address already exists");
            Storage.Put(Storage.CurrentContext, routerInfoPrefix.Concat(invoker), Blockchain.GetHeight());

            RouterOperation operation = getRouterOperation();
            RouterUpdated(operation.Add, invoker);
            return true;
        }

        [DisplayName("deregisterRouter")]
        public static bool deregisterRouter(byte[] invoker)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(invoker), "invoker is illegal");
            //BasicMethods.assert(Runtime.CheckWitness(invoker), "Checkwitness failed");
            //Pending check invoker
            BasicMethods.assert(Storage.Get(Storage.CurrentContext, routerInfoPrefix.Concat(invoker)).AsBigInteger() != 0, "Router address does not exist");
            Storage.Delete(Storage.CurrentContext, routerInfoPrefix.Concat(invoker));

            RouterOperation operation = getRouterOperation();
            RouterUpdated(operation.Remove, invoker);
            return true;
        }

        [DisplayName("refreshRouter")]
        public static bool refreshRouter(byte[] invoker)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(invoker), "invoker is illegal");
            //BasicMethods.assert(Runtime.CheckWitness(invoker), "Checkwitness failed");
            //Pending check invoker

            BasicMethods.assert(Storage.Get(Storage.CurrentContext, routerInfoPrefix.Concat(invoker)).AsBigInteger() != 0, "Router address does not exist");
            Storage.Put(Storage.CurrentContext, routerInfoPrefix.Concat(invoker), Blockchain.GetHeight());

            RouterOperation operation = getRouterOperation();
            RouterUpdated(operation.Refresh, invoker);
            return true;
        }
    }
}
