using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.Numerics;
using System.ComponentModel;
using Helper = Neo.SmartContract.Framework.Helper;
using Neo.SmartContract.Framework.Services.System;

namespace CelerWallet
{
    public class CelerWallet : SmartContract
    {
        public struct Wallet
        {
            public byte[][] owners;
            public byte[] theOperator;
            //public Map<byte[], BigInteger> balances;
            public byte[] proposedNewOperator;
            //public Map<byte[], bool> proposalVotes;
        }

        public struct MathOperation
        {
            public byte add;
            public byte sub;
        }

        public static MathOperation getStandardMathOperation()
        {
            return new MathOperation
            {
                add = 0,
                sub = 1
            };
        }

        //private static bool _paused;
        public static readonly byte[] PausedKey = "paused".AsByteArray();
        //public static Map<byte[], bool> _pausers;
        public static readonly byte[] PauserKey = "pausers".AsByteArray();
        public static readonly byte[] WalletNum = "walletNum".AsByteArray();
        //public static Map<byte[], Wallet> wallets;
        public static readonly byte[] WalletsPrefix = "wallets".AsByteArray();
        public static readonly byte[] WalletsBalancesPrefix = "balances".AsByteArray();
        public static readonly byte[] WalletsProposalVotesPrefix = "proposalVotes".AsByteArray();

        public delegate object NEP5Contract(string method, object[] args);

        [DisplayName("pause")]
        public static event Action<byte[]> Paused;
        [DisplayName("unpause")]
        public static event Action<byte[]> UnPaused;
        [DisplayName("addPauser")]
        public static event Action<byte[]> PauserAdded;
        [DisplayName("removePauser")]
        public static event Action<byte[]> PauserRemoveded;
        [DisplayName("create")]
        public static event Action<byte[], byte[][], byte[]> CreateWallet;
        [DisplayName("deposit")]
        public static event Action<byte[], byte[], BigInteger> DepositToWallet;
        [DisplayName("withdraw")]
        public static event Action<byte[], byte[], byte[], BigInteger> WithdrawFromWallet;
        [DisplayName("transferToWallet")]
        public static event Action<byte[], byte[], byte[], byte[], BigInteger> TransferToWallet;
        [DisplayName("changeOperator")]
        public static event Action<byte[], byte[], byte[]> ChangeOperator;
        [DisplayName("proposeNewOperator")]
        public static event Action<byte[], byte[], byte[]> ProposeNewOperator;
        [DisplayName("drainToken")]
        public static event Action<byte[], byte[], BigInteger> DrainToken;

        public static object Main(string method, object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)//取钱才会涉及这里
            {
                Transaction tx = ExecutionEngine.ScriptContainer as Transaction;
                TransactionAttribute[] attributes = tx.GetAttributes();
                byte[] walletid = null;
                foreach (TransactionAttribute attribute in attributes)
                {
                    if (attribute.Usage == 0xff)//Remark15, used to pass walletid
                    {
                        walletid = attribute.Data;
                        break;
                    }
                }
                if (walletid == null) return false;

                BigInteger[] tokenValues = getTokenWithdraw();

