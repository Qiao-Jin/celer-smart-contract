using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;

namespace CelerLedgerMock
{
    public class CelerLedgerMock : SmartContract
    {
        private static readonly byte[] ledgerHashKey = "ledgerhash".AsByteArray();

        private static readonly byte[] tmpChannelIdHashKey = "tmpChannelId".AsByteArray();

        private static readonly byte[] tmpChannelIdSetHashKey = "tmpChannelIdSet".AsByteArray();

        [DisplayName("OpenChannelEvent")]
        public static event Action<byte[], byte, byte[], byte[][], BigInteger[]> OpenChannelEvent;

        [DisplayName("DepositEvent")]
        public static event Action<byte[], byte[][], BigInteger[], BigInteger[]> DepositEvent;

        [DisplayName("SnapshotStatesEvent")]
        public static event Action<byte[], BigInteger[]> SnapshotStatesEvent;

        private static LedgerStruct.Ledger getLedger()
        {
            byte[] result = Storage.Get(Storage.CurrentContext, ledgerHashKey);
            return (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize(result);
        }

        private static void setLedger(LedgerStruct.Ledger ledger)
        {
            Storage.Put(Storage.CurrentContext, ledgerHashKey, Neo.SmartContract.Framework.Helper.Serialize(ledger));
        }

        private static byte[] getTmpChannelId()
        {
            return Storage.Get(Storage.CurrentContext, tmpChannelIdHashKey);
        }

        private static void setTmpChannelId(byte[] value)
        {
            Storage.Put(Storage.CurrentContext, tmpChannelIdHashKey, value);
        }

        private static byte[][] getTmpChannelIdSet()
        {
            return (byte[][])Neo.SmartContract.Framework.Helper.Deserialize(Storage.Get(Storage.CurrentContext, tmpChannelIdSetHashKey));
        }

        private static void setTmpChannelIdSet(byte[][] value)
        {
            Storage.Put(Storage.CurrentContext, tmpChannelIdSetHashKey, Neo.SmartContract.Framework.Helper.Serialize(value));
        }

        public static object Main(string operation, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                return true;
            }
            /*else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "init")
                {
                    BasicMethods.assert(args.Length == 2, "init parameter error");
                    byte[] _payRegistryHash = (byte[])args[0];
                    byte[] _celerWalletHash = (byte[])args[1];
                    return init(_payRegistryHash, _celerWalletHash);
                }
                if (operation == "openChannelMockSet")
                {
                    BasicMethods.assert(args.Length == 6, "init parameter error");
                    byte[] _channelId = (byte[])args[0];
                    BigInteger _disputeTimeout = (BigInteger)args[1];
                    byte[] _tokenAddress = (byte[])args[2];
                    byte _tokenType = (byte)args[3];
                    byte[][] _peerAddrs = (byte[][])args[4];
                    BigInteger[] _deposits = (BigInteger[])args[5];
                    return openChannelMockSet(_channelId, _disputeTimeout, _tokenAddress, _tokenType, _peerAddrs, _deposits);
                }
                if (operation == "openChannel")
                {
                    BasicMethods.assert(args.Length == 1, "openChannel parameter error");
                    byte[] _openRequest = (byte[])args[0];
                    return openChannel(_openRequest);
                }
                if (operation == "deposit")
                {
                    BasicMethods.assert(args.Length == 4, "deposit parameter error");
                    byte[] _channelId = (byte[])args[0];
                    byte[] _receiver = (byte[])args[1];
                    BigInteger _transferFromAmount = (BigInteger)args[2];
                    BigInteger _value = (BigInteger)args[3];
                    return deposit(_channelId, _receiver, _transferFromAmount, _value);
                }
                if (operation == "snapshotStatesMockSet")
                {
                    BasicMethods.assert(args.Length == 5, "snapshotStatesMockSet parameter error");
                    byte[][] _channelIds = (byte[][])args[0];
                    byte[][] _peerFroms = (byte[][])args[1];
                    BigInteger[] _seqNums = (BigInteger[])args[2];
                    BigInteger[] _transferOuts = (BigInteger[])args[3];
                    BigInteger[] _pendingPayOuts = (BigInteger[])args[3];
                    return snapshotStatesMockSet(_channelIds, _peerFroms, _seqNums, _transferOuts, _pendingPayOuts);
                }
                if (operation == "snapshotStates")
                {
                    BasicMethods.assert(args.Length == 1, "snapshotStates parameter error");
                    byte[] _signedSimplexStateArray = (byte[])args[0];
                    return snapshotStates(_signedSimplexStateArray);
                }

            }*/
            return false;
        }

