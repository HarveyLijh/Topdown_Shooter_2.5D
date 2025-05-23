﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Micosmo.SensorToolkit
{
    public struct WaitForSecondsCache {
        WaitForSeconds w;
        float s;
        public WaitForSeconds WaitForSeconds(float s) {
            if (this.s == s && w != null) {
                return w;
            }
            w = new WaitForSeconds(s);
            this.s = s;
            return w;
        }
    }

    public struct ComponentCache {
        enum InvokeType { GetComponent, GetComponentInParent, GetComponentInChildren }

        Component component;
        System.Type type;
        GameObject obj;
        InvokeType prevInvokeType;

        public T GetComponent<T>(GameObject ofObj) where T : class {
            checkInvokeType(InvokeType.GetComponent);

            if (type == typeof(T) && ReferenceEquals(ofObj, obj)) {
                return component as T;
            } else {
                component = null;
                if (ofObj != null) {
                    component = ofObj.GetComponent<T>() as Component;
                }
                type = typeof(T);
                obj = ofObj;
                return component as T;
            }
        }

        public T GetComponentInParent<T>(GameObject ofObj) where T : class {
            checkInvokeType(InvokeType.GetComponentInParent);

            if (type == typeof(T) && ReferenceEquals(ofObj, obj)) {
                return component as T;
            } else {
                component = null;
                if (ofObj != null) {
                    component = ofObj.GetComponentInParent<T>() as Component;
                }
                type = typeof(T);
                obj = ofObj;
                return component as T;
            }
        }

        public T GetComponentInChildren<T>(GameObject ofObj) where T : class {
            checkInvokeType(InvokeType.GetComponentInChildren);

            if (type == typeof(T) && ReferenceEquals(ofObj, obj)) {
                return component as T;
            } else {
                component = null;
                if (ofObj != null) {
                    component = ofObj.GetComponentInChildren<T>() as Component;
                }
                type = typeof(T);
                obj = ofObj;
                return component as T;
            }
        }

        void checkInvokeType(InvokeType nextInvokeType) {
            if (nextInvokeType != prevInvokeType) {
                component = null;
                obj = null;
            }
            prevInvokeType = nextInvokeType;
        }
    }

    public class ObjectCache<T> where T : new()
    {
        Stack<T> cache;

        public ObjectCache() : this(10) { }
        public ObjectCache(int startSize)
        {
            cache = new Stack<T>();
            for (int i = 0; i < startSize; i++) { cache.Push(create()); }
        }

        public T Get()
        {
            if (cache.Count > 0) return cache.Pop();
            else return create();
        }

        public virtual void Dispose(T obj)
        {
            cache.Push(obj);
        }

        protected virtual T create()
        {
            return System.Activator.CreateInstance<T>();
        }
    }

    public class ListCache<T> : ObjectCache<List<T>>
    {
        public override void Dispose(List<T> obj)
        {
            obj.Clear();
            base.Dispose(obj);
        }
    }
}
