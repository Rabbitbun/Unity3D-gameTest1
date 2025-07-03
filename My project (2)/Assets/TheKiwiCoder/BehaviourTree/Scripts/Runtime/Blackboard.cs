using System;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using System.Reflection;
using System.Linq;
using static TheKiwiCoder.Blackboard;

namespace TheKiwiCoder {
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Combat
    }
    public enum AICombatState
    {
        Idle,
        Observed,
        Attack,
        Flee,
        Dead
    }

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard
    {
        //private Dictionary<string, Type> propertyTypes = new Dictionary<string, Type>();
        //private Dictionary<string, bool> boolProperties = new Dictionary<string, bool>();

        //public Vector3 moveToPosition;
        //public AbilitySystemCharacter target;
        //public bool IsPlayerNearby
        //{
        //    get { return boolProperties["IsPlayerNearby"]; }
        //    set { boolProperties["IsPlayerNearby"] = value; }
        //}
        //public bool PauseCheck
        //{
        //    get { return boolProperties["PauseCheck"]; }
        //    set { boolProperties["PauseCheck"] = value; }
        //}

        //public Blackboard()
        //{
        //    // init
        //    FieldInfo[] fields = this.GetType().GetFields();
        //    foreach (var field in fields)
        //    {
        //        propertyTypes[field.Name] = field.FieldType;
        //        if (field.FieldType == typeof(bool))
        //        {
        //            boolProperties[field.Name] = (bool)field.GetValue(this);
        //        }
        //    }
        //    PropertyInfo[] properties = this.GetType().GetProperties();
        //    foreach (var property in properties)
        //    {
        //        propertyTypes[property.Name] = property.PropertyType;
        //        if (property.PropertyType == typeof(bool))
        //        {
        //            boolProperties[property.Name] = false;
        //        }
        //    }
        //}

        //public bool GetBool(string propertyName)
        //{
        //    return boolProperties[propertyName];
        //}

        //public Type GetPropertyType(string propertyName)
        //{
        //    // find in dic
        //    if (propertyTypes.TryGetValue(propertyName, out var propertyType))
        //    {
        //        return propertyType;
        //    }
        //    else
        //    {
        //        Debug.LogError($"Property '{propertyName}' not found in type '{GetType().Name}'");
        //        return null;
        //    }
        //}

        protected Dictionary<string, IBlackboardData> mBlackboardDataMap;
        

        public Blackboard()
        {
            mBlackboardDataMap = new Dictionary<string, IBlackboardData>();

            // 添加初始數據
            AddData("MoveSpeed", new FloatBlackboardData(0.0f));
            AddData("moveToPosition", new Vector3BlackboardData(Vector3.zero));
            AddData("target", new AbilitySystemCharacterBlackboardData(null));
            AddData("IsPlayerNearby", new BoolBlackboardData(false));
            AddData("PauseCheck", new BoolBlackboardData(false));
            AddData("AIState", new AIStateBlackboardData(AIState.Idle));
            AddData("AICombatState", new AICombatStateBlackboardData(AICombatState.Idle));

            AddData("IsInChaseRange", new BoolBlackboardData(false));
            AddData("IsInMeleeRange", new BoolBlackboardData(false));
            AddData("IsInObservedRange", new BoolBlackboardData(false));

            AddData("IsInFarDistanceRange", new BoolBlackboardData(false));
            AddData("IsInMiddleDistanceRange", new BoolBlackboardData(false));
            AddData("IsInCloseDistanceRange", new BoolBlackboardData(false));

            AddData("CanMeleeAttack", new BoolBlackboardData(false));
            AddData("AttackCoolDown", new FloatBlackboardData(0.0f));
            AddData("UsingAbility", new BoolBlackboardData(false));
        }

        public bool AddData(string key, IBlackboardData data)
        {
            if (!mBlackboardDataMap.ContainsKey(key))
            {
                mBlackboardDataMap.Add(key, data);
                return true;
            }
            else
            {
                Debug.LogError($"Blackboard裡已存在Key:{key}的data，添加data失敗!");
                return false;
            }
        }

        public bool RemoveData(string key)
        {
            return mBlackboardDataMap.Remove(key);
        }
        public string[] GetAllNames()
        {
            return mBlackboardDataMap.Keys.ToArray();
        }

        public T GetData<T>(string key)
        {
            var value = GetBlackboardData(key);
            if (value != null)
            {
                return ((BlackboardData<T>)value).Data;
            }
            else
            {
                Debug.LogError("返回默認值!");
                return default(T);
            }
        }

        public bool UpdateData<T>(string key, T data)
        {
            var value = GetBlackboardData(key);
            if (value != null)
            {
                ((BlackboardData<T>)value).Data = data;
                return true;
            }
            else
            {
                Debug.LogError($"更新Key:{key}的data失敗!");
                return false;
            }
        }

        public void ClearData()
        {
            mBlackboardDataMap.Clear();
        }

        public IBlackboardData GetBlackboardData(string key)
        {
            if (mBlackboardDataMap.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                Debug.LogError($"找不到Key:{key}的data!");
                return null;
            }
        }

        public Type GetDataType(string key)
        {
            var value = GetBlackboardData(key);
            
            if (value != null)
            {
                //var v = value.GetType().GetGenericArguments()[0];
                var v = value.GetType();
                //Debug.Log(v);
                return v;
            }
            else
            {
                Debug.LogError($"找不到Key:{key}的data!");
                return null;
            }
        }
    }

    public interface IBlackboardData { }
    public class BlackboardData<T> : IBlackboardData
    {
        public T Data { get; set; }

        public BlackboardData(T data)
        {
            Data = data;
        }
    }

    public class IntBlackboardData : BlackboardData<int>
    {
        public IntBlackboardData(int data) : base(data) { }
    }

    public class FloatBlackboardData : BlackboardData<float>
    {
        public FloatBlackboardData(float data) : base(data) { }
    }

    public class StringBlackboardData : BlackboardData<string>
    {
        public StringBlackboardData(string data) : base(data) { }
    }

    public class BoolBlackboardData : BlackboardData<bool>
    {
        public BoolBlackboardData(bool data) : base(data) { }
    }

    public class Vector3BlackboardData : BlackboardData<Vector3>
    {
        public Vector3BlackboardData(Vector3 data) : base(data) { }
    }

    public class AbilitySystemCharacterBlackboardData : BlackboardData<AbilitySystemCharacter>
    {
        public AbilitySystemCharacterBlackboardData(AbilitySystemCharacter data) : base(data) { }
    }

    public class AIStateBlackboardData : BlackboardData<AIState>
    {
        public AIStateBlackboardData(AIState data) : base(data) { }
    }

    public class AICombatStateBlackboardData : BlackboardData<AICombatState>
    {
        public AICombatStateBlackboardData(AICombatState data) : base(data) { }
    }
}