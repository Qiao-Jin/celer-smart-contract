using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;

namespace CelerLedger
{
    public class CelerLedger : SmartContract
    {
        private static readonly byte[] ledgerHashKey = "ledgerhash".AsByteArray();

        private static LedgerStruct.Ledger getLedger()
        {
            byte[] result = Storage.Get(Storage.CurrentContext, ledgerHashKey);
            return (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize(result);
        }

        private static void setLedger(LedgerStruct.Ledger ledger)
        {
            Storage.Put(Storage.CurrentContext, ledgerHashKey, Neo.SmartContract.Framework.Helper.Serialize(ledger));
        }

        public static Object Main(string operation, params object[] args)
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
                if (operation == "setBalanceLimits")
                {
                    BasicMethods.assert(args.Length == 2, "setBalanceLimits parameter error");
                    byte[][] _tokenAddrs = (byte[][])args[0];
                    BigInteger[] _celerWalletHash = (BigInteger[])args[1];
                    return setBalanceLimits(_tokenAddrs, _celerWalletHash);
                }
                if (operation == "disableBalanceLimits")
                {
                    BasicMethods.assert(args.Length == 0, "disableBalanceLimits parameter error");
                    return disableBalanceLimits();
                }
                if (operation == "enableBalanceLimits")
                {
                    BasicMethods.assert(args.Length == 0, "enableBalanceLimits parameter error");
                    return enableBalanceLimits();
                }
                if (operation == "openChannel")
                {
                    BasicMethods.assert(args.Length == 2, "openChannel parameter error");
                    byte[] _openRequest = (byte[])args[0];
                    BigInteger _value = (BigInteger)args[1];
                    return openChannel(_openRequest, _value);
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
                if (operation == "depositInBatch")
                {
                    BasicMethods.assert(args.Length == 4, "depositInBatch parameter error");
                    byte[][] _channelIds = (byte[][])args[0];
                    byte[][] _receivers = (byte[][])args[1];
                    BigInteger[] _transferFromAmounts = (BigInteger[])args[2];
                    BigInteger[] _values = (BigInteger[])args[3];
                    return depositInBatch(_channelIds, _receivers, _transferFromAmounts, _values);
                }
                if (operation == "snapshotStates")
                {
                    BasicMethods.assert(args.Length == 1, "snapshotStates parameter error");
                    byte[] _signedSimplexStateArray = (byte[])args[0];
                    return snapshotStates(_signedSimplexStateArray);
                }
                if (operation == "intendWithdraw")
                {
                    BasicMethods.assert(args.Length == 4, "intendWithdraw parameter error");
                    byte[] _channelId = (byte[])args[0];
                    BigInteger _amount = (BigInteger)args[1];
                    byte[] _recipientChannelId = (byte[])args[2];
                    byte[] _sender = (byte[])args[3];
                    return intendWithdraw(_channelId, _amount, _recipientChannelId, _sender);
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
                if (operation == "cooperativeWithdraw")
                {
                    BasicMethods.assert(args.Length == 1, "cooperativeWithdraw parameter error");
                    byte[] _cooperativeWithdrawRequest = (byte[])args[0];
                    return cooperativeWithdraw(_cooperativeWithdrawRequest);
                }
                if (operation == "intendSettle")
                {
                    BasicMethods.assert(args.Length == 2, "intendSettle parameter error");
                    byte[] _signedSimplexStateArray = (byte[])args[0];
                    byte[] _sender = (byte[])args[1];
                    return intendSettle(_signedSimplexStateArray, _sender);
                }
                if (operation == "clearPays")
                {
                    BasicMethods.assert(args.Length == 4, "clearPays parameter error");
                    byte[] _channelId = (byte[])args[0];
                    byte[] _peerFrom = (byte[])args[1];
                    byte[] _sender = (byte[])args[2];
                    return clearPays(_channelId, _peerFrom, _sender);
                }
                if (operation == "confirmSettle")
                {
                    BasicMethods.assert(args.Length == 1, "confirmSettle parameter error");
                    byte[] _channelId = (byte[])args[0];
                    return confirmSettle(_channelId);
                }
                if (operation == "cooperativeSettle")
                {
                    BasicMethods.assert(args.Length == 1, "cooperativeSettle parameter error");
                    byte[] _settleRequest = (byte[])args[0];
                    return cooperativeSettle(_settleRequest);
                }
                if (operation == "migrateChannelTo")
                {
                    BasicMethods.assert(args.Length == 2, "migrateChannelTo parameter error");
                    byte[] _migrationRequest = (byte[])args[0];
                    byte[] _sender = (byte[])args[1];
                    return migrateChannelTo(_migrationRequest, _sender);
                }
                if (operation == "migrateChannelFrom")
                {
                    BasicMethods.assert(args.Length == 3, "migrateChannelFrom parameter error");
                    byte[] _fromLedgerAddr = (byte[])args[0];
                    byte[] _migrationRequest = (byte[])args[1];
                    byte[] _sender = (byte[])args[2];
                    return migrateChannelFrom(_fromLedgerAddr, _migrationRequest, _sender);
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
            LedgerBalanceLimit.enableBalanceLimits();
            return true;
        }

        [DisplayName("setBalanceLimits")]
        public static bool setBalanceLimits(byte[][] _tokenAddrs, BigInteger[] _limits)
        {
            if (!onlyOwner()) return false;
            LedgerBalanceLimit.setBalanceLimits(_tokenAddrs, _limits);
            return true;
        }

        [DisplayName("disableBalanceLimits")]
        public static bool disableBalanceLimits()
        {
            if (!onlyOwner()) return false;
            LedgerBalanceLimit.disableBalanceLimits();
            return true;
        }

        [DisplayName("enableBalanceLimits")]
        public static bool enableBalanceLimits()
        {
            LedgerBalanceLimit.enableBalanceLimits();
            return true;
        }

        [DisplayName("openChannel")]
        public static bool openChannel(byte[] _openRequest, BigInteger _value)
        {
            LedgerStruct.Ledger ledger = getLedger();
            LedgerOperation.openChannel(ledger, _openRequest, _value, LedgerBalanceLimit.getBalanceLimitsEnabled());
            return true;
        }

        [DisplayName("deposit")]
        public static bool deposit(byte[] _channelId, byte[] _receiver, BigInteger _transferFromAmount, BigInteger _value)
        {
            LedgerStruct.Ledger ledger = getLedger();
            LedgerOperation.deposit(ledger, _channelId, _receiver, _transferFromAmount, _value, LedgerBalanceLimit.getBalanceLimitsEnabled());
            return true;
        }

        [DisplayName("depositInBatch")]
        public static bool depositInBatch(byte[][] _channelIds, byte[][] _receivers, BigInteger[] _transferFromAmounts, BigInteger[] _values)
        {
            BasicMethods.assert(
                _channelIds.Length == _receivers.Length && _receivers.Length == _transferFromAmounts.Length && _transferFromAmounts.Length == _values.Length,
                "Lengths do not match"
                );
            bool balanceLimited = LedgerBalanceLimit.getBalanceLimitsEnabled();
            for (int i = 0; i < _channelIds.Length; i++)
            {
                LedgerOperation.deposit(getLedger(), _channelIds[i], _receivers[i], _transferFromAmounts[i], _values[i], balanceLimited);
            }
            return true;
        }

        [DisplayName("snapshotStates")]
        public static bool snapshotStates(byte[] _signedSimplexStateArray)
        {
            LedgerOperation.snapshotStates(getLedger(), _signedSimplexStateArray);
            return true;
        }

        [DisplayName("intendWithdraw")]
        public static bool intendWithdraw(byte[] _channelId, BigInteger _amount, byte[] _recipientChannelId, byte[] _sender)
        {
            LedgerOperation.intendWithdraw(_sender, getLedger(), _channelId, _amount, _recipientChannelId);
            return true;
        }

        [DisplayName("confirmWithdraw")]
        public static bool confirmWithdraw(byte[] _channelId)
        {
            LedgerOperation.confirmWithdraw(getLedger(), _channelId, LedgerBalanceLimit.getBalanceLimitsEnabled());
            return true;
        }

        [DisplayName("vetoWithdraw")]
        public static bool vetoWithdraw(byte[] _channelId, byte[] _sender)
        {
            LedgerOperation.vetoWithdraw(_sender, getLedger(), _channelId);
            return true;
        }

        [DisplayName("cooperativeWithdraw")]
        public static bool cooperativeWithdraw(byte[] _cooperativeWithdrawRequest)
        {
            LedgerOperation.cooperativeWithdraw(getLedger(), _cooperativeWithdrawRequest, LedgerBalanceLimit.getBalanceLimitsEnabled());
            return true;
        }

        [DisplayName("intendSettle")]
        public static bool intendSettle(byte[] _signedSimplexStateArray, byte[] _sender)
        {
            LedgerOperation.intendSettle(_sender, getLedger(), _signedSimplexStateArray);
            return true;
        }

        [DisplayName("clearPays")]
        public static bool clearPays(byte[] _channelId, byte[] _peerFrom, byte[] _payIdList)
        {
            LedgerOperation.clearPays(getLedger(), _channelId, _peerFrom, _payIdList);
            return true;
        }

        [DisplayName("confirmSettle")]
        public static bool confirmSettle(byte[] _channelId)
        {
            LedgerOperation.confirmSettle(getLedger(), _channelId);
            return true;
        }

        [DisplayName("cooperativeSettle")]
        public static bool cooperativeSettle(byte[] _settleRequest)
        {
            LedgerOperation.cooperativeSettle(getLedger(), _settleRequest);
            return true;
        }

        [DisplayName("migrateChannelTo")]
        public static bool migrateChannelTo(byte[] _migrationRequest, byte[] sender)
        {
            LedgerMigrate.migrateChannelTo(sender, getLedger(), _migrationRequest);
            return true;
        }

        [DisplayName("migrateChannelFrom")]
        public static bool migrateChannelFrom(byte[] _fromLedgerAddr, byte[] _migrationRequest, byte[] sender)
        {
            LedgerMigrate.migrateChannelFrom(sender, getLedger(), _fromLedgerAddr, _migrationRequest);
            return true;
        }

        [DisplayName("getSettleFinalizedTime")]
        public static BigInteger getSettleFinalizedTime(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getSettleFinalizedTime(c);
        }

        [DisplayName("getTokenContract")]
        public static byte[] getTokenContract(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTokenContract(c);
        }

        [DisplayName("getTokenType")]
        public static byte getTokenType(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTokenType(c);
        }

        [DisplayName("getChannelStatus")]
        public static byte getChannelStatus(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getChannelStatus(c);
        }

        [DisplayName("getCooperativeWithdrawSeqNum")]
        public static BigInteger getCooperativeWithdrawSeqNum(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getCooperativeWithdrawSeqNum(c);
        }

        [DisplayName("getTotalBalance")]
        public static BigInteger getTotalBalance(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTotalBalance(c);
        }

        [DisplayName("getBalanceMap")]
        public static LedgerStruct.BalanceMap getBalanceMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getBalanceMap(c);
        }

        [DisplayName("getChannelMigrationArgs")]
        public static LedgerStruct.ChannelMigrationArgs getChannelMigrationArgs(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getChannelMigrationArgs(c);
        }

        [DisplayName("getPeersMigrationInfo")]
        public static LedgerStruct.PeersMigrationInfo getPeersMigrationInfo(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getPeersMigrationInfo(c);
        }

        [DisplayName("getDisputeTimeout")]
        public static BigInteger getDisputeTimeout(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getDisputeTimeout(c);
        }

        [DisplayName("getMigratedTo")]
        public static byte[] getMigratedTo(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getMigratedTo(c);
        }

        [DisplayName("getStateSeqNumMap")]
        public static LedgerStruct.StateSeqNumMap getStateSeqNumMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getStateSeqNumMap(c);
        }

        [DisplayName("getTransferOutMap")]
        public static LedgerStruct.TransferOutMap getTransferOutMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getTransferOutMap(c);
        }

