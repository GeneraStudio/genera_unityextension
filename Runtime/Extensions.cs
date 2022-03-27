using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class Extensions
{
    [Tooltip("Doesn't work on none stored variable")]
    public static Vector2 Clamp(this Vector2 v2, float xMin, float xMax, float yMin, float yMax) => new Vector2(Mathf.Clamp(v2.x, xMin, xMax), Mathf.Clamp(v2.y, yMin, yMax));
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1) { n--; int k = Random.Range(0, n + 1); T value = list[k]; list[k] = list[n]; list[n] = value; }
    }
    public static float SmoothStep7(this float x) => 924 * Mathf.Pow(x, 13) - 6006 * Mathf.Pow(x, 12) + 16380 * Mathf.Pow(x, 11) - 24024 * Mathf.Pow(x, 10) + 20020 * Mathf.Pow(x, 9) - 9009 * Mathf.Pow(x, 8) + 1716 * Mathf.Pow(x, 7);
    public static ColorBlock SetColorMultiplier(this ColorBlock colorBlock, float colorMultiplier)
    {
        ColorBlock cache = colorBlock;
        cache.colorMultiplier = colorMultiplier;
        return cache;
    }
    public static bool IsSubOrClassOf(this Type type, Type parent) => type.IsSubclassOf(parent) || type == parent;
    public static Vector3 RotateAround(Vector3 point, Vector3 pivot, Vector3 angles) => Quaternion.Euler(angles) * (point - pivot) + pivot;
    public static double Abs(this double dbl) => dbl > 0 ? dbl : dbl * -1;
    public static char Last(this string str) => str.Length != 0 ? str[str.Length - 1] : char.MinValue;
    public static char First(this string str) => str.Length != 0 ? str[0] : char.MinValue;
    public static float ToVolume(this float value) => value < 0.2f ? value.Remap(0f, 0.2f, -80, -12f) : value.Remap(0.2f, 1f, -12f, 0f);
    public static float Fromvolume(this float volume) => volume < -12f ? volume.Remap(-80, -12f, 0f, 0.2f) : volume.Remap(-12f, 0f, 0.2f, 1f);
    public static bool IsValid(this RaycastHit hit) => hit.normal != Vector3.zero;
    public static string ToHardcoin(this int hardcoin)
    {
        if (hardcoin < 999) return hardcoin.ToString();
        if (hardcoin < 999999) return (hardcoin / 1000f).ToString("0.0") + " K";
        if (hardcoin < 999999999) return (hardcoin / 1000000f).ToString("0.00") + " M";
        if (hardcoin < int.MaxValue) return (hardcoin / 1000000000f).ToString("0.000") + " G";
        else return "HC over 9000";
    }
    public static float Distance(this Transform transform, Vector3 pos) => Vector3.Distance(transform.position, pos);
    public static float Distance(this Transform transform, Transform othertransform) => Vector3.Distance(transform.position, othertransform.position);
    public static void ClearChild(this Transform transform, bool Immediately = false)
    {
        foreach (Transform child in transform)
        {
            if (Immediately) Object.DestroyImmediate(child.gameObject);
            else Object.Destroy(child.gameObject);
        }
    }
    public static float Remap(this float f, float IMin, float IMax, float OMin, float Omax) => OMin + (f - IMin) * (Omax - OMin) / (IMax - IMin);
    public static bool TryGetComponent<T>(this GameObject Go) => Go.TryGetComponent(out T found);
    public static bool TryGetComponent<T>(this Component cpnt) => cpnt.TryGetComponent(out T found);
    public static T AddComponent<T>(this GameObject game, T duplicate) where T : Component
    {
        T target = game.AddComponent<T>();
        foreach (PropertyInfo x in typeof(T).GetProperties()) if (x.CanWrite) x.SetValue(target, x.GetValue(duplicate));
        return target;
    }
    public static T GetComponent<T>(this GameObject Go, out T Component)
    {
        T Return = Go.GetComponent<T>();
        Component = Return;
        return Return;
    }
    public static T GetComponent<T>(this Component Cpnt, out T Component)
    {
        T Return = Cpnt.GetComponent<T>();
        Component = Return;
        return Return;
    }
    public static T GetComponentInChildren<T>(this GameObject GO, out T Component)
    {
        T Return = GO.GetComponentInChildren<T>();
        Component = Return;
        return Return;
    }
    public static bool TryGetComponentInChildren<T>(this GameObject Go, out T Component)
    {
        Component = Go.GetComponentInChildren<T>();
        return Component != null;
    }
    public static bool TryGetComponentInChildren<T>(this Component Go, out T Component)
    {
        Component = Go.GetComponentInChildren<T>();
        return Component != null;
    }
    public static bool TryGetComponentInChildren<T>(this MonoBehaviour Go, out T Component)
    {
        Component = Go.GetComponentInChildren<T>();
        return Component != null;
    }
    public static Vector3 Direction(this Vector3 vec3, Vector3 from, Vector3 to) => (to - from).normalized;
    public static Vector3 Direction01(this Vector3 vec3, Vector3 from, Vector3 to)
    {
        Vector3 dir = (to - from);
        return (dir.magnitude > 1f) ? dir.normalized : dir;
    }
    public static List<T> AddRange<T>(this List<T> list, IEnumerable<T> collection)
    {
        list.AddRange(collection);
        return list;
    }
    public static T Find<T>(this List<T> list, Predicate<T> predicate) => list.Exists(predicate) ? list.Find(predicate) : default;
    public static bool Find<T>(this List<T> list, Predicate<T> predicate, out T item) { item = list.Find(predicate); return list.Exists(predicate); }
    public static void Remove<T>(this List<T> list, Predicate<T> predicate) => list.Remove(list.Find(predicate));
    public static void Remove<T>(this List<T> list, Predicate<T> predicate, out T removed) { removed = list.Find(predicate); list.Remove(removed); }
    public static T Pick<T>(this List<T> list, Predicate<T> predicate) { T removed = list.Find(predicate); list.Remove(removed); return removed; }
    public static T AddAndReturn<T>(this List<T> list, T newItem)
    {
        list.Add(newItem);
        return newItem;
    }
    public static void LookAt(this Transform transform, Vector3 target, float smooth, bool worldUp = true)
    {
        Quaternion from = transform.rotation;
        if (worldUp) transform.LookAt(target, Vector3.up);
        else transform.LookAt(target);
        Quaternion to = transform.rotation;
        transform.rotation = Quaternion.Slerp(from, to, smooth);
    }
    public static void LookAt(this Transform transform, Transform target, float smooth, bool worldUp = true)
    {
        Quaternion from = transform.rotation;
        if (worldUp) transform.LookAt(target, Vector3.up);
        else transform.LookAt(target);
        Quaternion to = transform.rotation;
        transform.rotation = Quaternion.Slerp(from, to, smooth);
    }
    public static void LookAt(this Transform transform, Vector3 target, float smooth, Vector3 up)
    {
        Quaternion from = transform.rotation;
        transform.LookAt(target, up);
        Quaternion to = transform.rotation;
        transform.rotation = Quaternion.Slerp(from, to, smooth);
    }
    public static void LookAt(this Transform transform, Transform target, float smooth, Vector3 up)
    {
        Quaternion from = transform.rotation;
        transform.LookAt(target, up);
        Quaternion to = transform.rotation;
        transform.rotation = Quaternion.Slerp(from, to, smooth);
    }
    public static string HashCode() => "#" + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString();
    public static T GetRandom<T>(this T[] array) => array.Length > 0 ? array[Random.Range(0, array.Length)] : default;
    public static T GetRandom<T>(this List<T> list) => list.Count > 0 ? list[Random.Range(0, list.Count)] : default;
    public static string GetCode(this string name) => name.Substring(name.LastIndexOf('_') + 1, name.Length - name.LastIndexOf('_') - 1);
    public static void Play(this AudioSource audioSource, AudioClip audioClip, bool loop = false, bool forceReplay = false, float start = 0f)
    {
        if (audioSource != null)
        {
            if (forceReplay || audioSource.clip != audioClip || !audioSource.isPlaying)
            {
                if (audioSource.clip != audioClip) audioSource.clip = audioClip;
                audioSource.time = Mathf.Clamp(start, 0f, audioClip.length - 0.1f);
                audioSource.loop = loop;
                audioSource.Play();
            }
        }
    }

    public static void SetLayerRecursively(this GameObject obj, int newLayer, params string[] ignores)
    {
        foreach (var ignore in ignores) if (ignore[0] != '~' && obj.name == ignore) return;
        obj.layer = newLayer;
        foreach (var ignore in ignores) if (ignore[0] == '~' && obj.name == ignore.Substring(1, ignore.Length - 1)) return;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer, ignores);
    }
    public static void AddOnce<T>(this List<T> list, T item) { if (!list.Contains(item)) list.Add(item); }
    public static Vector3 ToVector3(this float v1) => new Vector3(v1, v1, v1);
    public static Vector2 ToVector2(this float v1) => new Vector2(v1, v1);
    public static float Negative(this float number) => -Mathf.Abs(number);
    public static float Positive(this float number) => Mathf.Abs(number);
    public static void PlayIfNot(this AudioSource audioSource) { if (!audioSource.isPlaying) audioSource.Play(); }
    public static void StopIfNot(this AudioSource audioSource) { if (audioSource.isPlaying) audioSource.Stop(); }
    public static void DestroyIfExist(this Object obj, Object toDestroy, bool immediate = false)
    {
        if (toDestroy)
        {
            if (immediate) Object.DestroyImmediate(toDestroy);
            else Object.Destroy(toDestroy);
        }
    }
    public static T DontDestroyOnLoad<T>(this T obj) where T : Object
    {
        Object.DontDestroyOnLoad(obj);
        return obj;
    }
    public static GameObject InstantiateAndName(this Object none, GameObject prefab, string name, bool protect = false)
    {
        GameObject Obj = Object.Instantiate(prefab);
        Obj.name = name;
        if (protect) Object.DontDestroyOnLoad(Obj);
        GameObject.FindObjectOfType<Mesh>();
        return Obj;
    }
    public static float GetFloat(this AudioMixer audioMixer, string name)
    {
        audioMixer.GetFloat(name, out float value);
        return value;
    }
    public static long Clamp(this long value, long min, long max) => value > max ? max : value < min ? min : value;
    public static uint Clamp(this uint value, uint min, uint max) => value > max ? max : value < min ? min : value;
    public static Vector3 RotateX(this Vector3 v3, float angle)
    {
        float x = 1f * v3.x + 0f * v3.y + 0f * v3.z;
        float y = 0f * v3.x + Mathf.Cos(angle * Mathf.Deg2Rad) * v3.y - Mathf.Sin(angle * Mathf.Deg2Rad) * v3.z;
        float z = 0f * v3.x + Mathf.Sin(angle * Mathf.Deg2Rad) * v3.y + Mathf.Cos(angle * Mathf.Deg2Rad) * v3.z;
        return new Vector3(x, y, z);
    }
    public static Vector3 RotateZ(this Vector3 v3, float angle)
    {
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * v3.x + -Mathf.Sin(angle * Mathf.Deg2Rad) * v3.y + 0f * v3.z;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * v3.x + Mathf.Cos(angle * Mathf.Deg2Rad) * v3.y - 0f * v3.z;
        float z = 0f * v3.x + 0f * v3.y + 1f * v3.z;
        return new Vector3(x, y, z);
    }
    //https://fr.wikipedia.org/wiki/Matrice_de_rotation
    /*public static Vector3 RotateY(this Vector3 v3, float angle)
    {
        float x = 1f * v3.x + 0f * v3.y + 0f * v3.z;
        float y = 0f * v3.x + Mathf.Cos(angle * Mathf.Deg2Rad) * v3.y - Mathf.Sin(angle * Mathf.Deg2Rad) * v3.z;
        float z = 0f * v3.x + Mathf.Sin(angle * Mathf.Deg2Rad) * v3.y + Mathf.Cos(angle * Mathf.Deg2Rad) * v3.z;
        return new Vector3(x, y, z);
    }*/
    public static bool TrueContains(this string strg, string value) => string.IsNullOrEmpty(value) ? false : strg.Contains(value);
    public static T Get<T>(this InputValue inputValue, out T value) where T : struct => value = inputValue.Get<T>();
    public static bool IsValid(this UnityWebRequest www)
    {
        bool isValid = www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError;
        if (!isValid) Debug.LogWarning(www.error);
        return isValid;
    }
}