                if (tokenValues[0] > getBalance(walletid, LedgerStruct.NeoAddress)) return false;
                if (tokenValues[1] > getBalance(walletid, LedgerStruct.GasAddress)) return false;
                return true;
            }
            else if (Runtime.Trigger == TriggerType.VerificationR)
            {
                return true;
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (method == "pause")
                {
                    BasicMethods.assert(args.Length == 1, "params length error");
                    byte[] invoker = (byte[])args[0];
                    return pause(invoker);
                }
                if (method == "unpause")
                {
                    BasicMethods.assert(args.Length == 1, "params length error");
                    byte[] invoker = (byte[])args[0];
                    return unpause(invoker);
                }
                if (method == "addPauser")
                {
                    BasicMethods.assert(args.Length == 2, "params length error");
                    byte[] invoker = (byte[])args[0];
                    byte[] account = (byte[])args[1];
                    return addPauser(invoker, account);
                }
                if (method == "removePauser")
                {
                    BasicMethods.assert(args.Length == 2, "params length error");
                    byte[] invoker = (byte[])args[0];
                    byte[] account = (byte[])args[1];
                    return removePauser(invoker, account);
                }
                if (method == "create")
                {
                    BasicMethods.assert(args.Length == 3, "params length error");
                    byte[] invoker = ExecutionEngine.CallingScriptHash;
                    byte[][] owners = (byte[][])args[0];
                    byte[] theOperator = (byte[])args[1];
                    byte[] nonce = (byte[])args[2];
                    return create(invoker, owners, theOperator, nonce);
                }
                if (method == "depositneo")
                {
                    BasicMethods.assert(args.Length == 1, "params length error");
                    byte[] walletId = (byte[])args[0];
                    return depositNEO(walletId);
                }
                if (method == "depositgas")
                {
                    BasicMethods.assert(args.Length == 1, "params length error");
                    byte[] walletId = (byte[])args[0];
                    return depositGAS(walletId);
                }
                /*if (method == "depositNEP5")
                {
                    BasicMethods.assert(args.Length == 4, "params length error");
                    byte[] invoker = (byte[])args[0];
                    byte[] walletId = (byte[])args[1];
                    byte[] tokenAddress = (byte[])args[2];
                    BigInteger amount = (BigInteger)args[3];
                    return depositNEP5(invoker, walletId, tokenAddress, amount);
                }*/
                if (method == "withdraw")
                {
                    BasicMethods.assert(args.Length == 4, "params length error");
                    byte[] invoker = (byte[])args[0];
                    byte[] tokenAddress = (byte[])args[1];
                    byte[] receiver = (byte[])args[2];
                    BigInteger amount = (BigInteger)args[3];
                    return withdraw(invoker, tokenAddress, receiver, amount, ExecutionEngine.CallingScriptHash);
                }
                if (method == "transferToWallet")
                {
                    BasicMethods.assert(args.Length == 5, "params length error");
                    byte[] fromWalletId = (byte[])args[0];
                    byte[] toWalletId = (byte[])args[1];
                    byte[] tokenAddress = (byte[])args[2];
                    byte[] receiver = (byte[])args[3];
                    BigInteger amount = (BigInteger)args[4];
                    return transferToWallet(fromWalletId, toWalletId, tokenAddress, receiver, amount, ExecutionEngine.CallingScriptHash);
                }
                if (method == "transferOperatorship")
                {
                    BasicMethods.assert(args.Length == 2, "params length error");
                    byte[] walletId = (byte[])args[0];
                    byte[] newOperator = (byte[])args[1];
                    return transferOperatorship(walletId, newOperator, ExecutionEngine.CallingScriptHash);
                }
                if (method == "proposeNewOperator")
                {
                    BasicMethods.assert(args.Length == 3, "params length error");
                    byte[] invoker = (byte[])args[0];
                    byte[] walletId = (byte[])args[1];
                    byte[] newOperator = (byte[])args[2];
                    return proposeNewOperator(invoker, walletId, newOperator);
                }
                if (method == "drainToken")
                {
                    BasicMethods.assert(args.Length == 4, "params length error");
                    byte[] invoker = (byte[])args[0];
                    byte[] tokenAddress = (byte[])args[1];
                    byte[] receiver = (byte[])args[2];
                    BigInteger amount = (BigInteger)args[3];
                    return drainToken(invoker, tokenAddress, receiver, amount);
                }
                if (method == "getWalletOwners")
                {
                    BasicMethods.assert(args.Length == 1, "params length error");
                    byte[] walletId = (byte[])args[0];
                    return getWalletOwners(walletId);
                }
                if (method == "getOperator")
                {
                    BasicMethods.assert(args.Length == 1, "params length error");
                    byte[] walletId = (byte[])args[0];
                    return getOperator(walletId);
                }
                if (method == "getBalance")
                {
                    BasicMethods.assert(args.Length == 2, "params length error");
                    byte[] walletId = (byte[])args[0];
                    byte[] tokenAddress = (byte[])args[1];
                    return getBalance(walletId, tokenAddress);
                }
                if (method == "getProposalVote")
                {
                    BasicMethods.assert(args.Length == 2, "params length error");
                    byte[] walletId = (byte[])args[0];
                    byte[] owner = (byte[])args[1];
                    return getProposalVote(walletId, owner);
                }
            }
            return false;
        }

