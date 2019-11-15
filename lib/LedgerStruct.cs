using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System.Numerics;

public class LedgerStruct
{
    public struct ChannelStatus
    {
        public byte Uninitialized;
        public byte Operable;
        public byte Settling;
        public byte Closed;
        public byte Migrated;
    }

    public static ChannelStatus getStandardChannelStatus()
    {
        ChannelStatus cs = new ChannelStatus();
        cs.Uninitialized = 0;
        cs.Operable = 1;
        cs.Settling = 2;
        cs.Closed = 3;
        cs.Migrated = 4;
        return cs;
    }

    public struct PeerState
    {
        public BigInteger seqNum;
        public BigInteger transferOut;
        public byte[] nextPayIdListHash;
        public BigInteger lastPayResolveDeadline;
        public BigInteger pendingPayOut;
    }

    public struct PeerProfile
    {
        public byte[] peerAddr;
        public BigInteger deposit;
        public BigInteger withdrawal;
        public PeerState state;
    }

    public struct WithdrawIntent
    {
        public byte[] receiver;
        public BigInteger amount;
        public BigInteger requestTime;
        public byte[] recipientChannelId;
    }

    public struct Channel
    {
        public BigInteger settleFinalizedTime;
        public BigInteger disputeTimeout;
        public PbEntity.TokenInfo token;
        public byte status;
        public byte[] migratedTo;
        public PeerProfile[] peerProfiles;
        public BigInteger cooperativeWithdrawSeqNum;
        public WithdrawIntent withdrawIntent;
    }

    public struct Ledger
    {
        //public Map<BigInteger, BigInteger> channelStatusNums;
        public static readonly byte[] channelStatusNumsPrefix = "channelStatusNums".AsByteArray();
        //public byte[] ethPool;
        public byte[] payRegistry;
        public byte[] celerWallet;
        //public Map<byte[], BigInteger> balanceLimits;
        //public static readonly byte[] balanceLimitsPrefix = "balanceLimits".AsByteArray();
        //public Map<byte[], Channel> channelMap;
        public static readonly byte[] channelMapPrefix = "channelMap".AsByteArray();
    }

    public static BigInteger getChannelStatusNums (BigInteger key)
    {
        byte[] result = Storage.Get(Storage.CurrentContext, Ledger.channelStatusNumsPrefix.Concat(key.ToByteArray()));
        if (result == null) return -1;
        return result.ToBigInteger();
    }

    public static bool setChannelStatusNums(BigInteger key, BigInteger value)
    {
        Storage.Put(Storage.CurrentContext, Ledger.channelStatusNumsPrefix.Concat(key.ToByteArray()), value);
        return true;
    }

    /*public static BigInteger getBalanceLimits(byte[] key)
    {
        byte[] result = Storage.Get(Storage.CurrentContext, Ledger.balanceLimitsPrefix.Concat(key));
        if (result == null) return -1;
        return result.ToBigInteger();
    }

    public static bool setBalanceLimits(byte[] key, BigInteger value)
    {
        Storage.Put(Storage.CurrentContext, Ledger.balanceLimitsPrefix.Concat(key), value);
        return true;
    }*/

    public static Channel getChannelMap(byte[] key)
    {
        byte[] result = Storage.Get(Storage.CurrentContext, Ledger.channelMapPrefix.Concat(key));
        return (Channel)Neo.SmartContract.Framework.Helper.Deserialize(result);
    }

    public static bool setChannelMap(byte[] key, Channel value)
    {
        Storage.Put(Storage.CurrentContext, Ledger.channelMapPrefix.Concat(key), Neo.SmartContract.Framework.Helper.Serialize(value));
        return true;
    }

    public struct BalanceMap
    {
        public byte[][] peerAddrs;
        public BigInteger[] deposits;
        public BigInteger[] withdrawals;
    }

    public struct ChannelMigrationArgs
    {
        public BigInteger disputeTimeout;
        public byte tokenType;
        public byte[] tokenAddress;
        public BigInteger cooperativeWithdrawSeqNum;
    }

    public struct PeersMigrationInfo
    {
        public byte[][] peerAddr;
        public BigInteger[] deposit;
        public BigInteger[] withdrawal;
        public BigInteger[] seqNum;
        public BigInteger[] transferOut;
        public BigInteger[] pendingPayout;
    }

    public struct StateSeqNumMap
    {
        public byte[][] peerAddr;
        public BigInteger[] seqNum;
    }

    public struct TransferOutMap
    {
        public byte[][] peerAddr;
        public BigInteger[] transferOut;
    }

    public struct NextPayIdListHashMap
    {
        public byte[][] peerAddr;
        public byte[][] nextPayIdListHash;
    }

    public struct LastPayResolveDeadlineMap
    {
        public byte[][] peerAddr;
        public BigInteger[] lastPayResolveDeadline;
    }

    public struct PendingPayOutMap
    {
        public byte[][] peerAddr;
        public BigInteger[] pendingPayOut;
    }

    public struct SettleBalance
    {
        public byte isSettled;
        public BigInteger[] balance;
    }

    public struct ChannelInfo
    {
        public byte[] channelId;
        public Channel channel;
    }
}
