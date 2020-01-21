using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System.Collections.Generic;
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
        ChannelStatus cs = new ChannelStatus()
        {
            Uninitialized = 0,
            Operable = 1,
            Settling = 2,
            Closed = 3,
            Migrated = 4
        };
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

    public static PeerState initPeerState()
    {
        PeerState state = new PeerState()
        {
            seqNum = 0,
            transferOut = 0,
            nextPayIdListHash = null,
            lastPayResolveDeadline = 0,
            pendingPayOut = 0
        };
        return state;
    }

    public struct PeerProfile
    {
        public byte[] peerAddr;
        public BigInteger deposit;
        public BigInteger withdrawal;
        public PeerState state;
    }

    public static PeerProfile initPeerProfile()
    {
        PeerProfile peerProfile = new PeerProfile()
        {
            peerAddr = null,
            deposit = 0,
            withdrawal = 0,
            state = initPeerState()
        };
        return peerProfile;
    }

    public struct WithdrawIntent
    {
        public byte[] receiver;
        public BigInteger amount;
        public BigInteger requestTime;
        public byte[] recipientChannelId;
    }

    public static WithdrawIntent initWithdrawIntent()
    {
        WithdrawIntent intent = new WithdrawIntent()
        {
            receiver = null,
            amount = 0,
            requestTime = 0,
            recipientChannelId = null
        };
        return intent;
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

    public static Channel initChannel()
    {
        Channel newChannel = new Channel();
        newChannel.settleFinalizedTime = 0;
        newChannel.disputeTimeout = 0;
        newChannel.token = new PbEntity.TokenInfo()
        {
            tokenType = 0,
            address = null
        };
        newChannel.status = 0;
        newChannel.migratedTo = null;
        PeerProfile[] peerProfiles = new PeerProfile[2];
        peerProfiles[0] = initPeerProfile();
        peerProfiles[1] = initPeerProfile();
        newChannel.peerProfiles = peerProfiles;
        newChannel.cooperativeWithdrawSeqNum = 0;
        newChannel.withdrawIntent = initWithdrawIntent();
        return newChannel;
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
        if (result == null) return 0;
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
        if (result != null)
        {
            return (Channel)Neo.SmartContract.Framework.Helper.Deserialize(result);
        }
        else
        {
            return initChannel();
        }
    }

    public static bool setChannelMap(byte[] key, Channel value)
    {
        Storage.Put(Storage.CurrentContext, Ledger.channelMapPrefix.Concat(key), Neo.SmartContract.Framework.Helper.Serialize(value));
        return true;
    }

    public static readonly byte[] NeoID = Neo.SmartContract.Framework.Helper.HexToBytes("9b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc5");
    public static readonly byte[] GasID = Neo.SmartContract.Framework.Helper.HexToBytes("e72d286979ee6cb1b7e65dfddfb2e384100b8d148e7758de42e4168b71792c60");
    public static readonly byte[] NeoAddress = new byte[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static readonly byte[] GasAddress = new byte[20] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public static BigInteger getTransactionValue(byte tokenType, byte[] receiver)
    {
        PbEntity.TokenType token = PbEntity.getStandardTokenType();
        Transaction tx = ExecutionEngine.ScriptContainer as Transaction;
        TransactionOutput[] outputs = tx.GetOutputs();
        BigInteger result = 0;
        foreach (TransactionOutput output in outputs)
        {
            byte[] scriptHash = output.ScriptHash;
            if (!scriptHash.Equals(receiver)) continue;
            byte[] assetid = output.AssetId;
            //Temperately only support NEO and GAS
            byte type = token.INVALID;
            if (assetid.Equals(NeoID))
            {
                type = token.NEO;
            }
            else if (assetid.Equals(GasID))
            {
                type = token.GAS;
            }
            if (tokenType != type) continue;
            result += output.Value;
        }
        return result;
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