        public static Wallet getWallet(byte[] walletId)
        {
            byte[] walletBs = Storage.Get(Storage.CurrentContext, WalletsPrefix.Concat(walletId));
            if (walletBs.Length == 0)
                return new Wallet();
            return (Wallet)Helper.Deserialize(walletBs);
        }

        public static void _onlyOperator(byte[] _walletId, byte[] callingScriptHash)
        {
            //BasicMethods.assert(Runtime.CheckWitness(getWallet(_walletId).theOperator), "operator checkwitness failed");
            BasicMethods.assert(getWallet(_walletId).theOperator.Equals(callingScriptHash), "operator checkwitness failed");
        }

        public static void _onlyWalletOwner(byte[] _walletId, byte[] _addr)
        {
            BasicMethods.assert(_isWalletOwner(_walletId, _addr), "Given address is not wallet owner");
        }

        [DisplayName("create")]
        public static byte[] create(byte[] invoker, byte[][] owners, byte[] theOperator, byte[] nonce)
        {
            BasicMethods.assert(BasicMethods._isLegalAddresses(owners), "owners addresses are not byte20");
            BasicMethods.assert(BasicMethods._isLegalAddress(theOperator), "the operator is not byte20");
            //TODO: no need to check the nonce byte[] length

            _whenNotPaused();

            BasicMethods.assert(BasicMethods._isLegalAddress(theOperator), "New operator is address zero");
            byte[] SelfContractHash = ExecutionEngine.ExecutingScriptHash;
            byte[] concatRes = SelfContractHash.Concat(invoker).Concat(nonce);
            byte[] walletId = SmartContract.Sha256(concatRes);
            BasicMethods.assert(getWallet(walletId).theOperator == null, "Occupied wallet id");
            Wallet w = new Wallet();
            BasicMethods.assert(BasicMethods._isLegalAddresses(owners), "owners contains illegal address");
            w.owners = owners;
            w.theOperator = theOperator;
            Storage.Put(Storage.CurrentContext, WalletsPrefix.Concat(walletId), Helper.Serialize(w));
            BigInteger walletNum = Storage.Get(Storage.CurrentContext, WalletNum).AsBigInteger();
            Storage.Put(Storage.CurrentContext, WalletNum, (walletNum + 1).AsByteArray());
            CreateWallet(walletId, owners, theOperator);
            return walletId;
        }

        [DisplayName("depositNEO")]
        public static object depositNEO(byte[] walletId)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId is not byte32");
            PbEntity.TokenType token = PbEntity.getStandardTokenType();
            LedgerStruct.TransactionValue transactionValue = LedgerStruct.getTransactionValue(token.NEO);
            BasicMethods.assert(transactionValue.value >= 0, "amount is less than zero");
            BasicMethods.assert(transactionValue.receiver.Equals(ExecutionEngine.ExecutingScriptHash) , "token receiver is not current smart contract");

            _whenNotPaused();
            BasicMethods.assert(_updateBalance(walletId, LedgerStruct.NeoAddress, transactionValue.value, getStandardMathOperation().add), "updateBalance failed");

