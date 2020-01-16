using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using System;
using System.ComponentModel;

public class LedgerMigrate : SmartContract
{
    public delegate object NEP5Contract(string method, object[] args);

    [DisplayName("MigrateChannelToEvent")]
    public static event Action<byte[], byte[]> MigrateChannelToEvent;

    [DisplayName("MigrateChannelFromEvent")]
    public static event Action<byte[], byte[]> MigrateChannelFromEvent;

    [DisplayName("migrateChannelToInner")]
    public static byte[] migrateChannelToInner(byte[] sender, LedgerStruct.Ledger _self, byte[][] pubKeys, byte[] _migrationRequest)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(sender), "sender illegal");
        PbChain.ChannelMigrationRequest migrationRequest =
            (PbChain.ChannelMigrationRequest)Neo.SmartContract.Framework.Helper.Deserialize(_migrationRequest);
        PbEntity.ChannelMigrationInfo migrationInfo =
            (PbEntity.ChannelMigrationInfo)Neo.SmartContract.Framework.Helper.Deserialize(migrationRequest.channelMigrationInfo);
        byte[] channelId = migrationInfo.channelId;
        BasicMethods.assert(BasicMethods._isByte32(channelId), "channelId illegal");
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(channelId);
        byte[] toLedgerAddr = migrationInfo.toLedgerAddress;

        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(
            c.status == channelStatus.Operable ||
            c.status == channelStatus.Settling, "status illegal"
        );
        byte[] h = Hash256(migrationRequest.channelMigrationInfo);
        // use Channel Library instead

        BasicMethods.assert(LedgerChannel._checkCoSignatures(c, h, pubKeys, migrationRequest.sigs), "Check co-sigs failed");
        BasicMethods.assert(migrationInfo.fromLedgerAddress.Equals(ExecutionEngine.ExecutingScriptHash), "From ledger address is not this");
        BasicMethods.assert(toLedgerAddr.Equals(sender), "To ledger address is not msg.sender");
        BasicMethods.assert(Blockchain.GetHeight() <= migrationInfo.migrationDeadline, "Passed migration deadline");

        c = LedgerOperation._updateChannelStatus(c, channelStatus.Migrated);
        c.migratedTo = toLedgerAddr;
        LedgerStruct.setChannelMap(channelId, c);
        MigrateChannelToEvent(channelId, toLedgerAddr);

        byte[] celerWallet = _self.celerWallet;

        NEP5Contract dyncall = (NEP5Contract)celerWallet.ToDelegate();
        Object[] args = new object[] { channelId, toLedgerAddr };
        dyncall("transferOperatorship", args);

        return channelId;
    }

    [DisplayName("migrateChannelFromInner")]
    public static bool migrateChannelFromInner(LedgerStruct.Ledger _self, byte[] _fromLedgerAddr, byte[] _migrationRequest)
    {
        BasicMethods.assert(BasicMethods._isLegalAddress(_fromLedgerAddr), "_fromLedgerAddr illegal");

        // TODO: latest version of openzeppelin Address.sol provide this api toPayable()
        byte[] fromLedgerAddrPayable = _fromLedgerAddr;

        NEP5Contract dyncall = (NEP5Contract)fromLedgerAddrPayable.ToDelegate();
        Object[] args = new object[] { _migrationRequest };
        byte[] channelId = (byte[])dyncall("migrateChannelTo", args);
        
        LedgerStruct.Channel c = LedgerStruct.getChannelMap(channelId);
        LedgerStruct.ChannelStatus channelStatus = LedgerStruct.getStandardChannelStatus();
        BasicMethods.assert(c.status == channelStatus.Uninitialized, "Immigrated channel already exists");

        byte[] celerWallet = _self.celerWallet;
        dyncall = (NEP5Contract)celerWallet.ToDelegate();
        args = new object[] { channelId };
        byte[] oper = (byte[])dyncall("getOperator", args);

        BasicMethods.assert(
            oper.Equals(ExecutionEngine.ExecutingScriptHash),
            "Operatorship not transferred"
        );

        c = LedgerOperation._updateChannelStatus(c, channelStatus.Operable);
        // Do not migrate WithdrawIntent, in other words, migration will implicitly veto
        // pending WithdrawIntent if any.
        c = LedgerChannel._importChannelMigrationArgs(c, fromLedgerAddrPayable, channelId);
        c = LedgerChannel._importPeersMigrationInfo(c, fromLedgerAddrPayable, channelId);
        LedgerStruct.setChannelMap(channelId, c);

        MigrateChannelFromEvent(channelId, _fromLedgerAddr);

        return true;
    }
}
