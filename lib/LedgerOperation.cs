using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;
using System.Numerics;

public class LedgerOperation : SmartContract
{
    public delegate object NEP5Contract(string method, object[] args);

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

    [DisplayName("ConfirmWithdrawEvent")]
    public static event Action<byte[], BigInteger, byte[], byte[], BigInteger[], BigInteger[], BigInteger> CooperativeWithdrawEvent;

    [DisplayName("IntendSettleEvent")]
    public static event Action<byte[], BigInteger[]> IntendSettleEvent;

    [DisplayName("ClearOnePayEvent")]
    public static event Action<byte[], byte[], byte[], BigInteger> ClearOnePayEvent;

    [DisplayName("CooperativeSettleEvent")]
    public static event Action<byte[], BigInteger[]> CooperativeSettleEvent;

    [DisplayName("ConfirmSettleFailEvent")]
    public static event Action<byte[]> ConfirmSettleFailEvent;

    [DisplayName("ConfirmSettleEvent")]
    public static event Action<byte[], BigInteger[]> ConfirmSettleEvent;

    /*public static Object Main(string operation, params object[] args)
    {
        if (Runtime.Trigger == TriggerType.Verification)
        {
            return true;
        }
        else if (Runtime.Trigger == TriggerType.Application)
        {
            if (operation == "openChannel")
            {
                BasicMethods.assert(args.Length == 3, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _openRequest = (byte[])args[1];
                BigInteger value = ((byte[])args[2]).ToBigInteger();
                return openChannel(_self, _openRequest, value);
            }
            if (operation == "deposit")
            {
                BasicMethods.assert(args.Length == 5, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _channelId = (byte[])args[1];
                byte[] _receiver = (byte[])args[2];
                BigInteger _transferFromAmount = ((byte[])args[3]).ToBigInteger();
                BigInteger value = ((byte[])args[4]).ToBigInteger();
                return deposit(_self, _channelId, _receiver, _transferFromAmount, value);
            }
            if (operation == "snapshotStates")
            {
                BasicMethods.assert(args.Length == 2, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _signedSimplexStateArray = (byte[])args[1];
                return snapshotStates(_self, _signedSimplexStateArray);
            }
            if (operation == "intendWithdraw")
            {
                BasicMethods.assert(args.Length == 5, "params length error");
                byte[] sender = (byte[])args[0];
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[1]);
                byte[] _channelId = (byte[])args[2];
                BigInteger _amount = ((byte[])args[3]).ToBigInteger();
                byte[] _recipientChannelId = ((byte[])args[4]);
                return intendWithdraw(sender, _self, _channelId, _amount, _recipientChannelId);
            }
            if (operation == "confirmWithdraw")
            {
                BasicMethods.assert(args.Length == 2, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _channelId = (byte[])args[1];
                return confirmWithdraw(_self, _channelId);
            }
            if (operation == "vetoWithdraw")
            {
                BasicMethods.assert(args.Length == 3, "params length error");
                byte[] sender = (byte[])args[0];
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[1]);
                byte[] _channelId = (byte[])args[2];
                return vetoWithdraw(sender, _self, _channelId);
            }
            if (operation == "cooperativeWithdraw")
            {
                BasicMethods.assert(args.Length == 2, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _channelId = (byte[])args[1];
                return cooperativeWithdraw(_self, _channelId);
            }
            if (operation == "intendSettle")
            {
                BasicMethods.assert(args.Length == 3, "params length error");
                byte[] sender = (byte[])args[0];
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[1]);
                byte[] _signedSimplexStateArray = (byte[])args[2];
                return intendSettle(sender, _self, _signedSimplexStateArray);
            }
            if (operation == "clearPays")
            {
                BasicMethods.assert(args.Length == 4, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _channelId = (byte[])args[1];
                byte[] _peerFrom = (byte[])args[2];
                byte[] _payIdList = (byte[])args[3];
                return clearPays(_self, _channelId, _peerFrom, _payIdList);
            }
            if (operation == "confirmSettle")
            {
                BasicMethods.assert(args.Length == 2, "params length error");
                LedgerStruct.Ledger _self = (LedgerStruct.Ledger)Neo.SmartContract.Framework.Helper.Deserialize((byte[])args[0]);
                byte[] _channelId = (byte[])args[1];
                return confirmSettle(_self, _channelId);
            }
        }
        return false;
    }*/