            DepositToWallet(walletId, LedgerStruct.NeoAddress, transactionValue.value);
            return true;
        }

        [DisplayName("depositGAS")]
        public static object depositGAS(byte[] walletId)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId is not byte32");
            PbEntity.TokenType token = PbEntity.getStandardTokenType();
            LedgerStruct.TransactionValue transactionValue = LedgerStruct.getTransactionValue(token.GAS);
            BasicMethods.assert(transactionValue.value >= 0, "amount is less than zero");
            BasicMethods.assert(transactionValue.receiver.Equals(ExecutionEngine.ExecutingScriptHash), "token receiver is not current smart contract");

            _whenNotPaused();

            BasicMethods.assert(_updateBalance(walletId, LedgerStruct.GasAddress, transactionValue.value, getStandardMathOperation().add), "updateBalance failed");

            DepositToWallet(walletId, LedgerStruct.GasAddress, transactionValue.value);
            return true;
        }

        /*[DisplayName("depositNEP5")]
        public static object depositNEP5(byte[] invoker, byte[] walletId, byte[] tokenAddress, BigInteger amount)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "CheckWitness failed");
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId is not byte32");
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddress), "tokenAddress is not byte20");
            BasicMethods.assert(amount >= 0, "amount is less than zero");

            _whenNotPaused();

            BasicMethods.assert(_updateBalance(walletId, tokenAddress, amount, mathOperation.add), "updateBalance failed");
            NEP5Contract dyncall = (NEP5Contract)tokenAddress.ToDelegate();
            Object[] args = new object[] { invoker, ExecutionEngine.ExecutingScriptHash, amount };
            bool res = (bool)dyncall("transfer", args);
            BasicMethods.assert(res, "transfer NEP5 tokens failed");

            DepositToWallet(walletId, tokenAddress, amount);
            return true;
        }*/

        [DisplayName("withdraw")]
        //需要重写
        public static object withdraw(byte[] walletId, byte[] tokenAddress, byte[] receiver, BigInteger amount, byte[] callingScriptHash)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal, not byte32");
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddress), "tokenAddress is illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(receiver), "receiver address is illegal");
            BasicMethods.assert(amount >= 0, "amount is less than zero");
            BasicMethods.assert(Runtime.CheckWitness(receiver), "CheckWitness failed");
            BigInteger[] tokenValues = getTokenWithdraw();
            if (tokenAddress.Equals(LedgerStruct.NeoAddress))
            {
                BasicMethods.assert(amount >= tokenValues[0], "amount is less than zero");
            }
            else if (tokenAddress.Equals(LedgerStruct.NeoAddress))
            {
                BasicMethods.assert(amount >= tokenValues[1], "amount is less than zero");
            }

            _whenNotPaused();
            _onlyOperator(walletId, callingScriptHash);
            _onlyWalletOwner(walletId, receiver);

            BasicMethods.assert(_updateBalance(walletId, tokenAddress, amount, getStandardMathOperation().sub), "updateBalance failed");
            BasicMethods.assert(_withdrawToken(tokenAddress, receiver, amount), "withdrawToken failed");
            WithdrawFromWallet(walletId, tokenAddress, receiver, amount);
            return true;
        }

        [DisplayName("transferToWallet")]
        public static object transferToWallet(byte[] fromWalletId, byte[] toWalletId, byte[] tokenAddress, byte[] receiver, BigInteger amount, byte[] callingScriptHash)
        {
            BasicMethods.assert(BasicMethods._isByte32(fromWalletId), "fromWalletId illegal, not byte32");
            BasicMethods.assert(BasicMethods._isByte32(toWalletId), "toWalletId illegal, not byte32");
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddress), "tokenAddress is illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(receiver), "receiver address is illegal");
            BasicMethods.assert(amount >= 0, "amount is less than zero");
            BasicMethods.assert(Runtime.CheckWitness(receiver), "CheckWitness failed");

            _whenNotPaused();
            _onlyOperator(fromWalletId, callingScriptHash);
            _onlyWalletOwner(fromWalletId, receiver);
            _onlyWalletOwner(toWalletId, receiver);

            BasicMethods.assert(_updateBalance(fromWalletId, tokenAddress, amount, getStandardMathOperation().sub), "sub balance in from wallet failed");
            BasicMethods.assert(_updateBalance(toWalletId, tokenAddress, amount, getStandardMathOperation().add), "add balance in to wallet failed");

            TransferToWallet(fromWalletId, toWalletId, tokenAddress, receiver, amount);
            return true;
        }

        [DisplayName("transferOperatorship")]
        public static object transferOperatorship(byte[] walletId, byte[] newOperator, byte[] callingScriptHash)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal, not byte32");
            BasicMethods.assert(BasicMethods._isLegalAddress(newOperator), "newOperator address is illegal");
            // no need to checkwitness since _onlyOperator has already done it

            _whenNotPaused();
            _onlyOperator(walletId, callingScriptHash);
            _changeOperator(walletId, newOperator);
            return true;
        }

        [DisplayName("proposeNewOperator")]
        public static object proposeNewOperator(byte[] invoker, byte[] walletId, byte[] newOperator)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "CheckWitness failed");
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(newOperator), "new operator is not address");

            _onlyWalletOwner(walletId, invoker);

            // wpvBs means Wallet Proposal Votes ByteS
            byte[] wpvBs = Storage.Get(Storage.CurrentContext, WalletsProposalVotesPrefix.Concat(walletId));
            Map<byte[], bool> wpv = new Map<byte[], bool>();
            if (wpvBs.Length > 0)
            {
                wpv = Helper.Deserialize(wpvBs) as Map<byte[], bool>;
            }
            Wallet w = getWallet(walletId);
            if (newOperator != w.proposedNewOperator)
            {
                wpv = _clearVotes(w, wpv);
                w.proposedNewOperator = newOperator;
            }
            Storage.Put(Storage.CurrentContext, WalletsPrefix.Concat(walletId), Helper.Serialize(w));
            wpv[invoker] = true;

            if (_checkAllVotes(w, wpv))
            {
                _changeOperator(walletId, newOperator);
                wpv = _clearVotes(w, wpv);
            }
            
            Storage.Put(Storage.CurrentContext, WalletsProposalVotesPrefix.Concat(walletId), Helper.Serialize(wpv));

            ProposeNewOperator(walletId, newOperator, invoker);
            return true;
        }

        [DisplayName("drainToken")]
        public static object drainToken(byte[] invoker, byte[] tokenAddress, byte[] receiver, BigInteger amount)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "CheckWitness failed");
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddress), "tokenAddress is illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(receiver), "receiver address is illegal");
            BasicMethods.assert(amount >= 0, "amount is less than zero");

            _whenPaused();
            _onlyPauser(invoker);

            BigInteger[] tokenValues = getTokenWithdraw();
            if (tokenAddress.Equals(LedgerStruct.NeoAddress))
            {
                BasicMethods.assert(amount >= tokenValues[0], "amount is less than zero");
            }
            else if (tokenAddress.Equals(LedgerStruct.NeoAddress))
            {
                BasicMethods.assert(amount >= tokenValues[1], "amount is less than zero");
            }

            BasicMethods.assert(_withdrawToken(tokenAddress, receiver, amount), "withdrawToken failed");

            DrainToken(tokenAddress, receiver, amount);
            return true;
        }

        [DisplayName("getWalletOwners")]
        public static byte[][] getWalletOwners(byte[] walletId)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal");
            Wallet w = getWallet(walletId);
            return w.owners;
        }

        [DisplayName("getOperator")]
        public static byte[] getOperator(byte[] walletId)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal");
            Wallet w = getWallet(walletId);
            return w.theOperator;
        }

        [DisplayName("getBalance")]
        public static BigInteger getBalance(byte[] walletId, byte[] tokenAddress)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(tokenAddress), "tokenAddress is illegal");
            Map<byte[], BigInteger> wBalanceMap = new Map<byte[], BigInteger>();
            byte[] wBalanceBs = Storage.Get(Storage.CurrentContext, WalletsBalancesPrefix.Concat(walletId));
            if (wBalanceBs.Length > 0)
            {
                wBalanceMap = Helper.Deserialize(wBalanceBs) as Map<byte[], BigInteger>;
            }
            if (wBalanceMap.HasKey(tokenAddress))
            {
                return wBalanceMap[tokenAddress];
            }
            return 0;
        }

        [DisplayName("getProposedNewOperator")]
        public static byte[] getProposedNewOperator(byte[] walletId)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal");
            Wallet w = getWallet(walletId);
            return w.proposedNewOperator;
        }

        [DisplayName("getProposalVote")]
        public static bool getProposalVote(byte[] walletId, byte[] owner)
        {
            BasicMethods.assert(BasicMethods._isByte32(walletId), "walletId illegal");
            BasicMethods.assert(BasicMethods._isLegalAddress(owner), "owner address is not length of 20 bytes");
            _onlyWalletOwner(walletId, owner);
            // wpvBs means Wallet Proposal Votes ByteS
            byte[] wpvBs = Storage.Get(Storage.CurrentContext, WalletsProposalVotesPrefix.Concat(walletId));
            Map<byte[], bool> wpv = new Map<byte[], bool>();
            if (wpvBs.Length > 0)
            {
                wpv = Helper.Deserialize(wpvBs) as Map<byte[], bool>;
                return wpv[owner];
            }
            return false;
        }

        private static bool _checkAllVotes(Wallet _w, Map<byte[], bool> _wpv)
        {
            for (var i = 0; i < _w.owners.Length; i++)
            {
                if (_wpv[_w.owners[i]] == false)
                    return false;
            }
            return true;
        }

        private static Map<byte[], bool> _clearVotes(Wallet _w, Map<byte[], bool> _wpv)
        {
            for (var i = 0; i < _w.owners.Length; i++)
            {
                _wpv[_w.owners[i]] = false;
            }

            return _wpv;
        }

        private static void _changeOperator(byte[] _walletId, byte[] _newOperator)
        {
            BasicMethods.assert(BasicMethods._isLegalAddress(_newOperator), "new operator is illegal");
            Wallet w = getWallet(_walletId);
            byte[] oldOperator = w.theOperator;
            BasicMethods.assert(BasicMethods._isLegalAddress(oldOperator), "old operator is not legal");

            w.theOperator = _newOperator;

            Storage.Put(Storage.CurrentContext, WalletsPrefix.Concat(_walletId), Helper.Serialize(w));
            ChangeOperator(_walletId, oldOperator, _newOperator);
        }

        private static bool _withdrawToken(byte[] _tokenAddress, byte[] _receiver, BigInteger _amount)
        {
            if (_tokenAddress.Equals(LedgerStruct.NeoAddress) || _tokenAddress.Equals(LedgerStruct.GasAddress))
            {
                return true;
            }
            NEP5Contract dyncall = (NEP5Contract)_tokenAddress.ToDelegate();
            bool res = (bool)dyncall("transfer", new object[] { ExecutionEngine.ExecutingScriptHash, _receiver, _amount });
            return res;
        }

        private static bool _updateBalance(byte[] _walletId, byte[] _tokenAddress, BigInteger _amount, byte _mathOperation)
        {
            BasicMethods.assert(_amount >= 0, "amount is less than zero");
            Wallet w = getWallet(_walletId);
            BasicMethods.assert(BasicMethods._isLegalAddress(w.theOperator), "wallet Object does not exist");
            byte[] wBalanceBs = Storage.Get(Storage.CurrentContext, WalletsBalancesPrefix.Concat(_walletId));
            Map<byte[], BigInteger> wBalanceMap = new Map<byte[], BigInteger>();
            if (wBalanceBs.Length > 0)
            {
                wBalanceMap = Helper.Deserialize(wBalanceBs) as Map<byte[], BigInteger>;
            }
            if (!wBalanceMap.HasKey(_tokenAddress))
            {
                wBalanceMap[_tokenAddress] = 0;
            }
            if (_mathOperation == getStandardMathOperation().add)
            {
                wBalanceMap[_tokenAddress] += _amount;
            }
            else if (_mathOperation == getStandardMathOperation().sub)
            {
                wBalanceMap[_tokenAddress] -= _amount;
                BasicMethods.assert(wBalanceMap[_tokenAddress] >= 0, "balance is less than zero");
            }
            else
            {
                BasicMethods.assert(false, "math operation illegal");
            }
            Storage.Put(Storage.CurrentContext, WalletsBalancesPrefix.Concat(_walletId), Helper.Serialize(wBalanceMap));
            return true;
        }

        private static bool _isWalletOwner(byte[] _walletId, byte[] _addr)
        {
            Wallet w = getWallet(_walletId);
            for (var i = 0; i < w.owners.Length; i++)
            {
                if (_addr.Equals(w.owners[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private static BigInteger[] getTokenWithdraw ()
        {
            Transaction tx = ExecutionEngine.ScriptContainer as Transaction;
            TransactionInput[] inputs = tx.GetInputs();
            BigInteger neoAmount = 0;
            BigInteger gasAmount = 0;
            foreach (var input in inputs)
            {
                TransactionOutput output = Blockchain.GetTransaction(input.PrevHash).GetOutputs()[input.PrevIndex];
                byte[] scriptHash = output.ScriptHash;
                if (!scriptHash.Equals(ExecutionEngine.ExecutingScriptHash)) continue;
                byte[] assetid = output.AssetId;
                BigInteger value = output.Value;
                if (assetid.Equals(LedgerStruct.NeoID))
                {
                    neoAmount += value;
                }
                else if (assetid.Equals(LedgerStruct.GasID))
                {
                    gasAmount += value;
                }
            }

            return new BigInteger[] { neoAmount, gasAmount };
        }

        private static void _onlyPauser(byte[] invoker)
        {
            BasicMethods.assert(isPauser(invoker), "the invoker is not one effective pauser");
        }

        public static bool isPauser(byte[] account)
        {
            byte[] pauserBs = Storage.Get(Storage.CurrentContext, PauserKey);
            if (pauserBs.Length > 0)
            {
                Map<byte[], bool> pausers = Helper.Deserialize(pauserBs) as Map<byte[], bool>;
                if (pausers.HasKey(account))
                {
                    return pausers[account];
                }
                return false;
            }
            return false;
        }

        [DisplayName("addPauser")]
        public static object addPauser(byte[] invoker, byte[] account)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "Checkwitness failed");
            // make sure the invoker is one effective pauser
            
            Map<byte[], bool> pausers = new Map<byte[], bool>();
            byte[] pauserBs = Storage.Get(Storage.CurrentContext, PauserKey);
            if (pauserBs.Length > 0) pausers = Helper.Deserialize(pauserBs) as Map<byte[], bool>;
            byte[] admin = BasicMethods.getAdmin();
            if (!invoker.Equals(admin))
            {
                BasicMethods.assert(pausers.HasKey(invoker), "invoker is not one pauser");
                BasicMethods.assert(pausers[invoker], "invoker is an effective pauser");
            }

            // update the pausers map
            pausers[account] = true;
            Storage.Put(Storage.CurrentContext, PauserKey, Helper.Serialize(pausers));
            PauserAdded(account);
            return true;
        }

        [DisplayName("removePauser")]
        public static object removePauser(byte[] invoker, byte[] account)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "Checkwitness failed");

            // make sure the invoker is one effective pauser
            byte[] pauserBs = Storage.Get(Storage.CurrentContext, PauserKey);
            BasicMethods.assert(pauserBs.Length > 0, "pauser map is empty");
            Map<byte[], bool> pausers = Helper.Deserialize(pauserBs) as Map<byte[], bool>;
            BasicMethods.assert(pausers.HasKey(invoker), "invoker is not one pauser");
            BasicMethods.assert(pausers[invoker], "invoker is an effective pauser");

            // update the pausers map
            pausers[account] = false;
            Storage.Put(Storage.CurrentContext, PauserKey, Helper.Serialize(pausers));
            PauserRemoveded(account);
            return true;
        }
        
        public static bool getPausedStatus()
        {
            return Storage.Get(Storage.CurrentContext, PausedKey).Equals(new byte[] { 0x01 });
        }

        public static void _whenNotPaused()
        {
            BasicMethods.assert(!getPausedStatus(), "Pausable: paused");
        }

        public static void _whenPaused()
        {
            BasicMethods.assert(getPausedStatus(), "Pausable: unpaused");
        }

        [DisplayName("pause")]
        public static object pause(byte[] invoker)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "CheckWitness failed");
            _onlyPauser(invoker);
            _whenNotPaused();
            Storage.Put(Storage.CurrentContext, PausedKey, new byte[] { 0x01 });
            Paused(invoker);
            return true;
        }

        [DisplayName("unpause")]
        public static object unpause(byte[] invoker)
        {
            BasicMethods.assert(Runtime.CheckWitness(invoker), "CheckWitness failed");

            _onlyPauser(invoker);
            _whenPaused();
            Storage.Put(Storage.CurrentContext, PausedKey, new byte[] { 0x00 });
            UnPaused(invoker);
            return true;
        }
    }
}
