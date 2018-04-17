﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AElf.Kernel.Storages;

namespace AElf.Kernel
{
    public class WorldStateManager: IWorldStateManager
    {
        private readonly IWorldStateStore _worldStateStore;
        private readonly IPointerCollection _pointerCollection;
        private Hash _preBlockHash;
        private readonly IAccountContextService _accountContextService;
        private readonly IChangesCollection _changesCollection;

        public WorldStateManager(IWorldStateStore worldStateStore, Hash preBlockHash, 
            IAccountContextService accountContextService, IPointerCollection pointerCollection, IChangesCollection changesCollection)
        {
            _worldStateStore = worldStateStore;
            _preBlockHash = preBlockHash;
            _accountContextService = accountContextService;
            _pointerCollection = pointerCollection;
            _changesCollection = changesCollection;
        }
        
        /// <summary>
        /// Get any previous WorldState
        /// </summary>
        /// <param name="chainId"></param>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        public async Task<WorldState> GetWorldStateAsync(Hash chainId, Hash blockHash)
        {
            return await _worldStateStore.GetWorldState(chainId, blockHash);
        }

        public IAccountDataProvider GetAccountDataProvider(Hash chainId, Hash accountHash)
        {
            return new AccountDataProvider(accountHash, chainId, _accountContextService,
                _pointerCollection, this, _preBlockHash, _changesCollection);
        }
        
        public async Task SetData(Hash pointerHash, byte[] data)
        {
            await _worldStateStore.SetData(pointerHash, data);
        }

        public async Task<byte[]> GetData(Hash pointerHash)
        {
            return await _worldStateStore.GetData(pointerHash);
        }

        public async Task SetWorldStateToCurrentState(Hash chainId, Hash newBlockHash)
        {
            await _worldStateStore.InsertWorldState(chainId, _preBlockHash, _changesCollection);
            _preBlockHash = newBlockHash;
        }
    }
}