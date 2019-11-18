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

        [DisplayName("IntendWithdrawEvent")]
        public static event Action<byte[], byte[], BigInteger> IntendWithdrawEvent;

        [DisplayName("ConfirmWithdrawEvent")]
        public static event Action<byte[], BigInteger, byte[], byte[], BigInteger[], BigInteger[]> ConfirmWithdrawEvent;

        [DisplayName("VetoWithdrawEvent")]
        public static event Action<byte[]> VetoWithdrawEvent;

        [DisplayName("IntendSettleEvent")]
        public static event Action<byte[], BigInteger[]> IntendSettleEvent;

        [DisplayName("ConfirmSettleFailEvent")]
        public static event Action<byte[]> ConfirmSettleFailEvent;

        [DisplayName("ConfirmSettleEvent")]
        public static event Action<byte[], BigInteger[]> ConfirmSettleEvent;

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
            else if (Runtime.Trigger == TriggerType.Application)
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
                if (operation == "intendWithdrawMockSet")
                {
                    BasicMethods.assert(args.Length == 4, "intendWithdrawMockSet parameter error");
                    byte[] _channelId = (byte[])args[0];
                    BigInteger _amount = (BigInteger)args[1];
                    byte[] _recipientChannelId = (byte[])args[2];
                    byte[] _receiver = (byte[])args[3];
                    return intendWithdrawMockSet(_channelId, _amount, _recipientChannelId, _receiver);
                }
                if (operation == "intendWithdraw")
                {
                    BasicMethods.assert(args.Length == 3, "intendWithdraw parameter error");
                    byte[] _channelId = (byte[])args[0];
                    BigInteger _amount = (BigInteger)args[1];
                    byte[] _recipientChannelId = (byte[])args[2];
                    return intendWithdraw(_channelId, _amount, _recipientChannelId);
                }
                if (operation == "confirmWithdraw")
                {
                    BasicMethods.assert(args.Length == 1, "confirmWithdraw parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return confirmWithdraw(_channelId);
                }
                if (operation == "vetoWithdraw")
                {
                    BasicMethods.assert(args.Length == 2, "vetoWithdraw parameter error");
                    byte[] _channelId = (byte[])args[0];
                    byte[] _sender = (byte[])args[1];
                    return vetoWithdraw(_channelId, _sender);
                }
                if (operation == "intendSettleMockSet")
                {
                    BasicMethods.assert(args.Length == 7, "intendSettleMockSet parameter error");
                    byte[] _channelId = (byte[])args[0];
                    byte[] _peerFrom = (byte[])args[1];
                    BigInteger _seqNum = (BigInteger)args[2];
                    BigInteger _transferOut = (BigInteger)args[3];
                    byte[] _nextPayIdListHash = (byte[])args[4]; ;
                    BigInteger _lastPayResolveDeadline = (BigInteger)args[5]; ;
                    BigInteger _pendingPayOut = (BigInteger)args[6];
                    return intendSettleMockSet(_channelId, _peerFrom, _seqNum, _transferOut, _nextPayIdListHash, _lastPayResolveDeadline, _pendingPayOut);
                }
                if (operation == "intendSettle")
                {
                    BasicMethods.assert(args.Length == 1, "intendSettle parameter error");
                    byte[] _signedSimplexStateArray = (byte[])args[0];
                    return intendSettle(_signedSimplexStateArray);
                }
                if (operation == "intendSettleRevert")
                {
                    BasicMethods.assert(args.Length == 1, "intendSettleRevert parameter error");
                    byte[] _signedSimplexStateArray = (byte[])args[0];
                    return intendSettleRevert(_signedSimplexStateArray);
                }
                if (operation == "confirmSettle")
                {
                    BasicMethods.assert(args.Length == 1, "confirmSettle parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return confirmSettle(_channelId);
                }
                if (operation == "getSettleFinalizedTime")
                {
                    BasicMethods.assert(args.Length == 1, "getSettleFinalizedTime parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getSettleFinalizedTime(_channelId);
                }
                if (operation == "getTokenContract")
                {
                    BasicMethods.assert(args.Length == 1, "getTokenContract parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getTokenContract(_channelId);
                }
                if (operation == "getTokenType")
                {
                    BasicMethods.assert(args.Length == 1, "getTokenType parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getTokenType(_channelId);
                }
                if (operation == "getChannelStatus")
                {
                    BasicMethods.assert(args.Length == 1, "getChannelStatus parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getChannelStatus(_channelId);
                }
                if (operation == "getCooperativeWithdrawSeqNum")
                {
                    BasicMethods.assert(args.Length == 1, "getCooperativeWithdrawSeqNum parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getCooperativeWithdrawSeqNum(_channelId);
                }
                if (operation == "getTotalBalance")
                {
                    BasicMethods.assert(args.Length == 1, "getTotalBalance parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getTotalBalance(_channelId);
                }
                if (operation == "getBalanceMap")
                {
                    BasicMethods.assert(args.Length == 1, "getBalanceMap parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getBalanceMap(_channelId);
                }
                if (operation == "getChannelMigrationArgs")
                {
                    BasicMethods.assert(args.Length == 1, "getChannelMigrationArgs parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getChannelMigrationArgs(_channelId);
                }
                if (operation == "getPeersMigrationInfo")
                {
                    BasicMethods.assert(args.Length == 1, "getPeersMigrationInfo parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getPeersMigrationInfo(_channelId);
                }
                if (operation == "getDisputeTimeout")
                {
                    BasicMethods.assert(args.Length == 1, "getDisputeTimeout parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getDisputeTimeout(_channelId);
                }
                if (operation == "getMigratedTo")
                {
                    BasicMethods.assert(args.Length == 1, "getMigratedTo parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getMigratedTo(_channelId);
                }
                if (operation == "getStateSeqNumMap")
                {
                    BasicMethods.assert(args.Length == 1, "getStateSeqNumMap parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getStateSeqNumMap(_channelId);
                }
                if (operation == "getTransferOutMap")
                {
                    BasicMethods.assert(args.Length == 1, "getTransferOutMap parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getTransferOutMap(_channelId);
                }
                if (operation == "getNextPayIdListHashMap")
                {
                    BasicMethods.assert(args.Length == 1, "getNextPayIdListHashMap parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getNextPayIdListHashMap(_channelId);
                }
                if (operation == "getLastPayResolveDeadlineMap")
                {
                    BasicMethods.assert(args.Length == 1, "getLastPayResolveDeadlineMap parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getLastPayResolveDeadlineMap(_channelId);
                }
                if (operation == "getPendingPayOutMap")
                {
                    BasicMethods.assert(args.Length == 1, "getPendingPayOutMap parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getPendingPayOutMap(_channelId);
                }
                if (operation == "getWithdrawIntent")
                {
                    BasicMethods.assert(args.Length == 1, "getWithdrawIntent parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return getWithdrawIntent(_channelId);
                }
                if (operation == "getChannelStatusNum")
                {
                    BasicMethods.assert(args.Length == 1, "getChannelStatusNum parameter error");
                    BigInteger _channelStatus = (BigInteger)args[0];
                    return getChannelStatusNum(_channelStatus);
                }
                if (operation == "getPayRegistry")
                {
                    BasicMethods.assert(args.Length == 0, "getPayRegistry parameter error");
                    return getPayRegistry();
                }
                if (operation == "getCelerWallet")
                {
                    BasicMethods.assert(args.Length == 0, "getCelerWallet parameter error");
                    return getCelerWallet();
                }
                if (operation == "getBalanceLimit")
                {
                    BasicMethods.assert(args.Length == 1, "getBalanceLimit parameter error");
                    byte[] _tokenAddr = (byte[])args[0];
                    return getBalanceLimit(_tokenAddr);
                }
                if (operation == "getBalanceLimitsEnabled")
                {
                    BasicMethods.assert(args.Length == 0, "getBalanceLimitsEnabled parameter error");
                    return getBalanceLimitsEnabled();
                }
            }
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

        [DisplayName("deposit")]
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
            LedgerStruct.BalanceMap balanceMap = LedgerChannel.getBalanceMapInner(c);
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
        }

        [DisplayName("intendWithdraw")]
        public static bool intendWithdraw(byte[] _channelId, BigInteger _amount, byte[] _recipientChannelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            BasicMethods.assert(_amount >= 0, "_amount smaller than zero");
            BasicMethods.assert(BasicMethods._isByte32(_recipientChannelId), "_recipientChannelId illegal");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            IntendWithdrawEvent(_channelId, c.withdrawIntent.receiver, _amount);
            return true;
        }

        [DisplayName("confirmWithdraw")]
        public static bool confirmWithdraw(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);

            LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;
            byte[] receiver = (byte[])withdrawIntent.receiver.Clone();
            BigInteger amount = withdrawIntent.amount;
            byte[] recipientChannelId = (byte[])withdrawIntent.recipientChannelId.Clone();
            withdrawIntent.receiver = null;
            withdrawIntent.amount = 0;
            withdrawIntent.requestTime = 0;
            withdrawIntent.recipientChannelId = null;
            c.withdrawIntent = withdrawIntent;

            c = LedgerChannel._addWithdrawal(c, receiver, amount);
            LedgerStruct.setChannelMap(_channelId, c);
            LedgerStruct.BalanceMap balanceMap = LedgerChannel.getBalanceMapInner(c);
            ConfirmWithdrawEvent(_channelId, amount, receiver, recipientChannelId, balanceMap.deposits, balanceMap.withdrawals);

            return true;
        }

        [DisplayName("vetoWithdraw")]
        public static bool vetoWithdraw(byte[] _channelId, byte[] _sender)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_sender), "_channelId illegal");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");
            LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;
            byte[] receiver = withdrawIntent.receiver;
            BasicMethods.assert(receiver.ToBigInteger() != 0, "No pending withdraw intent");
            BasicMethods.assert(LedgerChannel._isPeer(c, _sender), "msg.sender is not peer");

            withdrawIntent.receiver = null;
            withdrawIntent.amount = 0;
            withdrawIntent.requestTime = 0;
            withdrawIntent.recipientChannelId = null;
            c.withdrawIntent = withdrawIntent;
            LedgerStruct.setChannelMap(_channelId, c);
            VetoWithdrawEvent(_channelId);
            return true;
        }

        [DisplayName("intendSettleMockSet")]
        public static bool intendSettleMockSet(byte[] _channelId, byte[] _peerFrom, BigInteger _seqNum, BigInteger _transferOut, byte[] _nextPayIdListHash, BigInteger _lastPayResolveDeadline, BigInteger _pendingPayOut)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(_peerFrom), "_peerFrom illegal");
            BasicMethods.assert(_seqNum >= 0, "_seqNum smaller than zero");
            BasicMethods.assert(_transferOut >= 0, "_transferOut smaller than zero");
            BasicMethods.assert(BasicMethods._isByte32(_nextPayIdListHash), "_nextPayIdListHash illegal");
            BasicMethods.assert(_lastPayResolveDeadline >= 0, "_lastPayResolveDeadline smaller than zero");
            BasicMethods.assert(_pendingPayOut >= 0, "_pendingPayOut smaller than zero");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            byte peerFromId = LedgerChannel._getPeerId(c, _peerFrom);
            BasicMethods.assert(peerFromId == 0 || peerFromId == 1, "peerFromId illegal");
            LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
            LedgerStruct.PeerProfile peerProfile = peerProfiles[peerFromId];
            LedgerStruct.PeerState state = peerProfile.state;

            state.seqNum = _seqNum;
            state.transferOut = _transferOut;
            state.nextPayIdListHash = _nextPayIdListHash;
            state.lastPayResolveDeadline = _lastPayResolveDeadline;
            state.pendingPayOut = _pendingPayOut;

            c = _updateOverallStatesByIntendState(c);
            LedgerStruct.setChannelMap(_channelId, c);
            setTmpChannelId(_channelId);
            return true;
        }

        [DisplayName("intendSettle")]
        public static bool intendSettle(byte[] _signedSimplexStateArray)
        {
            byte[] _channelId = getTmpChannelId();
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");

            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            IntendSettleEvent(_channelId, LedgerChannel._getStateSeqNums(c));
            return true;
        }

        [DisplayName("intendSettleRevert")]
        public static bool intendSettleRevert(byte[] _signedSimplexStateArray)
        {
            //Pending
            return true;
        }

        [DisplayName("confirmSettle")]
        public static bool confirmSettle(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            LedgerStruct.SettleBalance balance = LedgerChannel._validateSettleBalance(c);
            if (balance.isSettled != 1)
            {
                c = _resetDuplexState(c);
                ConfirmSettleFailEvent(_channelId);
                return false;
            }
            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            c = _updateChannelStatus(c, channelStatus.Closed);
            LedgerStruct.setChannelMap(_channelId, c);
            ConfirmSettleEvent(_channelId, balance.balance);
            return true;
        }

        [DisplayName("getSettleFinalizedTime")]
        public static BigInteger getSettleFinalizedTime(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getSettleFinalizedTimeInner(c);
        }

        [DisplayName("getTokenContract")]
        public static byte[] getTokenContract(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTokenContractInner(c);
        }

        [DisplayName("getTokenType")]
        public static byte getTokenType(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTokenTypeInner(c);
        }

        [DisplayName("getChannelStatus")]
        public static byte getChannelStatus(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getChannelStatusInner(c);
        }

        [DisplayName("getCooperativeWithdrawSeqNum")]
        public static BigInteger getCooperativeWithdrawSeqNum(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getCooperativeWithdrawSeqNumInner(c);
        }

        [DisplayName("getTotalBalance")]
        public static BigInteger getTotalBalance(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTotalBalanceInner(c);
        }

        [DisplayName("getBalanceMap")]
        public static LedgerStruct.BalanceMap getBalanceMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getBalanceMapInner(c);
        }

        [DisplayName("getChannelMigrationArgs")]
        public static LedgerStruct.ChannelMigrationArgs getChannelMigrationArgs(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getChannelMigrationArgsInner(c);
        }

        [DisplayName("getPeersMigrationInfo")]
        public static LedgerStruct.PeersMigrationInfo getPeersMigrationInfo(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getPeersMigrationInfoInner(c);
        }

        [DisplayName("getDisputeTimeout")]
        public static BigInteger getDisputeTimeout(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getDisputeTimeoutInner(c);
        }

        [DisplayName("getMigratedTo")]
        public static byte[] getMigratedTo(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getMigratedToInner(c);
        }

        [DisplayName("getStateSeqNumMap")]
        public static LedgerStruct.StateSeqNumMap getStateSeqNumMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getStateSeqNumMapInner(c);
        }

        [DisplayName("getTransferOutMap")]
        public static LedgerStruct.TransferOutMap getTransferOutMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTransferOutMapInner(c);
        }

        [DisplayName("getNextPayIdListHashMap")]
        public static LedgerStruct.NextPayIdListHashMap getNextPayIdListHashMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getNextPayIdListHashMapInner(c);
        }

        [DisplayName("getLastPayResolveDeadlineMap")]
        public static LedgerStruct.LastPayResolveDeadlineMap getLastPayResolveDeadlineMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getLastPayResolveDeadlineMapInner(c);
        }

        [DisplayName("getPendingPayOutMap")]
        public static LedgerStruct.PendingPayOutMap getPendingPayOutMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getPendingPayOutMapInner(c);
        }

        [DisplayName("getWithdrawIntent")]
        public static LedgerStruct.WithdrawIntent getWithdrawIntent(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getWithdrawIntentInner(c);
        }

        [DisplayName("getChannelStatusNum")]
        public static BigInteger getChannelStatusNum(BigInteger _channelStatus)
        {
            return LedgerStruct.getChannelStatusNums(_channelStatus);
        }

        [DisplayName("getPayRegistry")]
        public static byte[] getPayRegistry()
        {
            return getLedger().payRegistry;
        }

        [DisplayName("getCelerWallet")]
        public static byte[] getCelerWallet()
        {
            return getLedger().celerWallet;
        }

        [DisplayName("getBalanceLimit")]
        public static BigInteger getBalanceLimit(byte[] _tokenAddr)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(_tokenAddr), "_tokenAddr illegal");
            return LedgerBalanceLimit.getBalanceLimitInner(_tokenAddr);
        }

        [DisplayName("getBalanceLimitsEnabled")]
        public static bool getBalanceLimitsEnabled()
        {
            return LedgerBalanceLimit.getBalanceLimitsEnabledInner();
        }

        private static LedgerStruct.Channel _updateChannelStatus(LedgerStruct.Channel _c, byte _newStatus)
        {
            if (_c.status == _newStatus)
            {
                return _c;
            }
            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            // update counter of old status
            if (_c.status != channelStatus.Uninitialized)
            {
                LedgerStruct.setChannelStatusNums(_c.status, LedgerStruct.getChannelStatusNums(_c.status) - 1);
            }

            // update counter of new status
            LedgerStruct.setChannelStatusNums(_newStatus, LedgerStruct.getChannelStatusNums(_newStatus) + 1);

            _c.status = _newStatus;

            return _c;
        }

        private static LedgerStruct.Channel _updateOverallStatesByIntendState(LedgerStruct.Channel c)
        {
            c.settleFinalizedTime = Blockchain.GetHeight() + c.disputeTimeout;
            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            return _updateChannelStatus(c, channelStatus.Settling);
        }

        private static LedgerStruct.Channel _resetDuplexState(LedgerStruct.Channel c)
        {
            c.settleFinalizedTime = 0;
            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            c = _updateChannelStatus(c, channelStatus.Operable);
            LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
            LedgerStruct.PeerProfile peerProfile0 = peerProfiles[0];
            LedgerStruct.PeerProfile peerProfile1 = peerProfiles[1];
            LedgerStruct.PeerState state = peerProfile0.state;
            state.seqNum = 0;
            state.transferOut = 0;
            state.nextPayIdListHash = null;
            state.lastPayResolveDeadline = 0;
            state.pendingPayOut = 0;
            state = peerProfile1.state;
            state.seqNum = 0;
            state.transferOut = 0;
            state.nextPayIdListHash = null;
            state.lastPayResolveDeadline = 0;
            state.pendingPayOut = 0;
            // reset possibly remaining WithdrawIntent freezed by previous intendSettle()
            LedgerStruct.WithdrawIntent intent = c.withdrawIntent;
            intent.receiver = null;
            intent.amount = 0;
            intent.requestTime = 0;
            intent.recipientChannelId = null;
            return c;
        }
    }
}