    [DisplayName("openChannelInner")]
    public static bool openChannelInner(LedgerStruct.Ledger _self, byte[][] pubKeys, byte[] _openRequest, bool _balanceLimited)
    {
        PbChain.OpenChannelRequest openRequest = (PbChain.OpenChannelRequest)Neo.SmartContract.Framework.Helper.Deserialize(_openRequest);
        PbEntity.PaymentChannelInitializer channelInitializer = (PbEntity.PaymentChannelInitializer)Neo.SmartContract.Framework.Helper.Deserialize(openRequest.channelInitializer);
        PbEntity.TokenDistribution tokenDistribution = channelInitializer.initDistribution;
        PbEntity.AccountAmtPair[] accountAmtPair = tokenDistribution.distribution;
        BasicMethods.assert(accountAmtPair.Length == 2, "Wrong length");
        BasicMethods.assert(Blockchain.GetHeight() <= channelInitializer.openDeadline, "Open deadline passed");

        PbEntity.TokenInfo token = tokenDistribution.token;
        BigInteger _value = LedgerStruct.getTransactionValue(token.tokenType, getCelerWalletInner(_self));
        BasicMethods.assert(_value >= 0, "value is illegal");

        PbEntity.AccountAmtPair accountAmtPair0 = accountAmtPair[0];
        PbEntity.AccountAmtPair accountAmtPair1 = accountAmtPair[1];
        BigInteger[] amounts = new BigInteger[] { accountAmtPair0.amt, accountAmtPair1.amt };
        BasicMethods.assert(amounts[0] >= 0, "amount0 is less than 0");
        BasicMethods.assert(amounts[1] >= 0, "amount1 is less than 0");
        byte[][] peerAddrs = new byte[][] { accountAmtPair0.account, accountAmtPair1.account };
        byte[] peerAddr0 = peerAddrs[0];
        byte[] peerAddr1 = peerAddrs[1];
        BasicMethods.assert(peerAddr0.ToBigInteger() < peerAddr1.ToBigInteger(), "Peer addrs are not ascending");

        byte[] celerWallet = _self.celerWallet;
        byte[] h = SmartContract.Hash256(openRequest.channelInitializer);
        LedgerStruct.ChannelInfo channelInfo = _createWallet(_self, celerWallet, peerAddrs, h);
        LedgerStruct.Channel c = channelInfo.channel;
        c.disputeTimeout = channelInitializer.disputeTimeout;
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        c = _updateChannelStatus(c, channelStatus.Operable);
        c.token = _validateTokenInfo(token);
        LedgerStruct.PeerProfile peerProfile0 = c.peerProfiles[0];
        LedgerStruct.PeerProfile peerProfile1 = c.peerProfiles[1];
        peerProfile0.peerAddr = peerAddr0;
        peerProfile0.deposit = amounts[0];
        peerProfile1.peerAddr = peerAddr1;
        peerProfile1.deposit = amounts[1];
        c.peerProfiles[0] = peerProfile0;
        c.peerProfiles[1] = peerProfile1;
        BasicMethods.assert(LedgerChannel._checkCoSignatures(c, h, pubKeys, openRequest.sigs), "Check co-sigs failed");
        LedgerStruct.setChannelMap(channelInfo.channelId, c);
        OpenChannelEvent(channelInfo.channelId, token.tokenType, token.address, peerAddrs, amounts);

        BigInteger amtSum = amounts[0] + amounts[1];
        //if total deposit is 0
        if (amtSum == 0)
        {
            BasicMethods.assert(_value == 0, "msg.value is not 0");
            return false;
        }

        //Pending debugging
        // if total deposit is larger than 0
        if (_balanceLimited)
            BasicMethods.assert(amtSum <= LedgerBalanceLimit.getBalanceLimitInner(token.address), "Balance exceeds limit");
        PbEntity.TokenType tokenType = PbEntity.getStandardTokenType();
        if (token.tokenType == tokenType.NEO || token.tokenType == tokenType.GAS)
        {
            byte msgValueReceiver = channelInitializer.msgValueReceiver;
            BasicMethods.assert(msgValueReceiver == 0 || msgValueReceiver == 1, "Illegal msgValueReceiver");
            BasicMethods.assert(_value == amounts[msgValueReceiver] + amounts[1 - msgValueReceiver], "value mismatch");
            if (_value > 0)
            {
                if (token.tokenType == tokenType.NEO)
                {
                    NEP5Contract dyncall = (NEP5Contract)celerWallet.ToDelegate();
                    byte[] channelId = (byte[])dyncall("depositneo", new object[] { channelInfo.channelId });
                }
                else
                {
                    NEP5Contract dyncall = (NEP5Contract)celerWallet.ToDelegate();
                    byte[] channelId = (byte[])dyncall("depositgas", new object[] { channelInfo.channelId });
                }
            }
        } else
        {
            BasicMethods.assert(false, "Unsupported token type");
        }
        
        return true;
    }

    [DisplayName("depositInner")]
    public static bool depositInner(LedgerStruct.Ledger _self, byte[] _channelId, byte[] _receiver, BigInteger _transferFromAmount, bool _balanceLimited)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId is illegal");
        BasicMethods.assert(BasicMethods._isLegalAddress(_receiver), "_receiver is illegal");

        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        PbEntity.TokenInfo token = c.token;
        PbEntity.TokenType tokenType = PbEntity.getStandardTokenType();
        BigInteger _value = LedgerStruct.getTransactionValue(token.tokenType, getCelerWalletInner(_self));
        BasicMethods.assert(_value >= 0, "value is illegal");
        BasicMethods.assert(_value == _transferFromAmount, "value is not same as announced");