        [DisplayName("init")]
        public static bool init(byte[] _payRegistryHash, byte[] _celerWalletHash)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(_payRegistryHash), "Pay registry contract hash illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_celerWalletHash), "celer wallet contract hash illegal");

            LedgerStruct.Ledger ledger = new LedgerStruct.Ledger();
            ledger.payRegistry = _payRegistryHash;
            ledger.celerWallet = _celerWalletHash;
            setLedger(ledger);
            LedgerBalanceLimit.enableBalanceLimitsInner();
            return true;
        }

        [DisplayName("openChannelMockSet")]
        public static bool openChannelMockSet(byte[] _channelId, BigInteger _disputeTimeout, byte[] _tokenAddress, byte _tokenType, byte[][] _peerAddrs, BigInteger[] _deposits)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_tokenAddress), "_channelId illegal");
            BasicMethods.assert(_tokenType >= 0 && _tokenType <= 3, "_tokenType illegal");
            BasicMethods.assert(_peerAddrs.Length == 2, "_peerAddrs length illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_peerAddrs[0]), "_peerAddrs 0 illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_peerAddrs[1]), "_peerAddrs 1 illegal");
            BasicMethods.assert(_deposits.Length == 2, "_deposits length illegal");
            BasicMethods.assert(_deposits[0] >= 0, "_deposits 0 illegal");
            BasicMethods.assert(_deposits[1] >= 0, "_deposits 1 illegal");

            setTmpChannelId(_channelId);

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            c.disputeTimeout = _disputeTimeout;
            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            c = LedgerOperation._updateChannelStatus(getLedger(), c, channelStatus.Operable);
            PbEntity.TokenInfo token = c.token;
            token.address = _tokenAddress;
            token.tokenType = _tokenType;
            LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
            LedgerStruct.PeerProfile peerProfile0 = peerProfiles[0];
            LedgerStruct.PeerProfile peerProfile1 = peerProfiles[1];
            peerProfile0.peerAddr = _peerAddrs[0];
            peerProfile0.deposit = _deposits[0];
            peerProfile1.peerAddr = _peerAddrs[1];
            peerProfile1.deposit = _deposits[1];
            LedgerStruct.setChannelMap(_channelId, c);
            return true;
        }

        [DisplayName("openChannel")]
        public static bool openChannel(byte[] _openRequest)
        {
            byte[] tmpChannelId = getTmpChannelId();
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(tmpChannelId);
            LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
            LedgerStruct.PeerProfile peerProfile0 = peerProfiles[0];
            LedgerStruct.PeerProfile peerProfile1 = peerProfiles[1];
            byte[][] peerAddrs = {
                peerProfile0.peerAddr,
                peerProfile1.peerAddr
            };
            BigInteger[] amounts = {
                peerProfile0.deposit,
                peerProfile1.deposit
            };

            PbEntity.TokenInfo token = c.token;
            OpenChannelEvent(
                tmpChannelId,
                token.tokenType,
                token.address,
                peerAddrs,
                amounts
            );
            return true;
        }

        /*[DisplayName("deposit")]
        public static bool deposit(byte[] _channelId, byte[] _receiver, BigInteger _transferFromAmount, BigInteger _value)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_receiver), "_receiver illegal");
            BasicMethods.assert(_transferFromAmount >= 0, "_transferFromAmount illegal");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            byte rid = LedgerChannel._getPeerId(c, _receiver);
            BasicMethods.assert(rid == 0 || rid == 1, "rid illegal");
            BigInteger amount = _transferFromAmount + _value;
            LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
            LedgerStruct.PeerProfile peerProfile = peerProfiles[rid];
            peerProfile.deposit = peerProfile.deposit+ amount;
            LedgerStruct.BalanceMap balanceMap = LedgerChannel.getBalanceMap(c);
            DepositEvent(_channelId, balanceMap.peerAddrs, balanceMap.deposits, balanceMap.withdrawals);
            return true;
        }

        [DisplayName("snapshotStatesMockSet")]
        public static bool snapshotStatesMockSet(byte[][] _channelIds, byte[][] _peerFroms, BigInteger[] _seqNums, BigInteger[] _transferOuts, BigInteger[] _pendingPayOuts)
        {
            BasicMethods.assert(_channelIds.Length == _peerFroms.Length
                && _peerFroms.Length == _seqNums.Length
                && _seqNums.Length == _transferOuts.Length
                && _transferOuts.Length == _pendingPayOuts.Length,
                "Parameter length not the same");
            for (int i = 0; i < _channelIds.Length; i++)
            {
                BasicMethods.assert(BasicMethods._isByte32(_channelIds[i]), "_channelIds " + i + " illegal");
                BasicMethods.assert(BasicMethods._isLegalAddress(_peerFroms[i]), "_peerFroms " + i + " illegal");
                BasicMethods.assert(_seqNums[i] >= 0, "_seqNums " + i + " illegal");
                BasicMethods.assert(_transferOuts[i] >= 0, "_transferOuts " + i + " illegal");
                BasicMethods.assert(_pendingPayOuts[i] >= 0, "_pendingPayOuts " + i + " illegal");

                LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelIds[i]);
                byte peerFromId = LedgerChannel._getPeerId(c, _peerFroms[i]);
                LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
                LedgerStruct.PeerProfile peerProfile = peerProfiles[peerFromId];
                LedgerStruct.PeerState state = peerProfile.state;

                state.seqNum = _seqNums[i];
                state.transferOut = _transferOuts[i];
                state.pendingPayOut = _pendingPayOuts[i];
                LedgerStruct.setChannelMap(_channelIds[i], c);
            }
            setTmpChannelIdSet(_channelIds);
            return true;
        }

        [DisplayName("snapshotStates")]
        public static bool snapshotStates(byte[] _signedSimplexStateArray)
        {
            byte[][] tmpChannelIds = getTmpChannelIdSet();
            for (int i = 0; i < tmpChannelIds.Length; i++)
            {
                BasicMethods.assert(BasicMethods._isByte32(tmpChannelIds[i]), "tmpChannelIds " + i + " illegal");
                LedgerStruct.Channel c = LedgerStruct.getChannelMap(tmpChannelIds[i]);
                SnapshotStatesEvent(tmpChannelIds[i], LedgerChannel._getStateSeqNums(c));
            }
            return true;
        }

        [DisplayName("intendWithdrawMockSet")]
        public static bool intendWithdrawMockSet(byte[] _channelId, BigInteger _amount, byte[] _recipientChannelId, byte[] _receiver)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            BasicMethods.assert(_amount >= 0, "_amount smaller than zero");
            BasicMethods.assert(BasicMethods._isByte32(_recipientChannelId), "_recipientChannelId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_receiver), "_receiver illegal");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;

            withdrawIntent.receiver = _receiver;
            withdrawIntent.amount = _amount;
            withdrawIntent.requestTime = Blockchain.GetHeight();
            withdrawIntent.recipientChannelId = _recipientChannelId;

            setTmpChannelId(_channelId);
            LedgerStruct.setChannelMap(_channelId, c);
            return true;
        }*/
    }
}
