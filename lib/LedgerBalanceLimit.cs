using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;
using System.Numerics;

public class LedgerBalanceLimit
{
    private static readonly byte[] BalanceLimitsPrefix = "balanceLimit".AsByteArray();

    private static readonly byte[] BalanceLimitsEnabledPrefix = "balanceLimitsEnabled".AsByteArray();

    [DisplayName("setBalanceLimitsInner")]
    public static object setBalanceLimitsInner(byte[][] tokenAddrs, BigInteger[] limits)
    {
        BasicMethods.assert(tokenAddrs.Length == limits.Length, "Lengths do not match");
        for (int i = 0; i < tokenAddrs.Length; i++)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddrs[i]), "Token address " + i + " is illegal");
            Storage.Put(Storage.CurrentContext, BalanceLimitsPrefix.Concat(tokenAddrs[i]), limits[i]);
        }
        return true;
    }

    [DisplayName("disableBalanceLimitsInner")]
    public static object disableBalanceLimitsInner()
    {
        Storage.Put(Storage.CurrentContext, BalanceLimitsEnabledPrefix, 1);
        return true;
    }

    [DisplayName("enableBalanceLimitsInner")]
    public static object enableBalanceLimitsInner()
    {
        Storage.Put(Storage.CurrentContext, BalanceLimitsEnabledPrefix, 0);
        return true;
    }

    [DisplayName("getBalanceLimitInner")]
    public static BigInteger getBalanceLimitInner(byte[] tokenAddr)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddr), "Token address is illegal");
        return Storage.Get(Storage.CurrentContext, BalanceLimitsPrefix.Concat(tokenAddr)).ToBigInteger();
    }

    [DisplayName("getBalanceLimitsEnabledInner")]
    public static bool getBalanceLimitsEnabledInner()
    {
        byte[] result = Storage.Get(Storage.CurrentContext, BalanceLimitsEnabledPrefix);
        if (result == null) return false;
        else return result.ToBigInteger() == 1;
    }
}
