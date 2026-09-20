using System;

/// <summary>
/// 数据单例
/// </summary>
public abstract class SingletonLock<T> where T : class, new()
{
    private static T instance;
    private static readonly object lockObject = new object();
    private static bool isInitialized = false;

    public static T self
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new T();
                        isInitialized = true;
                    }
                }
            }
            return instance;
        }
    }
    protected string sign = "";
    protected SingletonLock()
    {
        if (isInitialized==false)
        {
            sign=typeof(T).Name;
            Initialize();
        }
    }
    protected virtual void Initialize() { }
}