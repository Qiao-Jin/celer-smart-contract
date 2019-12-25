using Neo.SmartContract.Framework.Services.Neo;
using System;

public class BasicMethods
{
    public delegate object DynamicCallContract(string method, object[] args);

    private static readonly byte[] addressZero = Neo.SmartContract.Framework.Helper.ToScriptHash("ASqYXoq6HXdfxu3XJr2RHiUFNHVh3fyQiz");

    private static readonly byte[] Admin = Neo.SmartContract.Framework.Helper.ToScriptHash("Ab5x97fSdMzddxnmWvDXaTJC51zkbNVp4n");

    private static readonly int legalLength = 20;

    public static void assert(bool condition, string msg)
    {
        if (!condition)
        {
            Runtime.Notify(msg);
            throw new Exception(msg + " error ");
        }
    }

    public static bool _isLegalAddress(byte[] addr)
    {
        return addr.Length == legalLength && !addr.Equals(addressZero);
    }

    public static bool _isLegalAddresses(byte[][] addrs)
    {
        for (var i = 0; i < addrs.Length; i++)
        {
            if (_isLegalAddress(addrs[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    public static bool _isLegalLength(int len)
    {
        return len == legalLength;
    }

    public static bool _isByte32(byte[] byte32)
    {
        return byte32.Length == 32;
    }

    public static bool _isByte32s(byte[][] byte32s)
    {
        for (var i = 0; i < byte32s.Length; i++)
        {
            if (_isByte32(byte32s[i]) == false)
            {
                return false;
            }
        }
        return true;
    }

    public static byte[] getAdmin()
    {
        return Admin;
    }

    public static byte[] clone(byte[] input)
    {
        if (input == null) return null;
        byte[] result = (byte[])Neo.SmartContract.Framework.Helper.Deserialize(Neo.SmartContract.Framework.Helper.Serialize(input));
        return result;
    }
}