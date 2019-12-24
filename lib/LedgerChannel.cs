using Neo.SmartContract.Framework;
using System.Numerics;

public class LedgerChannel
{
    public delegate object DynamicCallContract(string method, object[] args);

    public static BigInteger getSettleFinalizedTimeInner(LedgerStruct.Channel _c)
    {
        return _c.settleFinalizedTime;
    }

    public static byte[] getTokenContractInner(LedgerStruct.Channel _c)
    {
        PbEntity.TokenInfo tokenInfo = _c.token;
        BasicMethods.assert(BasicMethods._isLegalAddress(tokenInfo.address), "address is illegal");
        return tokenInfo.address;
    }

    public static byte getTokenTypeInner(LedgerStruct.Channel _c)
    {
        PbEntity.TokenInfo tokenInfo = _c.token;
        return tokenInfo.tokenType;
    }

    public static byte getChannelStatusInner(LedgerStruct.Channel _c)
    {
        return _c.status;
    }

    public static BigInteger getCooperativeWithdrawSeqNumInner(LedgerStruct.Channel _c)
    {
        return _c.cooperativeWithdrawSeqNum;
    }

    public static BigInteger getTotalBalanceInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        return peer0.deposit + peer1.deposit - peer0.withdrawal - peer1.withdrawal;
    }

    public static LedgerStruct.BalanceMap getBalanceMapInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.BalanceMap balanceMap = new LedgerStruct.BalanceMap();
        balanceMap.peerAddrs = new byte[2][];
        balanceMap.deposits = new BigInteger[2];
        balanceMap.withdrawals = new BigInteger[2];
        balanceMap.peerAddrs[0] = peer0.peerAddr;
        balanceMap.peerAddrs[1] = peer1.peerAddr;
        balanceMap.deposits[0] = peer0.deposit;
        balanceMap.deposits[1] = peer1.deposit;
        balanceMap.withdrawals[0] = peer0.withdrawal;
        balanceMap.withdrawals[1] = peer1.withdrawal;
        return balanceMap;
    }

    public static LedgerStruct.ChannelMigrationArgs getChannelMigrationArgsInner(LedgerStruct.Channel _c)
    {
        LedgerStruct.ChannelMigrationArgs channelMigrationArgs = new LedgerStruct.ChannelMigrationArgs();
        channelMigrationArgs.disputeTimeout = _c.disputeTimeout;
        PbEntity.TokenInfo tokenInfo = _c.token;
        channelMigrationArgs.tokenType = tokenInfo.tokenType;
        channelMigrationArgs.tokenAddress = tokenInfo.address;
        channelMigrationArgs.cooperativeWithdrawSeqNum = _c.cooperativeWithdrawSeqNum;
        return channelMigrationArgs;
    }

    public static LedgerStruct.PeersMigrationInfo getPeersMigrationInfoInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;

        LedgerStruct.PeersMigrationInfo peersMigrationInfo = new LedgerStruct.PeersMigrationInfo();
        peersMigrationInfo.peerAddr = new byte[2][];
        peersMigrationInfo.deposit = new BigInteger[2];
        peersMigrationInfo.withdrawal = new BigInteger[2];
        peersMigrationInfo.seqNum = new BigInteger[2];
        peersMigrationInfo.transferOut = new BigInteger[2];
        peersMigrationInfo.pendingPayout = new BigInteger[2];

        peersMigrationInfo.peerAddr[0] = peer0.peerAddr;
        peersMigrationInfo.peerAddr[1] = peer1.peerAddr;
        peersMigrationInfo.deposit[0] = peer0.deposit;
        peersMigrationInfo.deposit[1] = peer1.deposit;
        peersMigrationInfo.withdrawal[0] = peer0.withdrawal;
        peersMigrationInfo.withdrawal[1] = peer1.withdrawal;
        peersMigrationInfo.seqNum[0] = peerState0.seqNum;
        peersMigrationInfo.seqNum[1] = peerState1.seqNum;
        peersMigrationInfo.transferOut[0] = peerState0.transferOut;
        peersMigrationInfo.transferOut[1] = peerState1.transferOut;
        peersMigrationInfo.pendingPayout[0] = peerState0.pendingPayOut;
        peersMigrationInfo.pendingPayout[1] = peerState1.pendingPayOut;

        return peersMigrationInfo;
    }

    public static BigInteger getDisputeTimeoutInner(LedgerStruct.Channel _c)
    {
        return _c.disputeTimeout;
    }

    public static byte[] getMigratedToInner(LedgerStruct.Channel _c)
    {
        return _c.migratedTo;
    }

    public static LedgerStruct.StateSeqNumMap getStateSeqNumMapInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;

        LedgerStruct.StateSeqNumMap stateSeqNumMap = new LedgerStruct.StateSeqNumMap();
        stateSeqNumMap.peerAddr = new byte[2][];
        stateSeqNumMap.seqNum = new BigInteger[2];
        stateSeqNumMap.peerAddr[0] = peer0.peerAddr;
        stateSeqNumMap.peerAddr[1] = peer1.peerAddr;
        stateSeqNumMap.seqNum[0] = peerState0.seqNum;
        stateSeqNumMap.seqNum[1] = peerState1.seqNum;
        return stateSeqNumMap;
    }

    public static LedgerStruct.TransferOutMap getTransferOutMapInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;

        LedgerStruct.TransferOutMap transferOutMap = new LedgerStruct.TransferOutMap();
        transferOutMap.peerAddr = new byte[2][];
        transferOutMap.transferOut = new BigInteger[2];
        transferOutMap.peerAddr[0] = peer0.peerAddr;
        transferOutMap.peerAddr[1] = peer1.peerAddr;
        transferOutMap.transferOut[0] = peerState0.transferOut;
        transferOutMap.transferOut[1] = peerState1.transferOut;
        return transferOutMap;
    }

    public static LedgerStruct.NextPayIdListHashMap getNextPayIdListHashMapInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;

        LedgerStruct.NextPayIdListHashMap nextPayIdListHashMap = new LedgerStruct.NextPayIdListHashMap();
        nextPayIdListHashMap.peerAddr = new byte[2][];
        nextPayIdListHashMap.nextPayIdListHash = new byte[2][];
        nextPayIdListHashMap.peerAddr[0] = peer0.peerAddr;
        nextPayIdListHashMap.peerAddr[1] = peer1.peerAddr;
        nextPayIdListHashMap.nextPayIdListHash[0] = peerState0.nextPayIdListHash;
        nextPayIdListHashMap.nextPayIdListHash[1] = peerState1.nextPayIdListHash;
        return nextPayIdListHashMap;
    }

    public static LedgerStruct.LastPayResolveDeadlineMap getLastPayResolveDeadlineMapInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;

        LedgerStruct.LastPayResolveDeadlineMap lastPayResolveDeadlineMap = new LedgerStruct.LastPayResolveDeadlineMap();
        lastPayResolveDeadlineMap.peerAddr = new byte[2][];
        lastPayResolveDeadlineMap.lastPayResolveDeadline = new BigInteger[2];
        lastPayResolveDeadlineMap.peerAddr[0] = peer0.peerAddr;
        lastPayResolveDeadlineMap.peerAddr[1] = peer1.peerAddr;
        lastPayResolveDeadlineMap.lastPayResolveDeadline[0] = peerState0.lastPayResolveDeadline;
        lastPayResolveDeadlineMap.lastPayResolveDeadline[1] = peerState1.lastPayResolveDeadline;
        return lastPayResolveDeadlineMap;
    }

    public static LedgerStruct.PendingPayOutMap getPendingPayOutMapInner(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;

        LedgerStruct.PendingPayOutMap pendingPayOutMap = new LedgerStruct.PendingPayOutMap();
        pendingPayOutMap.peerAddr = new byte[2][];
        pendingPayOutMap.pendingPayOut = new BigInteger[2];
        pendingPayOutMap.peerAddr[0] = peer0.peerAddr;
        pendingPayOutMap.peerAddr[1] = peer1.peerAddr;
        pendingPayOutMap.pendingPayOut[0] = peerState0.pendingPayOut;
        pendingPayOutMap.pendingPayOut[1] = peerState1.pendingPayOut;
        return pendingPayOutMap;
    }

    public static LedgerStruct.WithdrawIntent getWithdrawIntentInner(LedgerStruct.Channel _c)
    {
        return _c.withdrawIntent;
    }

    public static LedgerStruct.Channel _importChannelMigrationArgs(LedgerStruct.Channel _c, byte[] _fromLedgerAddr, byte[] _channelId)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_fromLedgerAddr), "invalid contract address");
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "invalid _channelId");

        DynamicCallContract dyncall = (DynamicCallContract)_fromLedgerAddr.ToDelegate();
        LedgerStruct.ChannelMigrationArgs args = (LedgerStruct.ChannelMigrationArgs)dyncall("getChannelMigrationArgs", new object[] { _channelId });
        _c.disputeTimeout = args.disputeTimeout;
        PbEntity.TokenInfo token = new PbEntity.TokenInfo();
        token.tokenType = args.tokenType;
        token.address = args.tokenAddress;
        _c.token = token;
        _c.cooperativeWithdrawSeqNum = args.cooperativeWithdrawSeqNum;
        return _c;
    }

    public static LedgerStruct.Channel _importPeersMigrationInfo(LedgerStruct.Channel _c, byte[] _fromLedgerAddr, byte[] _channelId)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_fromLedgerAddr), "invalid contract address");
        BasicMethods.assert(BasicMethods._isByte32(_channelId), "invalid _channelId");

        DynamicCallContract dyncall = (DynamicCallContract)_fromLedgerAddr.ToDelegate();
        LedgerStruct.PeersMigrationInfo args = (LedgerStruct.PeersMigrationInfo)dyncall("getPeersMigrationInfo", new object[] { _channelId });
        for (int i = 0; i < 2; i++)
        {
            LedgerStruct.PeerProfile peerProfile = new LedgerStruct.PeerProfile();
            peerProfile.peerAddr = args.peerAddr[i];
            peerProfile.deposit = args.deposit[i];
            peerProfile.withdrawal = args.withdrawal[i];
            LedgerStruct.PeerState peerState = peerProfile.state;
            peerState.seqNum = args.seqNum[i];
            peerState.transferOut = args.transferOut[i];
            peerState.pendingPayOut = args.pendingPayout[i];
            peerProfile.state = peerState;
            _c.peerProfiles[i] = peerProfile;
        }
        return _c;
    }

    public static BigInteger[] _getStateSeqNums(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;
        return new BigInteger[] { peerState0.seqNum, peerState1.seqNum };
    }

    public static bool _isPeer(LedgerStruct.Channel _c, byte[] _addr)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_addr), "_fromLedgerAddr parameter error");
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        return _addr.Equals(peer0.peerAddr) || _addr.Equals(peer1.peerAddr);
    }

    public static byte _getPeerId(LedgerStruct.Channel _c, byte[] _peer)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_peer), "_fromLedgerAddr parameter error");
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        if (_peer.Equals(peer0.peerAddr))
        {
            return 0;
        }
        else if (_peer.Equals(peer1.peerAddr))
        {
            return 1;
        }
        else
        {
            BasicMethods.assert(false, "Nonexist peer");
        }
        return 2;
    }

    public static bool _checkSingleSignature(LedgerStruct.Channel _c, byte[] _h, byte[] _sig)
    {
        BasicMethods.assert(BasicMethods._isByte32(_h), "_fromLedgerAddr parameter error");
        //Pending
        return true;
    }

    public static bool _checkCoSignatures(LedgerStruct.Channel _c, byte[] _h, byte[][] _sig)
    {
        BasicMethods.assert(BasicMethods._isByte32(_h), "_fromLedgerAddr parameter error");
        BasicMethods.assert(_sig.Length == 2, "_fromLedgerAddr parameter error");
        //Pending
        return true;
    }

    public static LedgerStruct.SettleBalance _validateSettleBalance(LedgerStruct.Channel _c)
    {
        BasicMethods.assert(_c.peerProfiles.Length == 2, "Illegal peerProfiles length");
        LedgerStruct.PeerProfile peer0 = _c.peerProfiles[0];
        LedgerStruct.PeerProfile peer1 = _c.peerProfiles[1];
        LedgerStruct.PeerState peerState0 = peer0.state;
        LedgerStruct.PeerState peerState1 = peer1.state;
        LedgerStruct.SettleBalance settleBalance = new LedgerStruct.SettleBalance();
        settleBalance.balance = new BigInteger[2];
        settleBalance.balance[0] = peer0.deposit + peerState1.transferOut;
        settleBalance.balance[1] = peer1.deposit + peerState0.transferOut;

        BigInteger subAmt = peerState0.transferOut + peer0.withdrawal;
        if (settleBalance.balance[0] < subAmt)
        {
            settleBalance.isSettled = 0;
            settleBalance.balance[0] = 0;
            settleBalance.balance[1] = 0;
            return settleBalance;
        }
        settleBalance.balance[0] = settleBalance.balance[0] - subAmt;

        subAmt = peerState1.transferOut + peer1.withdrawal;
        if (settleBalance.balance[1] < subAmt)
        {
            settleBalance.isSettled = 0;
            settleBalance.balance[0] = 0;
            settleBalance.balance[1] = 0;
            return settleBalance;
        }
        settleBalance.balance[1] = settleBalance.balance[1] - subAmt;

        settleBalance.isSettled = 1;
        return settleBalance;
    }

    public static LedgerStruct.Channel _addWithdrawal(LedgerStruct.Channel _c, byte[] _receiver, BigInteger _amount)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_receiver), "_receiver is illegal");
        BasicMethods.assert(_amount >= 0, "_amount is negative");
        byte rid = _getPeerId(_c, _receiver);
        LedgerStruct.PeerProfile peerProfile = _c.peerProfiles[rid];
        peerProfile.withdrawal = peerProfile.withdrawal + _amount;
        BasicMethods.assert(getTotalBalanceInner(_c) >= 0, "Total balance is negative");
        return _c;
    }
}