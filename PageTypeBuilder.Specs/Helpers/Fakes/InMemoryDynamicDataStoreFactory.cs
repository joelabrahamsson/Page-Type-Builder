using System;
using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryDynamicDataStoreFactory : DynamicDataStoreFactory
    {
        List<DynamicDataStore> stores = new List<DynamicDataStore>();

        public override DynamicDataStore CreateStore(string storeName, IDictionary<string, Type> typeBag)
        {
            throw new NotImplementedException();
        }

        public override DynamicDataStore CreateStore(string storeName, IDictionary<string, Type> typeBag, StoreDefinitionParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override DynamicDataStore CreateStore(Type type)
        {
            throw new NotImplementedException();
        }

        public override DynamicDataStore CreateStore(Type type, StoreDefinitionParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override DynamicDataStore CreateStore(string storeName, Type type)
        {
            var store = new InMemoryDynamicDataStore(new StoreDefinition(storeName, null));
            stores.Add(store);
            return store;
        }

        public override DynamicDataStore CreateStore(string storeName, Type type, StoreDefinitionParameters parameters)
        {
            throw new NotImplementedException();
        }

        public override DynamicDataStore GetStore(string storeName)
        {
            return stores.Find(store => store.Name.Equals(storeName));
        }

        public override DynamicDataStore GetStore(Type type)
        {
            throw new NotImplementedException();
        }

        public override DynamicDataStore GetStoreForItem(Identity itemId, string storeTableName)
        {
            throw new NotImplementedException();
        }

        public override void DeleteStore(string storeName, bool deleteObjects)
        {
            throw new NotImplementedException();
        }

        public override void DeleteStore(Type type, bool deleteObjects)
        {
            throw new NotImplementedException();
        }
    }
}
