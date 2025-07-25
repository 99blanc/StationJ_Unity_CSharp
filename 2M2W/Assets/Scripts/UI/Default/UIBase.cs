using System.Collections.Generic;
using System;
using TMPro;
using Object = UnityEngine.Object;
using UnityEngine.UI;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    private Dictionary<Type, Object[]> objects = new Dictionary<Type, Object[]>();

    public virtual void Init()
    {

    }

    private void Start()
        => Init();

    protected void Bind<T>(Type type) where T : Object
    {
        string[] names = Enum.GetNames(type);
        Object[] newObjects = new Object[names.Length];
        objects.Add(typeof(T), newObjects);

        for (int index = 0; index < names.Length; ++index)
        {
            if (typeof(T) == typeof(GameObject))
            {
                newObjects[index] = Utilities.FindChild(gameObject, names[index], true);
            }
            else
            {
                newObjects[index] = Utilities.FindChild<T>(gameObject, names[index], true);
            }
        }
    }

    protected void BindObject(Type type)
        => Bind<GameObject>(type);

    protected void BindRawImage(Type type)
        => Bind<RawImage>(type);

    protected void BindImage(Type type)
        => Bind<Image>(type);

    protected void BindText(Type type)
        => Bind<TMP_Text>(type);

    protected void BindButton(Type type)
        => Bind<Button>(type);

    protected void BindInputField(Type type)
    => Bind<TMP_InputField>(type);

    protected T Get<T>(int index) where T : Object
    {
        if (objects.TryGetValue(typeof(T), out Object[] newObjects))
        {
            return newObjects[index] as T;
        }

        throw new InvalidOperationException();
    }

    protected GameObject GetObject(int index)
        => Get<GameObject>(index);

    protected RawImage GetRawImage(int index)
        => Get<RawImage>(index);

    protected Image GetImage(int index)
        => Get<Image>(index);

    protected TMP_Text GetText(int index)
        => Get<TMP_Text>(index);

    protected Button GetButton(int index)
        => Get<Button>(index);

    protected TMP_InputField GetInputField(int index)
        => Get<TMP_InputField>(index);
}