        [DisplayName("getNextPayIdListHashMap")]
        public static LedgerStruct.NextPayIdListHashMap getNextPayIdListHashMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getNextPayIdListHashMap(c);
        }

        [DisplayName("getLastPayResolveDeadlineMap")]
        public static LedgerStruct.LastPayResolveDeadlineMap getLastPayResolveDeadlineMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getLastPayResolveDeadlineMap(c);
        }

        [DisplayName("getPendingPayOutMap")]
        public static LedgerStruct.PendingPayOutMap getPendingPayOutMap(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getPendingPayOutMap(c);
        }

        [DisplayName("getWithdrawIntent")]
        public static LedgerStruct.WithdrawIntent getWithdrawIntent(byte[] _channelId)
        {
            BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
            return LedgerChannel.getWithdrawIntent(c);
        }

        [DisplayName("getChannelStatusNum")]
        public static BigInteger getChannelStatusNum(BigInteger _channelStatus)
        {
            return LedgerOperation.getChannelStatusNum(getLedger(), _channelStatus);
        }

        [DisplayName("getPayRegistry")]
        public static byte[] getPayRegistry()
        {
            return LedgerOperation.getPayRegistry(getLedger());
        }

        [DisplayName("getCelerWallet")]
        public static byte[] getCelerWallet()
        {
            return LedgerOperation.getCelerWallet(getLedger());
        }

        [DisplayName("getBalanceLimit")]
        public static BigInteger getBalanceLimit(byte[] _tokenAddr)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(_tokenAddr), "_tokenAddr illegal");
            return LedgerBalanceLimit.getBalanceLimit(_tokenAddr);
        }

        [DisplayName("getBalanceLimit")]
        public static bool getBalanceLimitsEnabled()
        {
            return LedgerBalanceLimit.getBalanceLimitsEnabled();
        }

        private static bool onlyOwner()
        {
            //Pending
            return true;
        }
    }
}
