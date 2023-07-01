using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZhuanTiNanMin
{
    namespace Singleton
    {
        /// <summary>
        /// 單例模式靜態存取
        /// </summary>
        public static class Singleton<T> where T : Object
        {
            private static readonly object _lock = new object();
            private static T _instance = null;

            public static T Instance
            {
                get
                {
                    return _instance;
                }
                set
                {
                    lock (_lock)
                    {
                        if (_instance == null) { _instance = value; }
                        else { Object.Destroy(value); }
                    }
                }
            }
        }
    }

    namespace ObjectPool
    {
        /// <summary>
        /// 物件池靜態方法
        /// </summary>
        public static class ObjectPool
        {
            /// <summary>
            /// 使用物件池資料生成物件
            /// </summary>
            /// <param name="poolInfo">物件池資料</param>
            /// <param name="itemInfo">物件初始化資料</param>
            public static void Spawn(PoolIDInfo poolInfo, ObjInfoBase itemInfo)
            {
                if (poolInfo.Pool.Count == 0)
                {
                    GameObject item = Object.Instantiate(poolInfo.Prefab);
                    item.GetComponent<IPoolObject>().Init(itemInfo);
                }
                else
                {
                    GameObject item = poolInfo.Pool[0];
                    item.GetComponent<IPoolObject>().Init(itemInfo);
                    poolInfo.Pool.Remove(item);
                    item.SetActive(true);
                }
            }

            /// <summary>
            /// 使用物件池資料回收物件
            /// </summary>
            /// <param name="poolInfo">物件池資料</param>
            /// <param name="item">回收物件</param>
            public static void Recycle(PoolIDInfo poolInfo, GameObject item)
            {
                poolInfo.Pool.Add(item);
                item.SetActive(false);
            }
        }

        /// <summary>
        /// 物件池儲存資料
        /// </summary>
        [System.Serializable]
        public sealed class PoolIDInfo
        {
            public GameObject Prefab;
            public readonly List<GameObject> Pool = new List<GameObject>();
        }

        /// <summary>
        /// 物件初始化資料
        /// </summary>
        public abstract class ObjInfoBase { }

        /// <summary>
        /// <br>物件池物件介面</br>
        /// <br>使用此介面的物件可通過物件池生成</br>
        /// </summary>
        public interface IPoolObject
        {
            /// <summary>
            /// 物件初始化方法
            /// </summary>
            /// <param name="info">
            /// 物件初始化資料
            /// </param>
            void Init(ObjInfoBase info);
        }
    }

    namespace FSMachine
    {
        /// <summary>
        /// 狀態內行為
        /// </summary>
        public abstract class ActionBase
        {
            /// <summary>
            /// 執行狀態內行為
            /// </summary>
            public abstract void Act(FSMachine fsm);
        }

        /// <summary>
        /// 狀態轉換判斷
        /// </summary>
        public abstract class DecisionBase
        {
            /// <summary>
            /// 執行狀態轉換判斷
            /// </summary>
            /// <returns>
            /// 判斷後的轉換狀態
            /// 不須轉換則為 null
            /// </returns>
            public abstract StateBase Decide(FSMachine fsm);
        }

        /// <summary>
        /// 狀態機狀態
        /// </summary>
        public abstract class StateBase
        {
            /// <summary>
            /// 可轉換新狀態
            /// </summary>
            private StateBase _newState = null;

            /// <summary>
            /// 轉換至新狀態
            /// </summary>
            protected void TransferState(FSMachine fsm)
            {
                if (_newState != null)
                {
                    fsm.Transition(_newState);
                }
            }

            /// <summary>
            /// 狀態內行為集合
            /// </summary>
            protected abstract List<ActionBase> Actions { get; }

            /// <summary>
            /// 執行狀態行為
            /// </summary>
            protected void DoAct(FSMachine fsm)
            {
                if (Actions != null)
                {
                    foreach (ActionBase action in Actions)
                    {
                        action.Act(fsm);
                    }
                }
            }

            /// <summary>
            /// 狀態內判斷集合
            /// </summary>
            protected abstract List<DecisionBase> Decisions { get; }

            /// <summary>
            /// 執行狀態判斷
            /// </summary>
            protected void DoDecide(FSMachine fsm)
            {
                if (Decisions != null)
                {
                    foreach (DecisionBase decision in Decisions)
                    {
                        StateBase temp;
                        temp = decision.Decide(fsm);
                        if (temp != null) { _newState = temp; }
                    }
                }
            }

            /// <summary>
            /// 當進入狀態時呼叫
            /// </summary>
            public virtual void OnStateEnter(FSMachine fsm)
            {
                //Debug.Log($"{fsm.Controller.gameObject} 進入狀態 {this}");
            }

            /// <summary>
            /// 當退出狀態時呼叫
            /// </summary>
            public virtual void OnStateExit(FSMachine fsm)
            {
                //Debug.Log($"{fsm.Controller.gameObject} 退出狀態 {this}");
            }

            /// <summary>
            /// 當狀態執行時呼叫
            /// </summary>
            public virtual void OnStateExecute(FSMachine fsm)
            {
                DoAct(fsm);
                DoDecide(fsm);
                TransferState(fsm);
            }
        }

        /// <summary>
        /// 有限狀態機
        /// </summary>
        public sealed class FSMachine
        {
            public StateBase InitialState { get; private set; }
            public StateBase CurrentState { get; private set; }

            public Object Controller { get; private set; }

            private bool _stateIsChanging;

            public FSMachine(Object controller, StateBase initialState)
            {
                if (controller != null && initialState != null)
                {
                    _stateIsChanging = true;
                    Controller = controller;
                    InitialState = initialState;
                    CurrentState = initialState;
                    CurrentState.OnStateEnter(this);
                    _stateIsChanging = false;
                }
            }

            public void UpdateFSM()
            {
                if (!_stateIsChanging && CurrentState != null)
                {
                    CurrentState.OnStateExecute(this);
                }
            }

            public void Transition(StateBase transitionTo)
            {
                if (!_stateIsChanging && CurrentState != null && transitionTo != null)
                {
                    if (transitionTo.GetType() != CurrentState.GetType())
                    {
                        _stateIsChanging = true;
                        CurrentState.OnStateExit(this);
                        CurrentState = transitionTo;
                        CurrentState.OnStateEnter(this);
                        _stateIsChanging = false;
                    }
                }
            }
        }
    }

    namespace Mathematics
    {
        /// <summary>
        /// 軸向
        /// </summary>
        public enum Axis
        {
            X,
            Y,
            Z,
        }

        /// <summary>
        /// 運算列舉
        /// </summary>
        public enum Operation
        {
            /// <summary>賦值</summary>
            Assignment,
            /// <summary>加法</summary>
            Addition,
            /// <summary>減法</summary>
            Subtraction,
            /// <summary>乘法</summary>
            Multiplication,
            /// <summary>除法</summary>
            Division,
        }

        /// <summary>
        /// 靜態運算函式庫
        /// </summary>
        public static class Funclib
        {
            /// <summary>
            /// <br>返回以 center 為圓心</br>
            /// <br>半徑為 radius 的圓內隨機點</br>
            /// </summary>
            public static Vector2 RandomInsideCircle(Vector2 center, float radius = 0.5f)
            {
                return Random.insideUnitCircle * Mathf.Abs(radius) + center;
            }

            /// <summary>
            /// 返回隨機標準化 2D 向量
            /// </summary>
            public static Vector2 RandomDirection2D { get => RandomInsideCircle(Vector2.zero).normalized; }

            /// <summary>
            /// 建立指定方向的 2D 旋轉
            /// </summary>
            /// <param name="start">方向起點</param>
            /// <param name="end">方向終點</param>
            /// <param name="offset">角度偏移</param>
            public static Quaternion LookRotation2D(Vector2 start, Vector2 end, float offset = 0f)
            {
                //Vector2 dir = (end - start).normalized;
                //transform.right = dir;

                //Vector2 dir = (end - start).normalized;
                //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                //angle += offset;
                //transform.rotation = Quaternion.Euler(0f, 0f, angle);

                //Vector2 dir = (end - start).normalized;
                //float angle = Vector3.Angle(dir, Vector3.right);
                //float dot = Vector3.Dot(dir, Vector3.up);
                //if (dot < 0f) { angle = 360f - angle; }
                //angle += offset;
                //transform.rotation = Quaternion.Euler(0f, 0f, angle);

                Vector2 dir = (end - start).normalized;
                if (dir == Vector2.zero) { return Quaternion.identity; }
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                angle += offset;
                return Quaternion.Euler(0f, 0f, angle);
            }

            /// <summary>
            /// 根據 value 值反轉 axis 軸方向
            /// </summary>
            /// <param name="reverse">是否反相判定 value 值</param>
            public static void AxisFlip(Transform transform, Axis axis, float value, bool reverse = false)
            {
                float sign = Mathf.Sign(value);
                if (reverse) { sign = -sign; }
                float x = transform.localScale.x;
                float y = transform.localScale.y;
                float z = transform.localScale.z;
                switch (axis)
                {
                    case Axis.X:
                        x = Mathf.Abs(x) * sign;
                        break;
                    case Axis.Y:
                        y = Mathf.Abs(y) * sign;
                        break;
                    case Axis.Z:
                        z = Mathf.Abs(z) * sign;
                        break;
                    default:
                        return;
                }
                transform.localScale = new Vector3(x, y, z);
            }

            /// <summary>
            /// 色彩調整
            /// <br>以 color 取代或相加、相減、相乘自身顏色</br>
            /// </summary>
            /// <param name="includeChildren">是否包含子項</param>
            public static void AdjustColor(GameObject gameObject, Color color, Operation operation, bool includeChildren = true)
            {
                SpriteRenderer[] sprites;
                if (includeChildren) { sprites = gameObject.GetComponentsInChildren<SpriteRenderer>(); }
                else { sprites = gameObject.GetComponents<SpriteRenderer>(); }
                if (sprites != null)
                {
                    foreach (SpriteRenderer sprite in sprites)
                    {
                        switch (operation)
                        {
                            case Operation.Assignment:
                                sprite.color = color;
                                break;
                            case Operation.Addition:
                                sprite.color += color;
                                break;
                            case Operation.Subtraction:
                                sprite.color -= color;
                                break;
                            case Operation.Multiplication:
                                sprite.color *= color;
                                break;
                            default:
                                return;
                        }
                    }
                }
            }

            /// <summary>
            /// 添加顏色至富文本
            /// </summary>
            public static string AddColorToString(string str, Color color)
            {
                return string.Format($"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{str}</color>");
            }

            /// <summary>
            /// 數值平滑處理:
            /// <br>將 smooth 往 origin 每秒偏移一個 offset</br>
            /// <br>僅當 smooth 與 origin 小於 bias 時回傳 origin</br>
            /// </summary>
            /// <param name="origin">原始數值</param>
            /// <param name="smooth">平滑處理數值</param>
            /// <param name="offset">偏移量</param>
            /// <param name="bias">容許誤差值</param>
            /// <returns>平滑處理後的值</returns>
            public static float Smoothing(float origin, float smooth, float offset, float bias)
            {
                if (Mathf.Abs(smooth - origin) < bias) { return origin; }
                if (origin > smooth)
                {
                    smooth += offset * Time.deltaTime;
                    if (smooth > origin) { return origin; }
                    else { return smooth; }
                }
                else if (origin < smooth)
                {
                    smooth -= offset * Time.deltaTime;
                    if (smooth < origin) { return origin; }
                    else { return smooth; }
                }
                else
                {
                    return smooth;
                }
            }

            //
        }

        /// <summary>
        /// 隨機不重複整數
        /// </summary>
        public class NonRepeating
        {
            private readonly System.Random _random = new System.Random();
            private readonly object _lock = new object();
            private readonly int _count;
            private List<int> _list;
            private int _index;

            /// <summary>
            /// 建立從 0 開始的 count 個整數清單以隨機抽取
            /// </summary>
            /// <param name="count">
            /// 當 count 不是正數時
            /// 抽取結果必為 0
            /// </param>
            public NonRepeating(int count)
            {
                _count = count > 0 ? count : 1;
                Relist();
            }

            /// <summary>
            /// 重新隨機排序清單內容
            /// </summary>
            private void Relist()
            {
                _list = Enumerable.Range(0, _count).OrderBy(x => _random.Next()).ToList();
                _index = 0;
            }

            /// <summary>
            /// 隨機抽取
            /// </summary>
            public int NextValue
            {
                get
                {
                    lock (_lock)
                    {
                        if (_index == _list.Count) { Relist(); }
                        int value = _list[_index];
                        _index++;
                        return value;
                    }
                }
            }

            private static readonly System.Random _listRnd = new System.Random();
            private static readonly object _listLock = new object();

            /// <summary>
            /// 生成隨機不重複整數清單
            /// </summary>
            /// <param name="start">首項</param>
            /// <param name="count">項數</param>
            public static List<int> RandomList(int start, int count)
            {
                return RandomList(start, count, count);
            }

            /// <summary>
            /// 生成隨機不重複整數清單
            /// </summary>
            /// <param name="start">首項</param>
            /// <param name="count">項數</param>
            /// <param name="take">取其中項數</param>
            public static List<int> RandomList(int start, int count, int take)
            {
                lock (_listLock)
                {
                    return Enumerable.Range(start, count).OrderBy(x => _listRnd.Next()).Take(take).ToList();
                }
            }
        }

        //
    }
}
