using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Data.Dynamic.Providers;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryDynamicDataStore : DynamicDataStore
    {
        private Dictionary<Guid, object> storeObjects = new Dictionary<Guid, object>();
        public InMemoryDynamicDataStore(StoreDefinition storeDefinition) : base(storeDefinition)
        {
            name = storeDefinition.StoreName;
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        public override Identity Save(object value)
        {
            var identity = Identity.NewIdentity();
            storeObjects[identity.ExternalId] = value;
            return identity;
        }

        public override Identity Save(object value, TypeToStoreMapper typeToStoreMapper)
        {
            throw new NotImplementedException();
        }

        public override Identity Save(object value, Identity id)
        {
            throw new NotImplementedException();
        }

        public override Identity Save(object value, Identity id, TypeToStoreMapper typeToStoreMapper)
        {
            throw new NotImplementedException();
        }

        public override object Load(Identity id)
        {
            throw new NotImplementedException();
        }

        public override TResult Load<TResult>(Identity id)
        {
            throw new NotImplementedException();
        }

        public override PropertyBag LoadAsPropertyBag(Identity id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<object> LoadAll()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TResult> LoadAll<TResult>()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PropertyBag> LoadAllAsPropertyBag()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<object> Find(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<object> Find(IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TResult> Find<TResult>(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<TResult> Find<TResult>(IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PropertyBag> FindAsPropertyBag(IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PropertyBag> FindAsPropertyBag(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public override IOrderedQueryable<object> Items()
        {
            throw new NotImplementedException();
        }

        public override IOrderedQueryable<TResult> Items<TResult>()
        {
            return storeObjects.Values.OfType<TResult>().AsQueryable().OrderBy(x => x.ToString());
        }

        public override IOrderedQueryable<PropertyBag> ItemsAsPropertyBag()
        {
            throw new NotImplementedException();
        }

        public override void Delete(Identity id)
        {
            throw new NotImplementedException();
        }

        public override void Delete(object value)
        {
            throw new NotImplementedException();
        }

        public override void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public override StoreDefinition StoreDefinition
        {
            get { throw new NotImplementedException(); }
        }

        string name;
        public override string Name
        {
            get { return name; }
        }

        public override DataStoreProvider DataStoreProvider
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
