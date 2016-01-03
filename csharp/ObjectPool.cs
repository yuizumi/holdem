using System;
using System.Collections.Generic;

namespace Yuizumi.TexasHoldem
{
    public class ObjectPool<TKey, TObject>
    {
        private readonly Dictionary<TKey, TObject> mObjects;
        private readonly Func<TKey, TObject> mFactory;

        public ObjectPool(Func<TKey, TObject> factory)
        {
            VerifyArg.NotNull(factory, nameof(factory));
            mObjects = new Dictionary<TKey, TObject>( );
            mFactory = factory;
        }

        public TObject Get(TKey key)
        {
            TObject instance;
            if (mObjects.TryGetValue(key, out instance)) return instance;
            return (mObjects[key] = mFactory(key));
        }
    }
}