        _addDeposit(_self, _channelId, _receiver, _value, _balanceLimited);
        byte[] _w = _self.celerWallet;
        if (token.tokenType == tokenType.NEO)
        {
            NEP5Contract dyncall = (NEP5Contract)_w.ToDelegate();
            dyncall("depositneo", new object[] { _channelId });
        } else if (token.tokenType == tokenType.GAS)
        {
            NEP5Contract dyncall = (NEP5Contract)_w.ToDelegate();
            dyncall("depositgas", new object[] { _channelId });
        } else
        {
            BasicMethods.assert(false, "Unsupported token type");
        }
        return true;
    }

    [DisplayName("snapshotStatesInner")]
    public static bool snapshotStatesInner(LedgerStruct.Ledger _self, byte[][] pubKeys, byte[] _signedSimplexStateArray)
    {
        PbChain.SignedSimplexStateArray signedSimplexStateArray = (PbChain.SignedSimplexStateArray)Neo.SmartContract.Framework.Helper.Deserialize(_signedSimplexStateArray);
        PbChain.SignedSimplexState[] signedSimplexStates = signedSimplexStateArray.signedSimplexStates;
        int simplexStatesNum = signedSimplexStates.Length;

        // snapshot each state
        PbChain.SignedSimplexState signedSimplexState = signedSimplexStates[0];
        PbEntity.SimplexPaymentChannel simplexState = (PbEntity.SimplexPaymentChannel)Neo.SmartContract.Framework.Helper.Deserialize(signedSimplexState.simplexState);
        for (int i = 0; i < simplexStatesNum; i++)
        {
            byte[] currentChannelId = simplexState.channelId;
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(currentChannelId);

            LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
            BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");

            PbChain.SignedSimplexState signedSimplexStateI = signedSimplexStates[i];
            byte[] stateHash = SmartContract.Hash256(signedSimplexStateI.simplexState);
            byte[][] sigs = signedSimplexStateI.sigs;
            BasicMethods.assert(LedgerChannel._checkCoSignatures(c, stateHash, pubKeys, sigs), "Check co-sigs failed");
            uint peerFromId = LedgerChannel._getPeerId(c, simplexState.peerFrom);
            LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
            LedgerStruct.PeerProfile peerProfile = peerProfiles[peerFromId];
            LedgerStruct.PeerState state = peerProfile.state;
            BasicMethods.assert(simplexState.seqNum > state.seqNum, "seqNum error");

            // no need to update nextPayIdListHash and lastPayResolveDeadline for snapshot purpose
            state.seqNum = simplexState.seqNum;
            PbEntity.TokenTransfer tokenTransfer = simplexState.transferToPeer;
            PbEntity.AccountAmtPair accountAmtPair = tokenTransfer.receiver;
            state.transferOut = accountAmtPair.amt;
            state.pendingPayOut = simplexState.totalPendingAmount;

            if (i == simplexStatesNum - 1)
            {
                SnapshotStatesEvent(currentChannelId, LedgerChannel._getStateSeqNums(c));
            }
            else if (i < simplexStatesNum - 1)
            {
                PbChain.SignedSimplexState signedSimplexStateI1 = signedSimplexStates[i + 1];
                simplexState = (PbEntity.SimplexPaymentChannel)Neo.SmartContract.Framework.Helper.Deserialize(
                    signedSimplexStateI1.simplexState
                );
                // enforce channelIds of simplex states are ascending
                byte[] channelId = simplexState.channelId;
                BasicMethods.assert(currentChannelId.ToBigInteger() <= channelId.ToBigInteger(), "Non-ascending channelIds");
                if (currentChannelId.ToBigInteger() < channelId.ToBigInteger())
                {
                    SnapshotStatesEvent(currentChannelId, LedgerChannel._getStateSeqNums(c));
                }
            }
            else
            {
                BasicMethods.assert(false, "Error");
            }
            LedgerStruct.setChannelMap(currentChannelId, c);
        }
        return true;
    }

    [DisplayName("intendWithdrawInner")]
    public static bool intendWithdrawInner(byte[] _sender, LedgerStruct.Ledger _self, byte[] _channelId, BigInteger _amount, byte[] _recipientChannelId)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_sender), "sender illegal");
        BasicMethods.assert(Runtime.CheckWitness(_sender), "sender check witness failed");
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        BasicMethods.assert(_amount >= 0, "_amount illegal");
        BasicMethods.assert(BasicMethods._isByte32(_recipientChannelId), "_recipientChannelId illegal");

        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");
        // withdrawIntent.receiver is address(0) if and only if there is no pending WithdrawIntent,
        // because withdrawIntent.receiver may only be set as msg.sender which can't be address(0).
        byte[] receiver = withdrawIntent.receiver;
        BasicMethods.assert(receiver.ToBigInteger() == 0, "Pending withdraw intent exists");
        BasicMethods.assert(LedgerChannel._isPeer(c, _sender), "Sender is not peer");

        withdrawIntent.receiver = _sender;
        withdrawIntent.amount = _amount;
        withdrawIntent.requestTime = Blockchain.GetHeight();
        withdrawIntent.recipientChannelId = _recipientChannelId;
        c.withdrawIntent = withdrawIntent;
        LedgerStruct.setChannelMap(_channelId, c);

        IntendWithdrawEvent(_channelId, _sender, _amount);
        return true;
    }

    [DisplayName("confirmWithdrawInner")]
    public static bool confirmWithdrawInner(LedgerStruct.Ledger _self, byte[] _channelId, bool _balanceLimited)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");
        LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;
        byte[] receiver = withdrawIntent.receiver;
        BasicMethods.assert(receiver.ToBigInteger() != 0, "No pending withdraw intent");

        BasicMethods.assert(
            Blockchain.GetHeight() >= withdrawIntent.requestTime + c.disputeTimeout,
            "Dispute not timeout"
        );

        BigInteger amount = withdrawIntent.amount;
        byte[] recipientChannelId = withdrawIntent.recipientChannelId;
        withdrawIntent.receiver = null;
        withdrawIntent.amount = 0;
        withdrawIntent.requestTime = 0;
        withdrawIntent.recipientChannelId = null;
        c.withdrawIntent = withdrawIntent;

        // check withdraw limit
        byte rid = LedgerChannel._getPeerId(c, receiver);
        byte pid = (byte)(1 - rid);
        LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
        LedgerStruct.PeerProfile peerProfileRid = peerProfiles[rid];
        LedgerStruct.PeerProfile peerProfilePid = peerProfiles[pid];
        LedgerStruct.PeerState peerStateRid = peerProfileRid.state;
        LedgerStruct.PeerState peerStatePid = peerProfilePid.state;
        BigInteger withdrawLimit = peerProfileRid.deposit
            + peerStatePid.transferOut
            - peerProfileRid.withdrawal
            - peerStateRid.transferOut
            - peerStateRid.pendingPayOut;
        BasicMethods.assert(amount <= withdrawLimit, "Exceed withdraw limit");

        c = LedgerChannel._addWithdrawal(c, receiver, amount);
        LedgerStruct.BalanceMap balanceMap = LedgerChannel.getBalanceMapInner(c);
        ConfirmWithdrawEvent(_channelId, amount, receiver, recipientChannelId, balanceMap.deposits, balanceMap.withdrawals);
        LedgerStruct.setChannelMap(_channelId, c);
        _withdrawFunds(_self, _channelId, receiver, amount, recipientChannelId, _balanceLimited);
        return true;
    }

    [DisplayName("vetoWithdrawInner")]
    public static bool vetoWithdrawInner(byte[] _sender, LedgerStruct.Ledger _self, byte[] _channelId)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_sender), "sender illegal");
        BasicMethods.assert(Runtime.CheckWitness(_sender), "sender check witness failed");
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");
        LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;
        byte[] receiver = withdrawIntent.receiver;
        BasicMethods.assert(receiver.ToBigInteger() != 0, "No pending withdraw intent");
        BasicMethods.assert(LedgerChannel._isPeer(c, _sender), "sender is not peer");
        withdrawIntent.receiver = null;
        withdrawIntent.amount = 0;
        withdrawIntent.requestTime = 0;
        withdrawIntent.recipientChannelId = null;
        c.withdrawIntent = withdrawIntent;
        LedgerStruct.setChannelMap(_channelId, c);
        VetoWithdrawEvent(_channelId);
        return true;
    }

    [DisplayName("cooperativeWithdrawInner")]
    public static bool cooperativeWithdrawInner(LedgerStruct.Ledger _self, byte[][] pubKeys, byte[] _cooperativeWithdrawRequest, bool _balanceLimited)
    {
        PbChain.CooperativeWithdrawRequest cooperativeWithdrawRequest =
            (PbChain.CooperativeWithdrawRequest)Neo.SmartContract.Framework.Helper.Deserialize(_cooperativeWithdrawRequest);
        PbEntity.CooperativeWithdrawInfo withdrawInfo =
            (PbEntity.CooperativeWithdrawInfo)Neo.SmartContract.Framework.Helper.Deserialize(cooperativeWithdrawRequest.withdrawInfo);
        byte[] channelId = withdrawInfo.channelId;
        byte[] recipientChannelId = withdrawInfo.recipientChannelId;
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(channelId);

        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");
        byte[] h = SmartContract.Hash256(cooperativeWithdrawRequest.withdrawInfo);
        BasicMethods.assert(
            LedgerChannel._checkCoSignatures(c, h, pubKeys, cooperativeWithdrawRequest.sigs),
            "Check co-sigs failed"
        );
        // require an increment of exactly 1 for seqNum of each cooperative withdraw request
        BasicMethods.assert(
            withdrawInfo.seqNum - c.cooperativeWithdrawSeqNum == 1,
            "seqNum error"
        );
        BasicMethods.assert(Blockchain.GetHeight() <= withdrawInfo.withdrawDeadline, "Withdraw deadline passed");

        PbEntity.AccountAmtPair accountAmtPair = withdrawInfo.withdraw;
        byte[] receiver = accountAmtPair.account;
        c.cooperativeWithdrawSeqNum = withdrawInfo.seqNum;
        BigInteger amount = accountAmtPair.amt;

        // this implicitly require receiver be a peer
        c = LedgerChannel._addWithdrawal(c, receiver, amount);
        LedgerStruct.setChannelMap(channelId, c);
        LedgerStruct.BalanceMap balanceMap = LedgerChannel.getBalanceMapInner(c);
        CooperativeWithdrawEvent(
            channelId,
            amount,
            receiver,
            recipientChannelId,
            balanceMap.deposits,
            balanceMap.withdrawals,
            withdrawInfo.seqNum
        );

        _withdrawFunds(_self, channelId, receiver, amount, recipientChannelId, _balanceLimited);
        return true;
    }

    [DisplayName("intendSettleInner")]
    public static bool intendSettleInner(byte[] _sender, LedgerStruct.Ledger _self, byte[][] pubKeys, byte[] _signedSimplexStateArray)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_sender), "sender illegal");
        BasicMethods.assert(Runtime.CheckWitness(_sender), "sender check witness failed");
        PbChain.SignedSimplexStateArray signedSimplexStateArray =
            (PbChain.SignedSimplexStateArray)Neo.SmartContract.Framework.Helper.Deserialize(_signedSimplexStateArray);
        PbChain.SignedSimplexState[] states = signedSimplexStateArray.signedSimplexStates;
        int simplexStatesNum = states.Length;

        PbChain.SignedSimplexState state0 = states[0];
        PbEntity.SimplexPaymentChannel simplexState =
            (PbEntity.SimplexPaymentChannel)Neo.SmartContract.Framework.Helper.Deserialize(state0.simplexState);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        for (uint i = 0; i < simplexStatesNum; i++)
        {
            byte[] currentChannelId = simplexState.channelId;
            BasicMethods.assert(BasicMethods._isByte32(currentChannelId), "currentChannelId illegal");
            LedgerStruct.Channel c = LedgerStruct.getChannelMap(currentChannelId);

            if (LedgerChannel._isPeer(c, _sender))
            {
                BasicMethods.assert(
                    c.status == channelStatus.Operable ||
                    c.status == channelStatus.Settling,
                    "Peer channel status error"
                );
            }
            else
            {
                // A nonpeer cannot be the first one to call intendSettle
                BasicMethods.assert(c.status == channelStatus.Settling, "Nonpeer channel status error");
            }
            BasicMethods.assert(
                c.settleFinalizedTime == 0 || Blockchain.GetHeight() < c.settleFinalizedTime,
                "Settle has already finalized"
            );

            PbChain.SignedSimplexState stateI = states[i];
            byte[] stateHash = SmartContract.Hash256(stateI.simplexState);
            byte[][] sigs = stateI.sigs;
            
            if (simplexState.seqNum > 0)
            {  // non-null state
                BasicMethods.assert(LedgerChannel._checkCoSignatures(c, stateHash, pubKeys, sigs), "Check co-sigs failed");
                byte peerFromId = LedgerChannel._getPeerId(c, simplexState.peerFrom);
                LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
                LedgerStruct.PeerProfile peerProfile = peerProfiles[peerFromId];
                LedgerStruct.PeerState state = peerProfile.state;
                // ensure each state can be intendSettle at most once
                if (c.status == channelStatus.Operable)
                {
                    // "==" is the case of cooperative on-chain checkpoint
                    BasicMethods.assert(simplexState.seqNum >= state.seqNum, "seqNum error");
                }
                else if (c.status == channelStatus.Settling)
                {
                    BasicMethods.assert(simplexState.seqNum > state.seqNum, "seqNum error");
                }
                else
                {
                    BasicMethods.assert(false, "Error");
                }

                // update simplexState-dependent fields
                state.seqNum = simplexState.seqNum;
                PbEntity.TokenTransfer tokenTransfer = simplexState.transferToPeer;
                PbEntity.AccountAmtPair receiver = tokenTransfer.receiver;
                state.transferOut = receiver.amt;
                PbEntity.PayIdList pendingPayIds = simplexState.pendingPayIds;
                state.nextPayIdListHash = pendingPayIds.nextListHash;
                state.lastPayResolveDeadline = simplexState.lastPayResolveDeadline;
                // updating pendingPayOut is only needed when migrating ledger during settling phrase, which will
                // affect the withdraw limit after the migration.
                // if nextListHash is bytes32(0), state.pendingPayOut will be set as 0 by _clearPays()
                byte[] nextListHash = pendingPayIds.nextListHash;
                if (nextListHash.ToBigInteger() != 0)
                {
                    state.pendingPayOut = simplexState.totalPendingAmount;
                }
                peerProfile.state = state;
                peerProfiles[peerFromId] = peerProfile;
                c.peerProfiles = peerProfiles;
                LedgerStruct.setChannelMap(currentChannelId, c);
                _clearPays(_self, currentChannelId, peerFromId, simplexState.pendingPayIds);
            }
            else if (simplexState.seqNum == 0)
            {  // null state
                // this implies both stored seqNums are 0
                BasicMethods.assert(c.settleFinalizedTime == 0, "intendSettle before");
                BasicMethods.assert(
                    sigs.Length == 1 && pubKeys.Length == 1 && LedgerChannel._checkSingleSignature(c, stateHash, pubKeys[0], sigs[0]),
                    "Check sig failed"
                );
            }
            else
            {
                BasicMethods.assert(false, "Error");
            }

            if (i == simplexStatesNum - 1)
            {
                c = _updateOverallStatesByIntendState(currentChannelId);
            }
            else if (i < simplexStatesNum - 1)
            {
                PbChain.SignedSimplexState stateI1 = states[i + 1];
                simplexState = (PbEntity.SimplexPaymentChannel)Neo.SmartContract.Framework.Helper.Deserialize(
                    stateI1.simplexState
                );
                // enforce channelIds of simplex states are ascending
                byte[] channelId = simplexState.channelId;
                BasicMethods.assert(currentChannelId.ToBigInteger() <= channelId.ToBigInteger(), "Non-ascending channelIds");
                if (currentChannelId.ToBigInteger() < channelId.ToBigInteger())
                {
                    c = _updateOverallStatesByIntendState(currentChannelId);
                }
            }
            else
            {
                BasicMethods.assert(false, "Error");
            }
            LedgerStruct.setChannelMap(currentChannelId, c);
        }

        return true;
    }

    [DisplayName("clearPaysInner")]
    public static bool clearPaysInner(LedgerStruct.Ledger _self, byte[] _channelId, byte[] _peerFrom, byte[] _payIdList)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        BasicMethods.assert(BasicMethods._isLegalAddress(_peerFrom), "_peerFrom illegal");

        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Settling, "Channel status error");
        byte peerFromId = LedgerChannel._getPeerId(c, _peerFrom);
        BasicMethods.assert(peerFromId == 0 || peerFromId == 1, "peerFromId illegal");
        byte[] listHash = SmartContract.Hash256(_payIdList);
        LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
        LedgerStruct.PeerProfile peerProfile = peerProfiles[peerFromId];
        LedgerStruct.PeerState state = peerProfile.state;
        BasicMethods.assert(state.nextPayIdListHash.Equals(listHash), "List hash mismatch");
        PbEntity.PayIdList payIdList = (PbEntity.PayIdList)Neo.SmartContract.Framework.Helper.Deserialize(_payIdList);
        state.nextPayIdListHash = payIdList.nextListHash;
        LedgerStruct.setChannelMap(_channelId, c);
        _clearPays(_self, _channelId, peerFromId, payIdList);
        return true;
    }

    [DisplayName("confirmSettleInner")]
    public static bool confirmSettleInner(LedgerStruct.Ledger _self, byte[] _channelId)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");

        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
        BigInteger blockNumber = Blockchain.GetHeight(); ;
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Settling, "Channel status error");
        // require no new intendSettle can be called
        BasicMethods.assert(blockNumber >= c.settleFinalizedTime, "Settle is not finalized");

        // require channel status of current intendSettle has been finalized,
        // namely all payments have already been either cleared or expired
        // Note: this lastPayResolveDeadline should use
        //   (the actual last resolve deadline of all pays + clearPays safe margin)
        //   to ensure that peers have enough time to clearPays before confirmSettle.
        //   However this only matters if there are multiple blocks of pending pay list
        //   i.e. the nextPayIdListHash after intendSettle is not bytes32(0).
        // TODO: add an additional clearSafeMargin param or change the semantics of
        //   lastPayResolveDeadline to also include clearPays safe margin and rename it.

        LedgerStruct.PeerProfile peerProfile0 = peerProfiles[0];
        LedgerStruct.PeerProfile peerProfile1 = peerProfiles[1];
        LedgerStruct.PeerState state0 = peerProfile0.state;
        LedgerStruct.PeerState state1 = peerProfile1.state;
        byte[] nextPayIdListHash0 = state0.nextPayIdListHash;
        byte[] nextPayIdListHash1 = state1.nextPayIdListHash;

        BasicMethods.assert(
            (nextPayIdListHash0.AsBigInteger() == 0 ||
                blockNumber > state0.lastPayResolveDeadline) &&
            (nextPayIdListHash1.AsBigInteger() == 0 ||
                blockNumber > state1.lastPayResolveDeadline),
            "Payments are not finalized"
        );

        LedgerStruct.SettleBalance settleBalance = LedgerChannel._validateSettleBalance(c);
        if (!(settleBalance.isSettled == 1))
        {
            c = _resetDuplexState(c);
            LedgerStruct.setChannelMap(_channelId, c);
            ConfirmSettleFailEvent(_channelId);
            return false;
        }

        c = _updateChannelStatus(c, channelStatus.Closed);
        LedgerStruct.setChannelMap(_channelId, c);

        ConfirmSettleEvent(_channelId, settleBalance.balance);

        // Withdrawal from Contracts pattern is needless here,
        // because peers need to sign messages which implies that they can't be contracts
        PbEntity.TokenInfo token = c.token;
        _batchTransferOut(
            _self,
            _channelId,
            token.address,
            new byte[][] { peerProfile0.peerAddr, peerProfile1.peerAddr },
            settleBalance.balance
        );

        return true;
    }

    [DisplayName("cooperativeSettleInner")]
    public static bool cooperativeSettleInner(LedgerStruct.Ledger _self, byte[][] pubKeys, byte[] _settleRequest)
    {
        PbChain.CooperativeSettleRequest settleRequest =
            (PbChain.CooperativeSettleRequest)Neo.SmartContract.Framework.Helper.Deserialize(_settleRequest);
        PbEntity.CooperativeSettleInfo settleInfo =
            (PbEntity.CooperativeSettleInfo)Neo.SmartContract.Framework.Helper.Deserialize(settleRequest.settleInfo);
        byte[] channelId = settleInfo.channelId;
        BasicMethods.assert(BasicMethods._isByte32(channelId), "_channelId illegal");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(
            c.status == channelStatus.Operable ||
            c.status == channelStatus.Settling,
            "Channel status error"
        );

        byte[] h = SmartContract.Hash256(settleRequest.settleInfo);
        BasicMethods.assert(LedgerChannel._checkCoSignatures(c, h, pubKeys, settleRequest.sigs), "Check co-sigs failed");

        LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
        LedgerStruct.PeerProfile peerProfile0 = peerProfiles[0];
        LedgerStruct.PeerProfile peerProfile1 = peerProfiles[1];
        LedgerStruct.PeerState state0 = peerProfile0.state;
        LedgerStruct.PeerState state1 = peerProfile1.state;
        byte[][] peerAddrs = { peerProfile0.peerAddr, peerProfile1.peerAddr };
        BasicMethods.assert(
            settleInfo.seqNum > state0.seqNum &&
                settleInfo.seqNum > state1.seqNum,
            "seqNum error"
        );
        BasicMethods.assert(settleInfo.settleDeadline >= Blockchain.GetHeight(), "Settle deadline passed");
        // require distribution is consistent with the order of peerAddrs in channel
        PbEntity.AccountAmtPair[] settleBalances = settleInfo.settleBalance;
        PbEntity.AccountAmtPair settleBalance0 = settleBalances[0];
        PbEntity.AccountAmtPair settleBalance1 = settleBalances[1];
        BasicMethods.assert(
            settleBalance0.account == peerAddrs[0] &&
                settleBalance1.account == peerAddrs[1],
            "Settle accounts mismatch"
        );

        BigInteger[] settleBalance = { settleBalance0.amt, settleBalance1.amt };
        BasicMethods.assert(settleBalance[0] + settleBalance[1] == LedgerChannel.getTotalBalanceInner(c), "Balance sum mismatch");

        c = _updateChannelStatus(c, channelStatus.Closed);
        LedgerStruct.setChannelMap(channelId, c);
        CooperativeSettleEvent(channelId, settleBalance);

        PbEntity.TokenInfo token = c.token;
        _batchTransferOut(_self, channelId, token.address, peerAddrs, settleBalance);

        return true;
    }

    [DisplayName("getChannelStatusNumInner")]
    public static BigInteger getChannelStatusNumInner(BigInteger _channelStatus)
    {
        return LedgerStruct.getChannelStatusNums(_channelStatus);
    }

    [DisplayName("getPayRegistryInner")]
    public static byte[] getPayRegistryInner(LedgerStruct.Ledger _self)
    {
        return _self.payRegistry;
    }

    [DisplayName("getCelerWalletInner")]
    public static byte[] getCelerWalletInner(LedgerStruct.Ledger _self)
    {
        return _self.celerWallet;
    }

    private static LedgerStruct.ChannelInfo _createWallet(LedgerStruct.Ledger _self, byte[] _w, byte[][] _peers, byte[] _nonce)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_w), "_w illegal");
        BasicMethods.assert(_peers.Length == 2, "_peers length illegal");
        byte[] peer0 = _peers[0];
        byte[] peer1 = _peers[1];
        BasicMethods.assert(BasicMethods._isLegalAddress(peer0), "_peer0 illegal");
        BasicMethods.assert(BasicMethods._isLegalAddress(peer1), "_peer1 illegal");
        BasicMethods.assert(BasicMethods._isByte32(_nonce), "_nonce illegal");
        byte[][] owners = new byte[2][];

        owners[0] = BasicMethods.clone(peer0);
        owners[1] = BasicMethods.clone(peer1);
        
        object[] args = new object[] { owners, ExecutionEngine.ExecutingScriptHash, _nonce };
        NEP5Contract dyncall = (NEP5Contract)_w.ToDelegate();
        byte[] channelId = (byte[])dyncall("create", args);
        BasicMethods.assert(channelId.ToBigInteger() != 0, "channelId gets 0");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Uninitialized, "Occupied channelId");

        LedgerStruct.ChannelInfo channelInfo = new LedgerStruct.ChannelInfo();
        channelInfo.channelId = channelId;
        channelInfo.channel = c;
        return channelInfo;
    }

    private static bool _addDeposit(LedgerStruct.Ledger _self, byte[] _channelId, byte[] _receiver, BigInteger _amount, bool _balanceLimited)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        BasicMethods.assert(BasicMethods._isLegalAddress(_receiver), "_receiver illegal");
        BasicMethods.assert(_amount >= 0, "_amount illegal");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Operable, "Channel status error");

        // this implicitly require _receiver be a peer
        byte rid = LedgerChannel._getPeerId(c, _receiver);
        if (_balanceLimited)
        {
            PbEntity.TokenInfo tokenInfo = c.token;
            byte[] address = tokenInfo.address;
            BasicMethods.assert(_amount + LedgerChannel.getTotalBalanceInner(c) <= LedgerBalanceLimit.getBalanceLimitInner(address), "Balance exceeds limit");
        }

        LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
        LedgerStruct.PeerProfile peerProfile = peerProfiles[rid];
        peerProfile.deposit = peerProfile.deposit + _amount;
        LedgerStruct.BalanceMap balanceMap = LedgerChannel.getBalanceMapInner(c);
        LedgerStruct.setChannelMap(_channelId, c);
        DepositEvent(_channelId, balanceMap.peerAddrs, balanceMap.deposits, balanceMap.withdrawals);
        return true;
    }

    private static bool _batchTransferOut(LedgerStruct.Ledger _self, byte[] _channelId, byte[] _tokenAddr, byte[][] _receivers, BigInteger[] _amounts)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        BasicMethods.assert(BasicMethods._isLegalAddress(_tokenAddr), "_receiver illegal");
        BasicMethods.assert(_receivers.Length == 2, "_receivers length is not 2");
        for (int i = 0; i < 2; i++)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(_receivers[i]), "_receivers" + i + " is illegal");
        }
        BasicMethods.assert(_amounts.Length == 2, "_amounts length is not 2");
        for (int i = 0; i < 2; i++)
        {
            BasicMethods.assert(_amounts[i] >= 0, "_amounts" + i + " is illegal");
        }

        for (uint i = 0; i < 2; i++)
        {
            if (_amounts[i] == 0) { continue; }

            byte[] celerWallet = _self.celerWallet;
            NEP5Contract dyncall = (NEP5Contract)celerWallet.ToDelegate();
            Object[] args = new object[] { _channelId, _tokenAddr, _receivers[i], _amounts[i] };
            dyncall("withdraw", args);
        }
        return true;
    }

    private static bool _withdrawFunds(LedgerStruct.Ledger _self, byte[] _channelId, byte[] _receiver, BigInteger _amount, byte[] _recipientChannelId, bool _balanceLimited)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        BasicMethods.assert(BasicMethods._isLegalAddress(_receiver), "_receiver illegal");
        BasicMethods.assert(_amount >= 0, "_amount illegal");
        BasicMethods.assert(BasicMethods._isByte32(_recipientChannelId), "_recipientChannelId illegal");

        if (_amount == 0) return true;
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        byte[] celerWallet = _self.celerWallet;
        NEP5Contract dyncall = (NEP5Contract)celerWallet.ToDelegate();
        PbEntity.TokenInfo channelTokenInfo = c.token;
        if (_recipientChannelId.ToBigInteger() == 0)
        {
            Object[] args = new object[] { _channelId, channelTokenInfo.address, _receiver, _amount };
            dyncall("withdraw", args);
        }
        else
        {
            LedgerStruct.Channel recipientChannel = LedgerStruct.getChannelMap(_recipientChannelId);
            PbEntity.TokenInfo tokenInfo = recipientChannel.token;
            BasicMethods.assert(
                channelTokenInfo.tokenType == tokenInfo.tokenType &&
                    channelTokenInfo.address == tokenInfo.address,
                "Token mismatch of recipient channel"
            );
            _addDeposit(_self, _recipientChannelId, _receiver, _amount, _balanceLimited);

            // move funds from one channel's wallet to another channel's wallet
            Object[] args = new object[] { _channelId, _recipientChannelId, channelTokenInfo, _receiver, _amount };
            dyncall("transferToWallet", args);
        }
        return true;
    }

    private static LedgerStruct.Channel _resetDuplexState(LedgerStruct.Channel c)
    {
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        c.settleFinalizedTime = 0;
        c = _updateChannelStatus(c, channelStatus.Operable);
        c.peerProfiles = null;
        // reset possibly remaining WithdrawIntent freezed by previous intendSettle()
        LedgerStruct.WithdrawIntent withdrawIntent = c.withdrawIntent;
        withdrawIntent.receiver = null;
        withdrawIntent.amount = 0;
        withdrawIntent.requestTime = 0;
        withdrawIntent.recipientChannelId = null;
        c.withdrawIntent = withdrawIntent;

        return c;
    }

    private static bool _clearPays(LedgerStruct.Ledger _self, byte[] _channelId, byte _peerId, PbEntity.PayIdList _payIdList)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        BasicMethods.assert(_peerId == 0 || _peerId == 1, "_channelId illegal");

        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        LedgerStruct.PeerProfile[] peerProfiles = c.peerProfiles;
        LedgerStruct.PeerProfile peerProfile = peerProfiles[_peerId];
        LedgerStruct.PeerState state = peerProfile.state;

        NEP5Contract dyncall = (NEP5Contract)_self.payRegistry.ToDelegate();
        Object[] args = new object[] { _payIdList.payIds, state.lastPayResolveDeadline };
        BigInteger[] outAmts = (BigInteger[])dyncall("getPayAmounts", args);

        BigInteger totalAmtOut = 0;
        byte[][] payIds = _payIdList.payIds;
        for (uint i = 0; i < outAmts.Length; i++)
        {
            totalAmtOut = totalAmtOut + outAmts[i];
            ClearOnePayEvent(_channelId, payIds[i], peerProfile.peerAddr, outAmts[i]);
        }
        state.transferOut = state.transferOut + totalAmtOut;
        // updating pendingPayOut is only needed when migrating ledger during settling phrase, which will
        // affect the withdraw limit after the migration.
        byte[] nextListHash = _payIdList.nextListHash;
        if (nextListHash.ToBigInteger() == 0)
        {
            // if there are no more uncleared pays in this state, the pendingPayOut must be 0
            state.pendingPayOut = 0;
        }
        else
        {
            // Note: if there are more uncleared pays in this state, because resolved pay amount
            //   is always less than or equal to the corresponding maximum amount counted in
            //   pendingPayOut, the updated pendingPayOut may be equal to or larger than the real
            //   pendingPayOut. This will lead to decreasing the maximum withdraw amount (withdrawLimit)
            //   of the peer in non-cooperative withdraw flow, but protect the fund in the channel
            //   from potentially malicious non-cooperative withdraw.
            state.pendingPayOut =
                state.pendingPayOut - totalAmtOut;
        }
        LedgerStruct.setChannelMap(_channelId, c);
        return true;
    }

    private static LedgerStruct.Channel _updateOverallStatesByIntendState(byte[] _channelId)
    {
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "_channelId illegal");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(_channelId);
        c.settleFinalizedTime = Blockchain.GetHeight() + c.disputeTimeout;
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        c = _updateChannelStatus(c, channelStatus.Settling);
        IntendSettleEvent(_channelId, LedgerChannel._getStateSeqNums(c));
        return c;
    }

    public static LedgerStruct.Channel _updateChannelStatus(LedgerStruct.Channel _c, byte _newStatus)
    {
        if (_c.status == _newStatus)
        {
            return _c;
        }
        // update counter of old status
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(_newStatus >= 0 && _newStatus <= 4, "_newStatus illegal");
        if (_c.status != channelStatus.Uninitialized)
        {
            LedgerStruct.setChannelStatusNums(_c.status, LedgerStruct.getChannelStatusNums(_c.status) - 1);
        }
        // update counter of new status
        LedgerStruct.setChannelStatusNums(_newStatus, LedgerStruct.getChannelStatusNums(_newStatus) + 1);

        _c.status = _newStatus;
        return _c;
    }

    private static PbEntity.TokenInfo _validateTokenInfo(PbEntity.TokenInfo _token)
    {
        PbEntity.TokenType tokenType = PbEntity.getStandardTokenType();
        if (_token.tokenType == tokenType.NEO)
        {
            BasicMethods.assert(_token.address.Equals(LedgerStruct.NeoAddress), "token address is not Neo address");
        }
        else if (_token.tokenType == tokenType.GAS)
        {
            BasicMethods.assert(_token.address.Equals(LedgerStruct.GasAddress), "token address is not Gas address");
        }
        else if (_token.tokenType == tokenType.NEP5)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(_token.address), "token address is not NEP5 address");
        }
        else
        {
            BasicMethods.assert(false, "invalid token type");
        }
        return _token;
    }
}
