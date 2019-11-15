using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.ComponentModel;
using System.Numerics;

public class LedgerBalanceLimit
{
    private static readonly byte[] BalanceLimitsPrefix = "balanceLimit".AsByteArray();

    private static readonly byte[] BalanceLimitsEnabledPrefix = "balanceLimitsEnabled".AsByteArray();

    [DisplayName("setBalanceLimits")]
    public static object setBalanceLimits(byte[][] tokenAddrs, BigInteger[] limits)
    {
        BasicMethods.assert(tokenAddrs.Length == limits.Length, "Lengths do not match");
        for (int i = 0; i < tokenAddrs.Length; i++)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddrs[i]), "Token address " + i + " is illegal");
            Storage.Put(Storage.CurrentContext, BalanceLimitsPrefix.Concat(tokenAddrs[i]), limits[i]);
        }
        return true;
    }

    [DisplayName("disableBalanceLimits")]
    public static object disableBalanceLimits()
    {
        Storage.Put(Storage.CurrentContext, BalanceLimitsEnabledPrefix, 1);
        return true;
    }

    [DisplayName("enableBalanceLimits")]
    public static object enableBalanceLimits()
    {
        Storage.Put(Storage.CurrentContext, BalanceLimitsEnabledPrefix, 0);
        return true;
    }

    [DisplayName("getBalanceLimit")]
    public static BigInteger getBalanceLimit(byte[] tokenAddr)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddr), "Token address is illegal");
        return Storage.Get(Storage.CurrentContext, BalanceLimitsPrefix.Concat(tokenAddr)).ToBigInteger();
    }

    [DisplayName("getBalanceLimitsEnabled")]
    public static bool getBalanceLimitsEnabled()
    {
        byte[] result = Storage.Get(Storage.CurrentContext, BalanceLimitsEnabledPrefix);
        if (result == null) return false;
        else return result.ToBigInteger() == 1;
    }
}
