using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
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

    public static readonly byte[] NeoID = Neo.SmartContract.Framework.Helper.HexToBytes("c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b").Reverse();
    public static readonly byte[] GasID = Neo.SmartContract.Framework.Helper.HexToBytes("602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7").Reverse();
    public static readonly byte[] NeoAddress = new byte[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static readonly byte[] GasAddress = new byte[20] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public struct TransactionValue
    {
        public byte tokenType;
        public BigInteger value;
        public byte[] receiver;
    }

    public static TransactionValue getTransactionValue(byte tokenType)
    {
        PbEntity.TokenType token = PbEntity.getStandardTokenType();
        Transaction tx = ExecutionEngine.ScriptContainer as Transaction;
        TransactionOutput[] outputs = tx.GetOutputs();
        if (outputs.Length == 0)
        {
            return new TransactionValue()
            {
                tokenType = tokenType,
                value = 0,
                receiver = ExecutionEngine.ExecutingScriptHash
            };
        }
        BasicMethods.assert(outputs.Length == 1, "Invalid outputs length");
        byte[] assetid = outputs[0].AssetId;
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
        BasicMethods.assert(tokenType == type, "Unmatch token type");
        TransactionValue transactionValue = new TransactionValue()
        {
            tokenType = type,
            value = outputs[0].Value,
            receiver = outputs[0].ScriptHash
        };
        return transactionValue;
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
