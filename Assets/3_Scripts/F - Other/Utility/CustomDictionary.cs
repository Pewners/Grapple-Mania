using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomDictionary<TKey, TValue>
{
    public List<CustomDictionaryItem<TKey, TValue>> keyValuePairs;

    public TValue this[TKey key]
    {
        get
        {
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                if (keyValuePairs[i].key.Equals(key))
                {
                    return keyValuePairs[i].value;
                }
            }
            throw new KeyNotFoundException($"The key '{key}' was not found in the dictionary.");
        }
        set
        {
            bool found = false;
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                if (keyValuePairs[i].key.Equals(key))
                {
                    keyValuePairs[i].value = value;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                CustomDictionaryItem<TKey, TValue> newItem = new CustomDictionaryItem<TKey, TValue>();
                newItem.key = key;
                newItem.value = value;
                keyValuePairs.Add(newItem);
            }
        }
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            Debug.LogError($"Can't add {key} since CustomDictionary already contains key {value}: ");
            return;
        }

        CustomDictionaryItem<TKey, TValue> newItem = new CustomDictionaryItem<TKey, TValue>();
        newItem.key = key;
        newItem.value = value;
        keyValuePairs.Add(newItem);
    }

    public void Remove(TKey key)
    {
        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (keyValuePairs[i].key.Equals(key))
            {
                keyValuePairs.RemoveAt(i);
                return;
            }
        }
    }

    public bool ContainsKey(TKey key)
    {
        for (int i = 0; i < keyValuePairs.Count; i++)
        {
            if (keyValuePairs[i].key.Equals(key))
                return true;
        }
        return false;
    }
}

[Serializable]
public class CustomDictionaryItem<T1, T2>
{
    public T1 key;
    public T2 value;
}