[Serializable]
public class Carrousel<T>
{
    public List<T> Array = new List<T>(0);
    public int Index { get; private set; }
    public void SetIndex(T item)
    {
        Index = Array.IndexOf(item);
        Updated?.Invoke();
    }
    public void SetIndex(Predicate<T> match)
    {
        Index = Array.FindIndex(match);
        Updated?.Invoke();
    }
    public T First => Array[0];
    public T Last => Array[Array.Count - 1];
    public T Current => Array[Index];
    public T Next => GetValue(1);
    public T Previous => GetValue(-1);
    public T GetValue(int offset = 0) => Array[((Index + offset) % Array.Count < 0) ? (((Index + offset) % Array.Count) + Array.Count) : ((Index + offset) % Array.Count)];
    public T Random => Array[UnityEngine.Random.Range(0, Array.Count - 1)];
    public Carrousel(List<T> list) { Array = list; Index = 0; Updated?.Invoke(); }
    public void SetValues(List<T> newValues) { Array = newValues; Index = 0; Updated?.Invoke(); }
    public void Increment() { Index = (Index + 1) % Array.Count; Updated?.Invoke(); }
    public void ClampedIncrement() { Index = (Index + 1) > Array.Count - 1 ? Index : Index + 1; Updated?.Invoke(); }
    public void Decrement() { Index = (Index - 1) % Array.Count < 0 ? ((Index - 1) % Array.Count) + Array.Count : (Index - 1) % Array.Count; Updated?.Invoke(); }
    public void ClampdDecrement() { Index = (Index - 1) < 0 ? 0 : Index - 1; Updated?.Invoke(); }
    public event Action Updated;
}
